using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace SmartHomeAutomationSystem.Domain.Tests;

public class ActuatorOperationTests
{
    private static DeviceAggregate CreateActiveActuator(string type = "Thermostat")
    {
        var actuator = new DeviceAggregate(new DeviceName("Heater"), new DeviceType(type), Guid.NewGuid());
        actuator.Activate();
        return actuator;
    }

    private static DeviceAggregate CreateInactiveActuator(string type = "DoorLock")
        => new(new DeviceName("Lock"), new DeviceType(type), Guid.NewGuid());

    [Fact]
    public void AO001_ValidCommand_ToActiveActuator_Succeeds()
    {
        var actuator = CreateActiveActuator("Thermostat");
        var cmd = actuator.IssueCommand("turnOnHeating", DateTime.UtcNow);
        Assert.Equal("turnOnHeating", cmd.CommandName);
        Assert.Equal(CommandStatus.Requested, cmd.Status);
    }

    [Fact]
    public void AO002_NullTimestamp_ThrowsDomainException()
    {
        var actuator = CreateActiveActuator("DoorLock");
        var ex = Assert.Throws<DomainException>(() => actuator.IssueCommand("lockDoor", null));
        Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AO003_NullStatus_ThrowsDomainException()
    {
        var actuator = CreateActiveActuator("DoorLock");
        var ex = Assert.Throws<DomainException>(() => actuator.IssueCommand("lockDoor", DateTime.UtcNow, null));
        Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(CommandStatus.Requested)]
    [InlineData(CommandStatus.Completed)]
    [InlineData(CommandStatus.Failed)]
    public void AO004_AllValidStatuses_AreAccepted(CommandStatus status)
    {
        var actuator = CreateActiveActuator("Thermostat");
        var cmd = actuator.IssueCommand("turnOnHeating", DateTime.UtcNow, status);
        Assert.Equal(status, cmd.Status);
    }

    [Fact]
    public void AO005_UnknownCommand_ThrowsDomainException()
    {
        var actuator = CreateActiveActuator("Thermostat");
        var ex = Assert.Throws<DomainException>(() => actuator.IssueCommand("activateLaserCannon", DateTime.UtcNow));
        Assert.Contains("not allowed", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AO006_EmptyCommandName_ThrowsDomainException(string? commandName)
    {
        var actuator = CreateActiveActuator("Thermostat");
        var ex = Assert.Throws<DomainException>(() => actuator.IssueCommand(commandName!, DateTime.UtcNow));
        Assert.Contains("empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AO007_CommandToInactiveActuator_ThrowsDomainException()
    {
        var actuator = CreateInactiveActuator("DoorLock");
        var ex = Assert.Throws<DomainException>(() => actuator.IssueCommand("lockDoor", DateTime.UtcNow));
        Assert.Contains("not active", ex.Message);
    }
}
