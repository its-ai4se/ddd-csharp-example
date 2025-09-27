using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public class Score : ValueObject
{
    public int Value { get; }

    public Score(int value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Score cannot be negative.", nameof(value));
        }

        if (value > 1000)
        {
            throw new ArgumentException("Score cannot exceed 1000.", nameof(value));
        }

        Value = value;
    }

    public static Score operator +(Score left, Score right)
    {
        return new Score(left.Value + right.Value);
    }

    public static Score operator -(Score left, Score right)
    {
        return new Score(Math.Max(0, left.Value - right.Value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
