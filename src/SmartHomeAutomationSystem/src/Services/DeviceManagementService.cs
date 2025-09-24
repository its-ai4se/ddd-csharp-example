using SmartHomeAutomationSystem.Domain.Shared.Services;
using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Device.Repositories;
using SmartHomeAutomationSystem.Domain.Room;
using SmartHomeAutomationSystem.Domain.Room.Repositories;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Services;

public class DeviceManagementService : DomainServiceBase
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IRoomRepository _roomRepository;

    public DeviceManagementService(
        IClock clock,
        IDeviceRepository deviceRepository,
        IRoomRepository roomRepository) : base(clock)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    }

    public async Task<DeviceAggregate> CreateDeviceAsync(
        DeviceName name, 
        DeviceType type, 
        Guid roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
            throw new DomainException("Room not found.");

        var device = new DeviceAggregate(name, type, roomId);
        await _deviceRepository.SaveAsync(device);
        
        room.AddDevice(device.Id);
        await _roomRepository.SaveAsync(room);
        
        return device;
    }

    public async Task MoveDeviceToRoomAsync(Guid deviceId, Guid newRoomId)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId);
        if (device == null)
            throw new DomainException("Device not found.");

        var newRoom = await _roomRepository.GetByIdAsync(newRoomId);
        if (newRoom == null)
            throw new DomainException("New room not found.");

        var oldRoom = await _roomRepository.GetByIdAsync(device.RoomId);
        if (oldRoom != null)
        {
            oldRoom.RemoveDevice(deviceId);
            await _roomRepository.SaveAsync(oldRoom);
        }

        device.MoveToRoom(newRoomId);
        await _deviceRepository.SaveAsync(device);

        newRoom.AddDevice(deviceId);
        await _roomRepository.SaveAsync(newRoom);
    }

    public async Task UpdateDeviceStatusAsync(Guid deviceId, DeviceStatus status)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId);
        if (device == null)
            throw new DomainException("Device not found.");

        device.UpdateStatus(status);
        await _deviceRepository.SaveAsync(device);
    }

    public async Task<List<DeviceAggregate>> GetDevicesByRoomAsync(Guid roomId)
    {
        return await _deviceRepository.GetByRoomIdAsync(roomId);
    }

    public async Task<List<DeviceAggregate>> GetDevicesByTypeAsync(string deviceType)
    {
        return await _deviceRepository.GetByTypeAsync(deviceType);
    }
}
