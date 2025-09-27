using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public class Speed : ValueObject
{
    public double Value { get; }

    public Speed(double value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Speed cannot be negative.", nameof(value));
        }

        Value = value;
    }

    public static Speed operator *(Speed speed, double factor)
    {
        return new Speed(speed.Value * factor);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("F2");
}
