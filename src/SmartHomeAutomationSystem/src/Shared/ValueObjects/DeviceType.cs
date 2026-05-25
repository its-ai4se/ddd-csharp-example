using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

public class DeviceType : ValueObject
{
    public string Value { get; }
    public DeviceKind Kind { get; }

    private static readonly Dictionary<string, DeviceKind> ValidTypes = new()
    {
        // Sensors
        { "MotionSensor",   DeviceKind.Sensor },
        { "SmokeDetector",  DeviceKind.Sensor },
        { "WindowSensor",   DeviceKind.Sensor },
        { "TemperatureSensor", DeviceKind.Sensor },
        { "HumiditySensor", DeviceKind.Sensor },
        // Actuators
        { "Light",          DeviceKind.Actuator },
        { "DoorLock",       DeviceKind.Actuator },
        { "Thermostat",     DeviceKind.Actuator },
        { "SmartPlug",      DeviceKind.Actuator },
        { "Blinds",         DeviceKind.Actuator },
    };

    public DeviceType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Device type cannot be empty.");
        if (!ValidTypes.TryGetValue(value, out var kind))
            throw new DomainException($"Invalid device type: {value}. Valid types: {string.Join(", ", ValidTypes.Keys)}");
        Value = value;
        Kind = kind;
    }

    public bool IsSensor => Kind == DeviceKind.Sensor;
    public bool IsActuator => Kind == DeviceKind.Actuator;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(DeviceType dt) => dt.Value;
    public static implicit operator DeviceType(string value) => new(value);
}

public enum DeviceKind { Sensor, Actuator }
