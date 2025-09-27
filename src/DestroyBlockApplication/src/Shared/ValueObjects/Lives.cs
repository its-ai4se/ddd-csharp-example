using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public class Lives : ValueObject
{
    public int Value { get; }

    public Lives(int value)
    {
        if (value < 0 || value > 3)
        {
            throw new ArgumentException("Lives must be between 0 and 3.", nameof(value));
        }

        Value = value;
    }

    public static Lives operator --(Lives lives)
    {
        return new Lives(Math.Max(0, lives.Value - 1));
    }

    public bool IsAlive => Value > 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
