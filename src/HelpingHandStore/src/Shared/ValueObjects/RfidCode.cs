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

        Code = code.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
