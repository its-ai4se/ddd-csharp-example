using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public class LevelNumber : ValueObject
{
    public int Value { get; }

    public LevelNumber(int value)
    {
        if (value < 1 || value > 99)
        {
            throw new ArgumentException("Level number must be between 1 and 99.", nameof(value));
        }

        Value = value;
    }

    public static LevelNumber operator ++(LevelNumber level)
    {
        return new LevelNumber(level.Value + 1);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
