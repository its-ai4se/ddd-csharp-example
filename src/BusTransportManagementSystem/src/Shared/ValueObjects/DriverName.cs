using BusTransportManagementSystem.Domain.Shared.Common;

namespace BusTransportManagementSystem.Domain.Shared.ValueObjects;

public class DriverName : ValueObject
{
    public string Value { get; }

    public DriverName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Driver name cannot be empty or whitespace.", nameof(value));
        }
        Value = value.Trim();
    }

    public static implicit operator string(DriverName driverName) => driverName.Value;
    public static explicit operator DriverName(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
