using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

// BR-020: paddle length starts at maximum and reduces to minimum across levels
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
        => new(length.Value - reduction);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("F2");
}
