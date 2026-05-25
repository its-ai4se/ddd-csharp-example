using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Device;

public class SensorReading : ValueObject
{
    public double Value { get; }
    public string Unit { get; }
    public DateTime Timestamp { get; }

    public SensorReading(double value, string unit, DateTime timestamp)
    {
        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException("Sensor reading unit cannot be empty.");
        Value = value;
        Unit = unit.Trim();
        Timestamp = timestamp;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
        yield return Timestamp;
    }

    public override string ToString() => $"{Value} {Unit} at {Timestamp:O}";
}
