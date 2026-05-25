namespace SmartHomeAutomationSystem.Domain.Device;

/// <summary>
/// Tracks active/inactive status of devices (BR-002).
/// </summary>
public class InfrastructureMap
{
    private readonly Dictionary<Guid, bool> _deviceStatus = [];

    public void UpdateDevice(DeviceAggregate device)
    {
        _deviceStatus[device.Id] = device.IsActive;
    }

    public bool? GetStatus(Guid deviceId)
        => _deviceStatus.TryGetValue(deviceId, out var active) ? active : null;

    public IReadOnlyDictionary<Guid, bool> GetAll() => _deviceStatus;
}
