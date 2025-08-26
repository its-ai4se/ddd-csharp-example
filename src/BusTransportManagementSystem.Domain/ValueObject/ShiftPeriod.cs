namespace BusTransportManagementSystem.Domain.ValueObject;

public enum ShiftPeriodType
{
    Morning,
    Afternoon,
    Night
}

public class ShiftPeriod : IEquatable<ShiftPeriod>
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

    public bool Equals(ShiftPeriod? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is ShiftPeriod other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool operator ==(ShiftPeriod left, ShiftPeriod right) => Equals(left, right);

    public static bool operator !=(ShiftPeriod left, ShiftPeriod right) => !Equals(left, right);
}
