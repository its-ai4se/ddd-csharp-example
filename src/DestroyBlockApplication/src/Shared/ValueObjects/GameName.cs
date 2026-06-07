using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

// BR-008: each game must have a unique name
public class GameName : ValueObject
{
    public string Value { get; }

    public GameName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Game name cannot be empty or whitespace.", nameof(value));
        }

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
