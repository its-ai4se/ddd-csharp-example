using SmartHomeAutomationSystem.Domain.ActivityLog;
using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace SmartHomeAutomationSystem.Domain.Tests;

public class ActivityLogTests
{
    private static DeviceAggregate CreateActiveSensor()
    {
        var s = new DeviceAggregate(new DeviceName("Sensor"), new DeviceType("TemperatureSensor"), Guid.NewGuid());
        s.Activate();
        return s;
    }

    private static DeviceAggregate CreateActiveActuator()
    {
        var a = new DeviceAggregate(new DeviceName("Heater"), new DeviceType("Thermostat"), Guid.NewGuid());
        a.Activate();
        return a;
    }

    [Fact]
    public void AL001_SensorReading_IsRecordedInActivityLog()
    {
        var homeId = Guid.NewGuid();
        var log = new ActivityLogAggregate(homeId);
        var sensor = CreateActiveSensor();
        var reading = sensor.GenerateReading(22.5, "°C", DateTime.UtcNow);

        log.RecordSensorReading(sensor.Id, reading);

        Assert.Single(log.Entries);
        Assert.Equal(sensor.Id, log.Entries[0].DeviceId);
        Assert.Equal(ActivityEntryType.SensorReading, log.Entries[0].EntryType);
    }

    [Fact]
    public void AL002_MultipleActiveSensors_AllReadingsRecorded()
    {
        var homeId = Guid.NewGuid();
        var log = new ActivityLogAggregate(homeId);
        var sensors = Enumerable.Range(0, 3).Select(_ => CreateActiveSensor()).ToList();

        foreach (var sensor in sensors)
        {
            var reading = sensor.GenerateReading(20.0, "°C", DateTime.UtcNow);
            log.RecordSensorReading(sensor.Id, reading);
        }

        Assert.Equal(3, log.Entries.Count);
    }

    [Fact]
    public void AL003_ControlCommand_IsRecordedInActivityLog()
    {
        var homeId = Guid.NewGuid();
        var log = new ActivityLogAggregate(homeId);
        var actuator = CreateActiveActuator();
        var cmd = actuator.IssueCommand("turnOnHeating", DateTime.UtcNow);

        log.RecordControlCommand(actuator.Id, cmd);

        Assert.Single(log.Entries);
        Assert.Equal(actuator.Id, log.Entries[0].DeviceId);
        Assert.Equal(ActivityEntryType.ControlCommand, log.Entries[0].EntryType);
    }

    [Fact]
    public void AL004_CommandStatusChange_RecordedAsNewEntry()
    {
        var homeId = Guid.NewGuid();
        var log = new ActivityLogAggregate(homeId);
        var actuator = CreateActiveActuator();
        var cmd1 = actuator.IssueCommand("turnOnHeating", DateTime.UtcNow, CommandStatus.Requested);
        var cmd2 = new ControlCommand("turnOnHeating", DateTime.UtcNow, CommandStatus.Completed);

        log.RecordControlCommand(actuator.Id, cmd1);
        log.RecordControlCommand(actuator.Id, cmd2);

        Assert.Equal(2, log.Entries.Count);
    }

    [Fact]
    public void AL005_ActivityLog_CannotBeModifiedOrDeleted()
    {
        var homeId = Guid.NewGuid();
        var log = new ActivityLogAggregate(homeId);
        var sensor = CreateActiveSensor();
        var reading = sensor.GenerateReading(22.5, "°C", DateTime.UtcNow);
        log.RecordSensorReading(sensor.Id, reading);

        // Entries is IReadOnlyList - cannot add/remove
        Assert.IsAssignableFrom<IReadOnlyList<ActivityLogEntry>>(log.Entries);

        // Verify no Delete/Modify methods exist on ActivityLogAggregate
        var methods = typeof(ActivityLogAggregate).GetMethods()
            .Select(m => m.Name.ToLower()).ToList();
        
        Assert.DoesNotContain("delete", methods);
        Assert.DoesNotContain("remove", methods);
        Assert.DoesNotContain("modify", methods);
        Assert.DoesNotContain("update", methods);
        Assert.DoesNotContain("clear", methods);
    }

    [Fact]
    public void AL006_ActivityLog_FilteredByHomeId()
    {
        var homeId1 = Guid.NewGuid();
        var homeId2 = Guid.NewGuid();
        var log1 = new ActivityLogAggregate(homeId1);
        var log2 = new ActivityLogAggregate(homeId2);
        var sensor = CreateActiveSensor();
        var reading = sensor.GenerateReading(22.5, "°C", DateTime.UtcNow);

        log1.RecordSensorReading(sensor.Id, reading);

        Assert.Equal(homeId1, log1.HomeId);
        Assert.Equal(homeId2, log2.HomeId);
        Assert.Single(log1.Entries);
        Assert.Empty(log2.Entries);
    }
}
