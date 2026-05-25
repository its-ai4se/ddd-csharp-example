using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Home;
using SmartHomeAutomationSystem.Domain.Room;
using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace SmartHomeAutomationSystem.Domain.Tests;

public class SmartHomeTests
{
    [Fact]
    public void SH001_SmartHomeWithNullAddress_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => new HomeAggregate(null!, Guid.NewGuid()));
        Assert.Contains("address", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SH002_DuplicateAddress_ThrowsDomainException()
    {
        var registry = new HomeRegistry();
        var home1 = new HomeAggregate("123 Main St", Guid.NewGuid());
        var home2 = new HomeAggregate("123 Main St", Guid.NewGuid());
        registry.Register(home1);
        var ex = Assert.Throws<DomainException>(() => registry.Register(home2));
        Assert.Contains("already registered", ex.Message);
    }

    [Fact]
    public void SH003_RemovingLastRoom_ThrowsDomainException()
    {
        var home = new HomeAggregate("123 Main St", Guid.NewGuid());
        var roomId = Guid.NewGuid();
        home.AddRoom(roomId);
        var ex = Assert.Throws<DomainException>(() => home.RemoveRoom(roomId));
        Assert.Contains("at least one room", ex.Message);
    }

    [Fact]
    public void SH004_RoomWithNoDevices_CanBeRegistered()
    {
        var homeId = Guid.NewGuid();
        var room = new RoomAggregate(new RoomName("Living Room"), homeId);
        Assert.Empty(room.DeviceIds);
    }

    [Fact]
    public void SH005_RoomCanHaveMultipleSensors()
    {
        var homeId = Guid.NewGuid();
        var room = new RoomAggregate(new RoomName("Living Room"), homeId);
        var sensor1 = new DeviceAggregate(new DeviceName("TempSensor"), new DeviceType("TemperatureSensor"), room.Id);
        var sensor2 = new DeviceAggregate(new DeviceName("MotionSensor"), new DeviceType("MotionSensor"), room.Id);
        room.AddDevice(sensor1.Id);
        room.AddDevice(sensor2.Id);
        Assert.Equal(2, room.DeviceIds.Count);
    }

    [Fact]
    public void SH006_RoomCanHaveMultipleActuators()
    {
        var homeId = Guid.NewGuid();
        var room = new RoomAggregate(new RoomName("Living Room"), homeId);
        var light = new DeviceAggregate(new DeviceName("Light"), new DeviceType("Light"), room.Id);
        var lock_ = new DeviceAggregate(new DeviceName("Lock"), new DeviceType("DoorLock"), room.Id);
        room.AddDevice(light.Id);
        room.AddDevice(lock_.Id);
        Assert.Equal(2, room.DeviceIds.Count);
    }
}
