using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

public class DeviceName : ValueObject
{
    public string Value { get; }

    public DeviceName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Device name cannot be empty.");
        
        if (value.Length > 100)
            throw new DomainException("Device name cannot exceed 100 characters.");
        
        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(DeviceName deviceName) => deviceName.Value;
    public static implicit operator DeviceName(string value) => new(value);
}
