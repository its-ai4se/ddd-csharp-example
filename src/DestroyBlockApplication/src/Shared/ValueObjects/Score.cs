using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

// BR-010: each block has a point value; BR-026: player earns points equal to the value of the hit block
public class Score : ValueObject
{
    public int Value { get; }

    public Score(int value)
    {
        if (value < 0)
            throw new ArgumentException("Score cannot be negative.", nameof(value));
        Value = value;
    }

    public static Score Zero => new(0);

    public static Score operator +(Score left, Score right) => new Score(left.Value + right.Value);

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value.ToString();
}
