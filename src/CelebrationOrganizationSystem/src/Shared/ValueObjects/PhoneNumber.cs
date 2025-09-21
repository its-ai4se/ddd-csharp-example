using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Phone number cannot be empty or whitespace.", nameof(value));
        }

        var cleanedValue = CleanPhoneNumber(value);
        if (!IsValidPhoneNumber(cleanedValue))
        {
            throw new ArgumentException("Invalid phone number format.", nameof(value));
        }

        Value = cleanedValue;
    }

    private static string CleanPhoneNumber(string phoneNumber)
    {
        return phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+", "");
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Basic validation - at least 10 digits
        return phoneNumber.All(char.IsDigit) && phoneNumber.Length >= 10;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
