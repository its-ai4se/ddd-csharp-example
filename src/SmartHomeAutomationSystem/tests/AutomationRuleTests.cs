using SmartHomeAutomationSystem.Domain.Automation;
using SmartHomeAutomationSystem.Domain.Automation.Precondition;
using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Home;
using SmartHomeAutomationSystem.Domain.Services;
using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace SmartHomeAutomationSystem.Domain.Tests;

public class AutomationRuleTests
{
    private static readonly Guid OwnerId = Guid.NewGuid();
    private static readonly Guid OtherId = Guid.NewGuid();

    private static HomeAggregate CreateHome(Guid? ownerId = null)
        => new("123 Main St", ownerId ?? OwnerId);

    private static AutomationRuleAggregate CreateRule(Guid? homeId = null)
        => new(new AutomationRuleName("TestRule"), homeId ?? Guid.NewGuid());

    private static IPreconditionExpression MakePrecondition()
        => new AtomicCondition(Guid.NewGuid(), AtomicConditionKind.SensorReadingValue, RelationalOperator.LessThan, 18);

    private static ActionSequence MakeAction()
        => new(new DeviceType("Thermostat"), [(Guid.NewGuid(), "turnOnHeating")]);

    [Fact]
    public async Task AR001_Owner_CanCreateAutomationRule()
    {
        var home = CreateHome();
        var service = new AutomationService(new InMemoryAutomationRuleRepository());
        var rule = await service.CreateRuleAsync(new AutomationRuleName("Rule1"), home, OwnerId);

        Assert.NotNull(rule);
        Assert.Equal(home.Id, rule.HomeId);
    }

    [Fact]
    public async Task AR002_NonOwner_CannotCreateAutomationRule()
    {
        var home = CreateHome();
        var service = new AutomationService(new InMemoryAutomationRuleRepository());

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CreateRuleAsync(new AutomationRuleName("Rule1"), home, OtherId));
    }

    [Fact]
    public async Task AR003_Owner_CanEnableRule()
    {
        var home = CreateHome();
        var repo = new InMemoryAutomationRuleRepository();
        var service = new AutomationService(repo);
        var rule = await service.CreateRuleAsync(new AutomationRuleName("Rule1"), home, OwnerId);
        rule.SetPrecondition(MakePrecondition());
        rule.SetActionSequence(MakeAction());
        await repo.SaveAsync(rule);

        await service.EnableRuleAsync(rule.Id, home, OwnerId);
        Assert.True(rule.IsEnabled);
    }

    [Fact]
    public async Task AR004_Owner_CanDisableRule()
    {
        var home = CreateHome();
        var repo = new InMemoryAutomationRuleRepository();
        var service = new AutomationService(repo);
        var rule = await service.CreateRuleAsync(new AutomationRuleName("Rule1"), home, OwnerId);
        rule.SetPrecondition(MakePrecondition());
        rule.SetActionSequence(MakeAction());
        await repo.SaveAsync(rule);
        await service.EnableRuleAsync(rule.Id, home, OwnerId);

        await service.DisableRuleAsync(rule.Id, home, OwnerId);
        Assert.False(rule.IsEnabled);
    }

    [Fact]
    public async Task AR005_NonOwner_CannotEnableOrDisableRule()
    {
        var home = CreateHome();
        var repo = new InMemoryAutomationRuleRepository();
        var service = new AutomationService(repo);
        var rule = await service.CreateRuleAsync(new AutomationRuleName("Rule1"), home, OwnerId);
        rule.SetPrecondition(MakePrecondition());
        rule.SetActionSequence(MakeAction());
        await repo.SaveAsync(rule);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.EnableRuleAsync(rule.Id, home, OtherId));
    }

    [Fact]
    public async Task AR006_OwnerOfHomeA_CannotManageRulesOfHomeB()
    {
        var homeA = CreateHome(OwnerId);
        var homeB = CreateHome(OtherId);
        var repo = new InMemoryAutomationRuleRepository();
        var service = new AutomationService(repo);
        var ruleB = await service.CreateRuleAsync(new AutomationRuleName("RuleB"), homeB, OtherId);
        ruleB.SetPrecondition(MakePrecondition());
        ruleB.SetActionSequence(MakeAction());
        await repo.SaveAsync(ruleB);

        // OwnerId tries to enable rule in homeB using homeA context
        await Assert.ThrowsAsync<DomainException>(() =>
            service.EnableRuleAsync(ruleB.Id, homeA, OwnerId));
    }

    [Fact]
    public void AR007_RuleWithOnePreconditionAndOneAction_IsValid()
    {
        var rule = CreateRule();
        rule.SetPrecondition(MakePrecondition());
        rule.SetActionSequence(MakeAction());
        Assert.NotNull(rule.Precondition);
        Assert.NotNull(rule.ActionSequence);
    }

    [Fact]
    public void AR008_RuleWithoutPrecondition_CannotBeEnabled()
    {
        var rule = CreateRule();
        rule.SetActionSequence(MakeAction());
        var ex = Assert.Throws<DomainException>(() => rule.Enable());
        Assert.Contains("precondition", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AR009_RuleWithoutAction_CannotBeEnabled()
    {
        var rule = CreateRule();
        rule.SetPrecondition(MakePrecondition());
        var ex = Assert.Throws<DomainException>(() => rule.Enable());
        Assert.Contains("action", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AR010_RuleCanOnlyHaveOnePreconditionExpression()
    {
        var rule = CreateRule();
        var p1 = MakePrecondition();
        var p2 = MakePrecondition();
        rule.SetPrecondition(p1);
        // Setting again replaces - only one precondition at a time
        rule.SetPrecondition(p2);
        Assert.NotEqual(p1, rule.Precondition);
        Assert.Equal(p2, rule.Precondition);
    }

    [Fact]
    public void AR011_PreconditionWithAND_IsValid()
    {
        var rule = CreateRule();
        var and = new AndExpression(MakePrecondition(), MakePrecondition());
        rule.SetPrecondition(and);
        Assert.NotNull(rule.Precondition);
    }

    [Fact]
    public void AR012_PreconditionWithOR_IsValid()
    {
        var rule = CreateRule();
        var or = new OrExpression(MakePrecondition(), MakePrecondition());
        rule.SetPrecondition(or);
        Assert.NotNull(rule.Precondition);
    }

    [Fact]
    public void AR013_PreconditionWithNOT_IsValid()
    {
        var rule = CreateRule();
        var not = new NotExpression(MakePrecondition());
        rule.SetPrecondition(not);
        Assert.NotNull(rule.Precondition);
    }

    [Fact]
    public void AR014_PreconditionWithCombinedBooleanOperators_IsValid()
    {
        var rule = CreateRule();
        var combined = new OrExpression(
            new AndExpression(MakePrecondition(), new NotExpression(MakePrecondition())),
            MakePrecondition());
        rule.SetPrecondition(combined);
        Assert.NotNull(rule.Precondition);
    }

    [Fact]
    public void AR015_PreconditionWithXOR_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            PreconditionParser.Validate("temp < 18 XOR window = closed"));
        Assert.Contains("XOR", ex.Message);
    }

    [Fact]
    public void AR016_RelationalTermReferencingSensor_IsValid()
    {
        var sensorId = Guid.NewGuid();
        var condition = new AtomicCondition(sensorId, AtomicConditionKind.SensorReadingValue, RelationalOperator.GreaterThan, 25);
        Assert.Equal(sensorId, condition.ReferenceId);
    }

    [Fact]
    public void AR017_RelationalTermReferencingActuator_IsValid()
    {
        var actuatorId = Guid.NewGuid();
        var condition = new AtomicCondition(actuatorId, AtomicConditionKind.CommandStatus, RelationalOperator.Equal, (double)CommandStatus.Requested);
        Assert.Equal(actuatorId, condition.ReferenceId);
    }

    [Fact]
    public void AR018_RelationalTermReferencingRoom_IsValid()
    {
        var roomId = Guid.NewGuid();
        var condition = new AtomicCondition(roomId, AtomicConditionKind.RoomActiveDeviceCount, RelationalOperator.GreaterThan, 0);
        Assert.Equal(roomId, condition.ReferenceId);
    }

    [Fact]
    public void AR019_ActionWithOneCommand_IsValid()
    {
        var action = new ActionSequence(new DeviceType("Thermostat"), [(Guid.NewGuid(), "turnOnHeating")]);
        Assert.Single(action.Steps);
    }

    [Fact]
    public void AR020_ActionWithMultipleCommands_IsValid()
    {
        var action = new ActionSequence(
        [
            (Guid.NewGuid(), new DeviceType("DoorLock"), "lockDoor"),
            (Guid.NewGuid(), new DeviceType("Thermostat"), "turnOnHeating"),
            (Guid.NewGuid(), new DeviceType("Light"), "turnOff"),
        ]);
        Assert.Equal(3, action.Steps.Count);
        Assert.Equal("lockDoor", action.Steps[0].CommandName);
        Assert.Equal("turnOnHeating", action.Steps[1].CommandName);
        Assert.Equal("turnOff", action.Steps[2].CommandName);
    }

    [Fact]
    public void AR021_EmptyAction_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            new ActionSequence(new DeviceType("Thermostat"), []));
        Assert.Contains("at least one", ex.Message);
    }

    [Fact]
    public void AR022_ActionWithUnknownCommand_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() =>
            new ActionSequence(new DeviceType("Thermostat"),
            [
                (Guid.NewGuid(), "turnOnHeating"),
                (Guid.NewGuid(), "activateLaserCannon"),
            ]));
        Assert.Contains("not allowed", ex.Message);
    }

    [Fact]
    public void AR023_DeactivatedRule_CanBeEdited()
    {
        var rule = CreateRule();
        rule.SetPrecondition(MakePrecondition());
        rule.SetActionSequence(MakeAction());
        var newPrecondition = new AtomicCondition(Guid.NewGuid(), AtomicConditionKind.SensorReadingValue, RelationalOperator.GreaterThan, 30);
        rule.SetPrecondition(newPrecondition);
        Assert.Equal(newPrecondition, rule.Precondition);
    }

    [Fact]
    public void AR024_ActiveRule_CannotBeEdited()
    {
        var rule = CreateRule();
        rule.SetPrecondition(MakePrecondition());
        rule.SetActionSequence(MakeAction());
        rule.Enable();
        var ex = Assert.Throws<DomainException>(() => rule.SetPrecondition(MakePrecondition()));
        Assert.Contains("deactivated", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AR025_NonOwner_CannotEditRule()
    {
        var home = CreateHome();
        var repo = new InMemoryAutomationRuleRepository();
        var service = new AutomationService(repo);
        var rule = await service.CreateRuleAsync(new AutomationRuleName("Rule1"), home, OwnerId);
        await repo.SaveAsync(rule);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.SetPreconditionAsync(rule.Id, MakePrecondition(), home, OtherId));
    }

    [Fact]
    public void AR026_EditingRuleWithInvalidAction_ThrowsDomainException()
    {
        var rule = CreateRule();
        rule.SetPrecondition(MakePrecondition());
        var ex = Assert.Throws<DomainException>(() =>
            rule.SetActionSequence(new ActionSequence(new DeviceType("Thermostat"),
                [(Guid.NewGuid(), "activateLaserCannon")])));
        Assert.Contains("not allowed", ex.Message);
    }

    [Fact]
    public void AR027_RuleCanDependOnAnotherRule()
    {
        var ruleA = CreateRule();
        var ruleB = CreateRule();
        ruleB.AddDependency(ruleA.Id);
        Assert.True(ruleB.DependsOn(ruleA.Id));
    }

    [Fact]
    public void AR028_ConflictBetweenRules_CanBeDefined()
    {
        var ruleA = CreateRule();
        var ruleB = CreateRule();
        ruleA.AddConflict(ruleB.Id);
        Assert.True(ruleA.HasConflictWith(ruleB.Id));
    }

    [Fact]
    public void AR029_RuleDependingOnInactiveRule_IsHandled()
    {
        var ruleA = CreateRule(); // dependency - deactivated
        var ruleB = CreateRule();
        ruleB.SetPrecondition(MakePrecondition());
        ruleB.SetActionSequence(MakeAction());
        ruleB.AddDependency(ruleA.Id);
        ruleB.Enable();

        Assert.True(ruleB.DependsOn(ruleA.Id));
        Assert.False(ruleA.IsEnabled);
        Assert.True(ruleB.IsEnabled);
    }

    [Fact]
    public void AR030_CircularDependency_ThrowsDomainException()
    {
        var ruleA = CreateRule();
        var ruleB = CreateRule();
        var allRules = new[] { ruleA, ruleB };

        ruleA.AddDependency(ruleB.Id, allRules);

        var ex = Assert.Throws<DomainException>(() =>
            ruleB.AddDependency(ruleA.Id, allRules));
        Assert.Contains("circular", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AR031_ComplexRuleHierarchy_IsSupported()
    {
        var ruleA = CreateRule();
        var ruleB = CreateRule();
        var ruleC = CreateRule();
        var allRules = new[] { ruleA, ruleB, ruleC };

        ruleB.AddDependency(ruleA.Id, allRules);
        ruleC.AddDependency(ruleB.Id, allRules);

        Assert.True(ruleB.DependsOn(ruleA.Id));
        Assert.True(ruleC.DependsOn(ruleB.Id));
    }

    [Fact]
    public void AR032_ActiveTriggeredRule_RecordedWithTimestamp()
    {
        var rule = CreateRule();
        rule.SetPrecondition(MakePrecondition());
        rule.SetActionSequence(MakeAction());
        rule.Enable();
        rule.MarkAsTriggered();
        Assert.NotNull(rule.LastTriggeredAt);
    }

    [Fact]
    public void AR033_InactiveRule_NotTriggeredEvenIfPreconditionMet()
    {
        var deviceId = Guid.NewGuid();
        var rule = CreateRule();
        rule.SetPrecondition(new AtomicCondition(deviceId, AtomicConditionKind.SensorReadingValue, RelationalOperator.LessThan, 18));
        rule.SetActionSequence(MakeAction());

        // Rule is NOT enabled
        var ctx = new EvaluationContext();
        ctx.SetReading(deviceId, new SensorReading(10, "°C", DateTime.UtcNow));

        Assert.False(rule.CanExecute(ctx));
    }

    [Fact]
    public void AR034_ActionsExecutedInOrder_AfterRuleTriggered()
    {
        var deviceId1 = Guid.NewGuid();
        var deviceId2 = Guid.NewGuid();
        var action = new ActionSequence(
        [
            (deviceId1, new DeviceType("DoorLock"), "lockDoor"),
            (deviceId2, new DeviceType("Thermostat"), "turnOnHeating"),
        ]);
        Assert.Equal("lockDoor", action.Steps[0].CommandName);
        Assert.Equal("turnOnHeating", action.Steps[1].CommandName);
    }

    [Fact]
    public void AR035_RepeatedTriggers_RecordedSeparately()
    {
        var rule = CreateRule();
        rule.SetPrecondition(MakePrecondition());
        rule.SetActionSequence(MakeAction());
        rule.Enable();

        rule.MarkAsTriggered();
        var first = rule.LastTriggeredAt;
        Thread.Sleep(10);
        rule.MarkAsTriggered();
        var second = rule.LastTriggeredAt;

        Assert.NotEqual(first, second);
        Assert.Equal(2, rule.DomainEvents.Count);
    }
}

// In-memory repository for testing
internal class InMemoryAutomationRuleRepository : Automation.Repositories.IAutomationRuleRepository
{
    private readonly Dictionary<Guid, AutomationRuleAggregate> _store = [];

    public Task<AutomationRuleAggregate?> GetByIdAsync(Guid id)
        => Task.FromResult(_store.TryGetValue(id, out var r) ? r : null);

    public Task<List<AutomationRuleAggregate>> GetByHomeIdAsync(Guid homeId)
        => Task.FromResult(_store.Values.Where(r => r.HomeId == homeId).ToList());

    public Task SaveAsync(AutomationRuleAggregate rule)
    {
        _store[rule.Id] = rule;
        return Task.CompletedTask;
    }
}
