namespace SmartHomeAutomationSystem.Domain.Device.Repositories;

public interface IDeviceRepository
{
    Task<DeviceAggregate?> GetByIdAsync(Guid id);
    Task SaveAsync(DeviceAggregate device);
}
