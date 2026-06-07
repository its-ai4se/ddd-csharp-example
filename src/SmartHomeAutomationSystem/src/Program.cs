using SmartHomeAutomationSystem.Domain.Home;
using SmartHomeAutomationSystem.Domain.Room;
using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Automation;
using SmartHomeAutomationSystem.Domain.Automation.Precondition;
using SmartHomeAutomationSystem.Domain.Services;
using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

namespace SmartHomeAutomationSystem.Domain;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Smart Home Automation System — Domain Model Demo ===\n");

        // ── BR-016: Home must have exactly one physical address ──────────────
        var ownerId = Guid.NewGuid();
        var guestId = Guid.NewGuid();

        var home = new HomeAggregate("42 Maple Street, Montreal, QC", ownerId);
        Console.WriteLine($"[BR-016] Home at '{home.Address}'");
        Console.WriteLine($"[BR-008/BR-018] Owner id={ownerId}");

        // ── BR-017: Home must have one or more rooms ─────────────────────────
        var livingRoom = new RoomAggregate(new RoomName("Living Room"), home.Id);
        var bedroom    = new RoomAggregate(new RoomName("Bedroom"),     home.Id);
        home.AddRoom(livingRoom.Id);
        home.AddRoom(bedroom.Id);
        Console.WriteLine($"[BR-017] Rooms: {livingRoom.Name.Value}, {bedroom.Name.Value}");

        try
        {
            var tmp = new HomeAggregate("Y", ownerId);
            tmp.AddRoom(Guid.NewGuid());
            tmp.RemoveRoom(tmp.RoomIds[0]);
            tmp.RemoveRoom(tmp.RoomIds[0]);
        }
        catch (DomainException ex) { Console.WriteLine($"[BR-017] Guard: {ex.Message}"); }

        // ── BR-001: Unique device IDs ────────────────────────────────────────
        var motionSensor = new DeviceAggregate(new DeviceName("Motion Sensor"),      new DeviceType("MotionSensor"), livingRoom.Id);
        var doorLock     = new DeviceAggregate(new DeviceName("Front Door Lock"),    new DeviceType("DoorLock"),     livingRoom.Id);
        var thermostat   = new DeviceAggregate(new DeviceName("Bedroom Thermostat"), new DeviceType("Thermostat"),   bedroom.Id);
        livingRoom.AddDevice(motionSensor.Id);
        livingRoom.AddDevice(doorLock.Id);
        bedroom.AddDevice(thermostat.Id);
        Console.WriteLine($"[BR-001] Devices: {motionSensor.Id}, {doorLock.Id}, {thermostat.Id}");

        // ── BR-002: Activation/deactivation raises domain events ─────────────
        motionSensor.Activate();
        doorLock.Activate();
        thermostat.Activate();
        Console.WriteLine($"[BR-002] motionSensor events: {string.Join(", ", motionSensor.DomainEvents.Select(e => e.GetType().Name))}");
        motionSensor.Deactivate();
        Console.WriteLine($"[BR-002] After deactivate: {string.Join(", ", motionSensor.DomainEvents.Select(e => e.GetType().Name))}");
        motionSensor.Activate();

        // ── BR-003: Active sensor generates reading with value + timestamp ────
        var reading = motionSensor.GenerateReading(1.0, "motion", DateTime.UtcNow);
        Console.WriteLine($"[BR-003] SensorReading: {reading}");

        var offlineSensor = new DeviceAggregate(new DeviceName("Offline Sensor"), new DeviceType("SmokeDetector"), bedroom.Id);
        try { offlineSensor.GenerateReading(0.5, "smoke", DateTime.UtcNow); }
        catch (DomainException ex) { Console.WriteLine($"[BR-003] Guard: {ex.Message}"); }

        // ── BR-004/BR-005: Control command with timestamp + status; predefined only ──
        var lockCmd = doorLock.IssueCommand("lockDoor", DateTime.UtcNow);
        Console.WriteLine($"[BR-004/BR-005] Command: {lockCmd}");

        try { doorLock.IssueCommand("fly", DateTime.UtcNow); }
        catch (DomainException ex) { Console.WriteLine($"[BR-005] Guard: {ex.Message}"); }

        try { motionSensor.IssueCommand("lockDoor", DateTime.UtcNow); }
        catch (DomainException ex) { Console.WriteLine($"[BR-005] Guard (sensor): {ex.Message}"); }

        // ── BR-009/BR-010/BR-011/BR-012: Precondition + ActionSequence ───────
        var rule = new AutomationRuleAggregate(new AutomationRuleName("Lock door when motion detected"), home.Id);

        var precondition = new AndExpression(
            new AtomicCondition(motionSensor.Id, AtomicConditionKind.SensorReadingValue,    RelationalOperator.GreaterThan, 0),
            new AndExpression(
                new AtomicCondition(thermostat.Id,   AtomicConditionKind.SensorReadingValue,    RelationalOperator.GreaterThanOrEqual, 18),
                new AtomicCondition(livingRoom.Id,   AtomicConditionKind.RoomActiveDeviceCount, RelationalOperator.GreaterThan, 0)
            )
        );
        rule.SetPrecondition(precondition);

        var sequence = new ActionSequence(new DeviceType("DoorLock"), new[] { (doorLock.Id, "lockDoor") });
        rule.SetActionSequence(sequence);
        Console.WriteLine($"[BR-009/BR-011/BR-012] Rule has precondition and {rule.ActionSequence!.Steps.Count}-step action sequence");

        try { _ = new ActionSequence(new DeviceType("DoorLock"), new[] { (doorLock.Id, "fly") }); }
        catch (DomainException ex) { Console.WriteLine($"[BR-005/BR-012] Guard: {ex.Message}"); }

        // ── BR-013: Cannot edit an active rule ───────────────────────────────
        rule.Enable();
        try { rule.SetPrecondition(precondition); }
        catch (DomainException ex) { Console.WriteLine($"[BR-013] Guard: {ex.Message}"); }

        // ── BR-008/BR-018: Only owner may manage rules and alerts ─────────────
        Console.WriteLine($"[BR-008] home.IsOwner(owner): {home.IsOwner(ownerId)}");
        Console.WriteLine($"[BR-008] home.IsOwner(guest): {home.IsOwner(guestId)}");

        var alertService = new AlertService();
        var alert = alertService.CreateAlert(home, ownerId, "Motion detected at front door", motionSensor.Id);
        Console.WriteLine($"[BR-018] Alert created: '{alert.Description}'");

        try { alertService.CreateAlert(home, guestId, "Unauthorized alert", motionSensor.Id); }
        catch (DomainException ex) { Console.WriteLine($"[BR-018] Guard: {ex.Message}"); }

        // ── BR-014: Rule dependency/conflict tracking ─────────────────────────
        rule.Disable();
        var rule2 = new AutomationRuleAggregate(new AutomationRuleName("Night mode"), home.Id);
        rule2.SetPrecondition(new AtomicCondition(motionSensor.Id, AtomicConditionKind.SensorReadingValue, RelationalOperator.Equal, 0));
        rule2.SetActionSequence(new ActionSequence(new DeviceType("DoorLock"), new[] { (doorLock.Id, "unlockDoor") }));
        rule2.AddConflict(rule.Id);
        Console.WriteLine($"[BR-014] rule2 conflicts with rule: {rule2.HasConflictWith(rule.Id)}");

        // ── BR-015: Triggered rule raises domain event with timestamp ─────────
        rule.Enable();
        var ctx = new EvaluationContext();
        ctx.SetReading(motionSensor.Id, new SensorReading(1.0, "motion", DateTime.UtcNow));
        ctx.SetReading(thermostat.Id,   new SensorReading(20.0, "°C",   DateTime.UtcNow));
        ctx.SetRoomActiveDeviceCount(livingRoom.Id, 2);

        if (rule.CanExecute(ctx))
        {
            rule.MarkAsTriggered();
            Console.WriteLine($"[BR-015] Rule triggered at {rule.LastTriggeredAt:O}");
            Console.WriteLine($"[BR-015] Domain event: {rule.DomainEvents.Last().GetType().Name}");
        }

        Console.WriteLine("\n=== All business rules demonstrated successfully ===");
    }
}
