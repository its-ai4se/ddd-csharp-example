using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Device;

/// <summary>
/// Enforces unique device IDs across all devices (BR-001).
/// </summary>
public class DeviceRegistry
{
    private readonly HashSet<Guid> _registeredIds = [];

    public void Register(DeviceAggregate device)
    {
        if (!_registeredIds.Add(device.Id))
            throw new DomainException($"Device with ID '{device.Id}' is already registered. Device IDs must be unique.");
    }

    public bool IsRegistered(Guid deviceId) => _registeredIds.Contains(deviceId);
}
