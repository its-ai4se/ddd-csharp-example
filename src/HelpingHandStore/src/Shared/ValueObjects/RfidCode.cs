using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class RfidCode : ValueObject
{
    public string Code { get; }

    public RfidCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("RFID code cannot be empty or whitespace.", nameof(code));
        }

        if (code.Length != 16)
        {
            throw new ArgumentException("RFID code must be exactly 16 characters.", nameof(code));
        }

        if (!code.All(c => char.IsLetterOrDigit(c)))
        {
            throw new ArgumentException("RFID code must contain only alphanumeric characters.", nameof(code));
        }

        Code = code.ToUpperInvariant();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
