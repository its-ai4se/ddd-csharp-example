using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public class Username : ValueObject
{
    public string Value { get; }

    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Username cannot be empty or whitespace.", nameof(value));
        }

        if (value.Length < 3 || value.Length > 50)
        {
            throw new ArgumentException("Username must be between 3 and 50 characters.", nameof(value));
        }

        if (!value.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'))
        {
            throw new ArgumentException("Username can only contain letters, digits, underscores, and hyphens.", nameof(value));
        }

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
