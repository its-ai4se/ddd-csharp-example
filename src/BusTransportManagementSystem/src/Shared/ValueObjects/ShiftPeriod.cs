using BusTransportManagementSystem.Domain.Shared.Common;

namespace BusTransportManagementSystem.Domain.Shared.ValueObjects;

public enum ShiftPeriodType
{
    Morning,
    Afternoon,
    Night
}

public class ShiftPeriod : ValueObject
{
    public ShiftPeriodType Value { get; }

    public ShiftPeriod(ShiftPeriodType value)
    {
        Value = value;
    }

    public ShiftPeriod(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Shift period cannot be empty or whitespace.", nameof(value));
        }

        if (!Enum.TryParse<ShiftPeriodType>(value.Trim(), true, out var enumValue))
        {
            throw new ArgumentException($"Invalid shift period. Valid values are: {string.Join(", ", Enum.GetNames<ShiftPeriodType>())}", nameof(value));
        }

        Value = enumValue;
    }

    public static implicit operator string(ShiftPeriod shiftPeriod) => shiftPeriod.Value.ToString();
    public static explicit operator ShiftPeriod(string value) => new(value);
    public static explicit operator ShiftPeriod(ShiftPeriodType value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
