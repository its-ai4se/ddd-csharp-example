using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Automation;

public class ActionSequence
{
    public IReadOnlyList<(Guid DeviceId, string CommandName)> Steps { get; }

    /// <summary>
    /// Creates an action sequence for a single device type.
    /// </summary>
    public ActionSequence(DeviceType targetDeviceType, IEnumerable<(Guid DeviceId, string CommandName)> steps)
    {
        ArgumentNullException.ThrowIfNull(targetDeviceType);
        var list = steps?.ToList() ?? throw new ArgumentNullException(nameof(steps));

        if (list.Count == 0)
            throw new DomainException("Action sequence must contain at least one command.");

        foreach (var (deviceId, commandName) in list)
        {
            if (deviceId == Guid.Empty)
                throw new DomainException("Device ID in action sequence cannot be empty.");
            if (string.IsNullOrWhiteSpace(commandName))
                throw new DomainException("Command name in action sequence cannot be empty.");
            if (!AllowedCommands.IsAllowed(targetDeviceType.Value, commandName))
                throw new DomainException(
                    $"Command '{commandName}' is not allowed for device type '{targetDeviceType.Value}'.");
        }

        Steps = list.AsReadOnly();
    }

    /// <summary>
    /// Creates an action sequence with per-step device type validation (multi-device-type support).
    /// </summary>
    public ActionSequence(IEnumerable<(Guid DeviceId, DeviceType DeviceType, string CommandName)> steps)
    {
        var list = steps?.ToList() ?? throw new ArgumentNullException(nameof(steps));

        if (list.Count == 0)
            throw new DomainException("Action sequence must contain at least one command.");

        var result = new List<(Guid, string)>();
        foreach (var (deviceId, deviceType, commandName) in list)
        {
            if (deviceId == Guid.Empty)
                throw new DomainException("Device ID in action sequence cannot be empty.");
            if (string.IsNullOrWhiteSpace(commandName))
                throw new DomainException("Command name in action sequence cannot be empty.");
            if (!AllowedCommands.IsAllowed(deviceType.Value, commandName))
                throw new DomainException(
                    $"Command '{commandName}' is not allowed for device type '{deviceType.Value}'.");
            result.Add((deviceId, commandName));
        }

        Steps = result.AsReadOnly();
    }
}
