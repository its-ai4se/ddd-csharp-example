using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class EmailAddress : ValueObject
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email address cannot be empty or whitespace.", nameof(value));
        }

        Value = value.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
