using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

public class DeviceStatus : ValueObject
{
    public string Value { get; }

    private static readonly HashSet<string> ValidStatuses = new()
    {
        "Online", "Offline", "Error", "Maintenance"
    };

    public DeviceStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Device status cannot be empty.");
        
        if (!ValidStatuses.Contains(value))
            throw new DomainException($"Invalid device status: {value}. Valid statuses are: {string.Join(", ", ValidStatuses)}");
        
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(DeviceStatus deviceStatus) => deviceStatus.Value;
    public static implicit operator DeviceStatus(string value) => new(value);
}
