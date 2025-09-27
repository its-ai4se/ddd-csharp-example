using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public class Color : ValueObject
{
    public string Value { get; }

    public Color(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Color cannot be empty or whitespace.", nameof(value));
        }

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
