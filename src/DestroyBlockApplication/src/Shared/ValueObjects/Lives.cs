using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public class Lives : ValueObject
{
    public int Value { get; }

    public Lives(int value)
    {
        if (value < 0)
            throw new ArgumentException("Lives cannot be negative.", nameof(value));

        Value = value;
    }

    public static Lives operator --(Lives lives) => new Lives(lives.Value - 1);

    public bool IsAlive => Value > 0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
