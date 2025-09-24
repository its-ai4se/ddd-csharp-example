using SmartHomeAutomationSystem.Domain.Device;

namespace SmartHomeAutomationSystem.Domain.Device.Repositories;

public interface IDeviceRepository
{
    Task<DeviceAggregate?> GetByIdAsync(Guid id);
    Task<List<DeviceAggregate>> GetByRoomIdAsync(Guid roomId);
    Task<List<DeviceAggregate>> GetByTypeAsync(string deviceType);
    Task<List<DeviceAggregate>> GetAllAsync();
    Task SaveAsync(DeviceAggregate device);
    Task DeleteAsync(Guid id);
}
