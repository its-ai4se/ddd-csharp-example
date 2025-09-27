using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public class PaddleLength : ValueObject
{
    public double Value { get; }

    public PaddleLength(double value)
    {
        if (value <= 0)
        {
            throw new ArgumentException("Paddle length must be positive.", nameof(value));
        }

        Value = value;
    }

    public static PaddleLength operator -(PaddleLength length, double reduction)
    {
        return new PaddleLength(Math.Max(0.1, length.Value - reduction));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("F2");
}
