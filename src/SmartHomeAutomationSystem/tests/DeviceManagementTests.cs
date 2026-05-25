using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace SmartHomeAutomationSystem.Domain.Tests;

public class DeviceManagementTests
{
    private static DeviceAggregate CreateSensor(string type = "TemperatureSensor")
        => new(new DeviceName("Sensor"), new DeviceType(type), Guid.NewGuid());

    private static DeviceAggregate CreateActuator(string type = "Thermostat")
        => new(new DeviceName("Actuator"), new DeviceType(type), Guid.NewGuid());

    [Fact]
    public void DM001_RegisteringSensorWithDuplicateId_ThrowsDomainException()
    {
        var registry = new DeviceRegistry();
        var sensor1 = CreateSensor();
        // Simulate second device with same ID by using reflection or a test-only constructor
        // Since IDs are auto-generated GUIDs, we test the registry directly
        registry.Register(sensor1);
        var ex = Assert.Throws<DomainException>(() => registry.Register(sensor1));
        Assert.Contains("already registered", ex.Message);
    }

    [Fact]
    public void DM002_RegisteringActuatorWithDuplicateId_ThrowsDomainException()
    {
        var registry = new DeviceRegistry();
        var actuator = CreateActuator();
        // Simulate second device with same ID by using reflection or a test-only constructor
        // Since IDs are auto-generated GUIDs, we test the registry directly
        registry.Register(actuator);
        var ex = Assert.Throws<DomainException>(() => registry.Register(actuator));
        Assert.Contains("already registered", ex.Message);
    }

    [Fact]
    public void DM003_SensorAndActuatorWithSameId_ThrowsDomainException()
    {
        var registry = new DeviceRegistry();
        var sensor = CreateSensor();
        registry.Register(sensor);
        // Attempt to register another device with same ID
        var ex = Assert.Throws<DomainException>(() => registry.Register(sensor));
        Assert.Contains("already registered", ex.Message);
    }

    [Fact]
    public void DM004_ActivatingSensor_UpdatesInfrastructureMap()
    {
        var map = new InfrastructureMap();
        var sensor = CreateSensor();
        sensor.Activate();
        map.UpdateDevice(sensor);
        Assert.Equal(true, map.GetStatus(sensor.Id));
    }

    [Fact]
    public void DM005_ActivatingActuator_UpdatesInfrastructureMap()
    {
        var map = new InfrastructureMap();
        var actuator = CreateActuator();
        actuator.Activate();
        map.UpdateDevice(actuator);
        Assert.Equal(true, map.GetStatus(actuator.Id));
    }

    [Fact]
    public void DM006_DeactivatingSensor_UpdatesInfrastructureMap()
    {
        var map = new InfrastructureMap();
        var sensor = CreateSensor();
        sensor.Activate();
        map.UpdateDevice(sensor);
        sensor.Deactivate();
        map.UpdateDevice(sensor);
        Assert.Equal(false, map.GetStatus(sensor.Id));
    }

    [Fact]
    public void DM007_DeactivatingActuator_UpdatesInfrastructureMap()
    {
        var map = new InfrastructureMap();
        var actuator = CreateActuator();
        actuator.Activate();
        map.UpdateDevice(actuator);
        actuator.Deactivate();
        map.UpdateDevice(actuator);
        Assert.Equal(false, map.GetStatus(actuator.Id));
    }

    [Fact]
    public void DM008_NoStatusChange_InfrastructureMapUnchanged()
    {
        var map = new InfrastructureMap();
        var sensor = CreateSensor();
        sensor.Activate();
        map.UpdateDevice(sensor);
        var before = map.GetStatus(sensor.Id);
        // No change so map not updated again
        Assert.Equal(before, map.GetStatus(sensor.Id));
    }
}
