using Xunit;
using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Tests.Aggregates;

public class DeviceAggregateTests
{
    [Fact]
    public void DeviceAggregate_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var deviceName = new DeviceName("Smart Light");
        var deviceType = new DeviceType("Light");

        // Act
        var device = new DeviceAggregate(deviceName, deviceType, roomId);

        // Assert
        Assert.Equal(deviceName.Value, device.Name.Value);
        Assert.Equal(deviceType.Value, device.Type.Value);
        Assert.Equal(roomId, device.RoomId);
        Assert.Equal("Offline", device.Status.Value);
        Assert.NotEqual(Guid.Empty, device.Id);
    }

    [Fact]
    public void DeviceAggregate_WithEmptyRoomId_ShouldThrowDomainException()
    {
        // Arrange
        var deviceName = new DeviceName("Smart Light");
        var deviceType = new DeviceType("Light");

        // Act & Assert
        Assert.Throws<DomainException>(() => new DeviceAggregate(deviceName, deviceType, Guid.Empty));
    }

    [Fact]
    public void TurnOn_WhenDeviceIsOffline_ShouldThrowDomainException()
    {
        // Arrange
        var device = CreateTestDevice();

        // Act & Assert
        Assert.Throws<DomainException>(() => device.TurnOn());
    }

    [Fact]
    public void TurnOn_WhenDeviceIsOnline_ShouldTurnOnSuccessfully()
    {
        // Arrange
        var device = CreateTestDevice();
        device.UpdateStatus(new DeviceStatus("Online"));

        // Act
        device.TurnOn();

        // Assert
        Assert.True(device.IsOn());
        Assert.Equal("Online", device.Status.Value);
    }

    [Fact]
    public void TurnOff_ShouldTurnOffSuccessfully()
    {
        // Arrange
        var device = CreateTestDevice();
        device.UpdateStatus(new DeviceStatus("Online"));
        device.TurnOn();

        // Act
        device.TurnOff();

        // Assert
        Assert.False(device.IsOn());
        Assert.Equal("Online", device.Status.Value);
    }

    [Fact]
    public void UpdateSetting_WithValidKey_ShouldUpdateSetting()
    {
        // Arrange
        var device = CreateTestDevice();

        // Act
        device.UpdateSetting("brightness", 80);

        // Assert
        Assert.Equal(80, device.GetSetting<int>("brightness"));
    }

    [Fact]
    public void UpdateSetting_WithEmptyKey_ShouldThrowDomainException()
    {
        // Arrange
        var device = CreateTestDevice();

        // Act & Assert
        Assert.Throws<DomainException>(() => device.UpdateSetting("", 80));
    }

    [Fact]
    public void MoveToRoom_WithValidRoomId_ShouldMoveDevice()
    {
        // Arrange
        var device = CreateTestDevice();
        var newRoomId = Guid.NewGuid();

        // Act
        device.MoveToRoom(newRoomId);

        // Assert
        Assert.Equal(newRoomId, device.RoomId);
    }

    [Fact]
    public void MoveToRoom_WithEmptyRoomId_ShouldThrowDomainException()
    {
        // Arrange
        var device = CreateTestDevice();

        // Act & Assert
        Assert.Throws<DomainException>(() => device.MoveToRoom(Guid.Empty));
    }

    private static DeviceAggregate CreateTestDevice()
    {
        var roomId = Guid.NewGuid();
        var deviceName = new DeviceName("Test Device");
        var deviceType = new DeviceType("Light");
        return new DeviceAggregate(deviceName, deviceType, roomId);
    }
}
