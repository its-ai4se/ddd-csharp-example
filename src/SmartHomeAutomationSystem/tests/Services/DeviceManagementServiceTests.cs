using Xunit;
using SmartHomeAutomationSystem.Domain.Shared.Services;
using SmartHomeAutomationSystem.Domain.Services;
using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Device.Repositories;
using SmartHomeAutomationSystem.Domain.Room;
using SmartHomeAutomationSystem.Domain.Room.Repositories;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Tests.Services;

public class DeviceManagementServiceTests
{
    private readonly MockDeviceRepository _deviceRepository;
    private readonly MockRoomRepository _roomRepository;
    private readonly DeviceManagementService _service;

    public DeviceManagementServiceTests()
    {
        _deviceRepository = new MockDeviceRepository();
        _roomRepository = new MockRoomRepository();
        var clock = new SystemClock();
        _service = new DeviceManagementService(clock, _deviceRepository, _roomRepository);
    }

    [Fact]
    public async Task CreateDeviceAsync_WithValidData_ShouldCreateDevice()
    {
        // Arrange
        var homeId = Guid.NewGuid();
        var room = new RoomAggregate(new RoomName("Test Room"), homeId);
        await _roomRepository.SaveAsync(room);
        
        var deviceName = new DeviceName("Test Device");
        var deviceType = new DeviceType("Light");

        // Act
        var device = await _service.CreateDeviceAsync(deviceName, deviceType, room.Id);

        // Assert
        Assert.Equal(deviceName.Value, device.Name.Value);
        Assert.Equal(deviceType.Value, device.Type.Value);
        Assert.Equal(room.Id, device.RoomId);
        
        // Verify the room was updated with the device
        var updatedRoom = await _roomRepository.GetByIdAsync(room.Id);
        Assert.Contains(device.Id, updatedRoom!.DeviceIds);
    }

    [Fact]
    public async Task CreateDeviceAsync_WithNonExistentRoom_ShouldThrowDomainException()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var deviceName = new DeviceName("Test Device");
        var deviceType = new DeviceType("Light");

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _service.CreateDeviceAsync(deviceName, deviceType, roomId));
    }

    [Fact]
    public async Task MoveDeviceToRoomAsync_WithValidData_ShouldMoveDevice()
    {
        // Arrange
        var homeId = Guid.NewGuid();
        var oldRoom = new RoomAggregate(new RoomName("Old Room"), homeId);
        var newRoom = new RoomAggregate(new RoomName("New Room"), homeId);
        
        await _roomRepository.SaveAsync(oldRoom);
        await _roomRepository.SaveAsync(newRoom);
        
        var device = new DeviceAggregate(new DeviceName("Test Device"), new DeviceType("Light"), oldRoom.Id);
        await _deviceRepository.SaveAsync(device);
        oldRoom.AddDevice(device.Id);
        await _roomRepository.SaveAsync(oldRoom);

        // Act
        await _service.MoveDeviceToRoomAsync(device.Id, newRoom.Id);

        // Assert
        Assert.Equal(newRoom.Id, device.RoomId);
        
        // Verify the rooms were updated correctly
        var updatedOldRoom = await _roomRepository.GetByIdAsync(oldRoom.Id);
        var updatedNewRoom = await _roomRepository.GetByIdAsync(newRoom.Id);
        
        Assert.DoesNotContain(device.Id, updatedOldRoom!.DeviceIds);
        Assert.Contains(device.Id, updatedNewRoom!.DeviceIds);
    }

    [Fact]
    public async Task UpdateDeviceStatusAsync_WithValidData_ShouldUpdateStatus()
    {
        // Arrange
        var homeId = Guid.NewGuid();
        var room = new RoomAggregate(new RoomName("Test Room"), homeId);
        var device = new DeviceAggregate(new DeviceName("Test Device"), new DeviceType("Light"), room.Id);
        await _deviceRepository.SaveAsync(device);
        
        var newStatus = new DeviceStatus("Online");

        // Act
        await _service.UpdateDeviceStatusAsync(device.Id, newStatus);

        // Assert
        Assert.Equal(newStatus.Value, device.Status.Value);
    }

    [Fact]
    public async Task UpdateDeviceStatusAsync_WithNonExistentDevice_ShouldThrowDomainException()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var status = new DeviceStatus("Online");

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _service.UpdateDeviceStatusAsync(deviceId, status));
    }

    [Fact]
    public async Task MockRepository_ShouldWorkCorrectly()
    {
        // Arrange
        var homeId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var room = new RoomAggregate(new RoomName("Test Room"), homeId);

        // Act
        await _roomRepository.SaveAsync(room);
        var retrievedRoom = await _roomRepository.GetByIdAsync(room.Id);

        // Assert
        Assert.NotNull(retrievedRoom);
        Assert.Equal(room.Id, retrievedRoom.Id);
        Assert.Equal("Test Room", retrievedRoom.Name.Value);
    }
}

// Mock implementations for testing
public class MockDeviceRepository : IDeviceRepository
{
    private readonly Dictionary<Guid, DeviceAggregate> _devices = new();

    public Task<DeviceAggregate?> GetByIdAsync(Guid id)
    {
        _devices.TryGetValue(id, out var device);
        return Task.FromResult(device);
    }

    public Task<List<DeviceAggregate>> GetByRoomIdAsync(Guid roomId)
    {
        var devices = _devices.Values.Where(d => d.RoomId == roomId).ToList();
        return Task.FromResult(devices);
    }

    public Task<List<DeviceAggregate>> GetByTypeAsync(string deviceType)
    {
        var devices = _devices.Values.Where(d => d.Type.Value == deviceType).ToList();
        return Task.FromResult(devices);
    }

    public Task<List<DeviceAggregate>> GetAllAsync()
    {
        return Task.FromResult(_devices.Values.ToList());
    }

    public Task SaveAsync(DeviceAggregate device)
    {
        _devices[device.Id] = device;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _devices.Remove(id);
        return Task.CompletedTask;
    }
}

public class MockRoomRepository : IRoomRepository
{
    private readonly Dictionary<Guid, RoomAggregate> _rooms = new();

    public Task<RoomAggregate?> GetByIdAsync(Guid id)
    {
        _rooms.TryGetValue(id, out var room);
        return Task.FromResult(room);
    }

    public Task<List<RoomAggregate>> GetByHomeIdAsync(Guid homeId)
    {
        var rooms = _rooms.Values.Where(r => r.HomeId == homeId).ToList();
        return Task.FromResult(rooms);
    }

    public Task<List<RoomAggregate>> GetAllAsync()
    {
        return Task.FromResult(_rooms.Values.ToList());
    }

    public Task SaveAsync(RoomAggregate room)
    {
        _rooms[room.Id] = room;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _rooms.Remove(id);
        return Task.CompletedTask;
    }
}