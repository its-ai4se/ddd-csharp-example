using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

public class DeviceType : ValueObject
{
    public string Value { get; }

    private static readonly HashSet<string> ValidTypes = new()
    {
        "Light", "Thermostat", "DoorLock", "SecurityCamera", "MotionSensor", 
        "SmokeDetector", "WindowSensor", "SmartPlug", "Speaker", "Blinds"
    };

    public DeviceType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Device type cannot be empty.");
        
        if (!ValidTypes.Contains(value))
            throw new DomainException($"Invalid device type: {value}. Valid types are: {string.Join(", ", ValidTypes)}");
        
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(DeviceType deviceType) => deviceType.Value;
    public static implicit operator DeviceType(string value) => new(value);
}
