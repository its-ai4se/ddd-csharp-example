using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; private set; }

    public PhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Phone number cannot be empty.");

        var cleaned = CleanPhoneNumber(phoneNumber);
        
        if (!IsValidPhoneNumber(cleaned))
            throw new DomainException("Invalid phone number format.");

        Value = cleaned;
    }

    private static string CleanPhoneNumber(string phoneNumber)
    {
        return new string(phoneNumber.Where(char.IsDigit).ToArray());
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        return phoneNumber.Length >= 10 && phoneNumber.Length <= 15;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
