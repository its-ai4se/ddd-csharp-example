using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Device.Repositories;
using SmartHomeAutomationSystem.Domain.ActivityLog;
using SmartHomeAutomationSystem.Domain.ActivityLog.Repositories;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Services;

public class DeviceManagementService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IActivityLogRepository _activityLogRepository;

    public DeviceManagementService(
        IDeviceRepository deviceRepository,
        IActivityLogRepository activityLogRepository)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _activityLogRepository = activityLogRepository ?? throw new ArgumentNullException(nameof(activityLogRepository));
    }

    public async Task<SensorReading> GenerateAndRecordReadingAsync(Guid deviceId, Guid homeId, double value, string unit)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId)
            ?? throw new DomainException("Device not found.");
        var reading = device.GenerateReading((double?)value, unit, (DateTime?)DateTime.UtcNow);
        var log = await GetOrCreateLogAsync(homeId);
        log.RecordSensorReading(deviceId, reading);
        await _activityLogRepository.SaveAsync(log);
        return reading;
    }

    public async Task<ControlCommand> IssueAndRecordCommandAsync(Guid deviceId, Guid homeId, string commandName)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId)
            ?? throw new DomainException("Device not found.");
        var command = device.IssueCommand(commandName, (DateTime?)DateTime.UtcNow);
        var log = await GetOrCreateLogAsync(homeId);
        log.RecordControlCommand(deviceId, command);
        await _activityLogRepository.SaveAsync(log);
        return command;
    }

    public async Task ActivateDeviceAsync(Guid deviceId)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId)
            ?? throw new DomainException("Device not found.");
        device.Activate();
        await _deviceRepository.SaveAsync(device);
    }

    public async Task DeactivateDeviceAsync(Guid deviceId)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId)
            ?? throw new DomainException("Device not found.");
        device.Deactivate();
        await _deviceRepository.SaveAsync(device);
    }

    private async Task<ActivityLogAggregate> GetOrCreateLogAsync(Guid homeId)
    {
        var log = await _activityLogRepository.GetByHomeIdAsync(homeId);
        return log ?? new ActivityLogAggregate(homeId);
    }
}
