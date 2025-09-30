using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Phone number cannot be empty or whitespace.", nameof(value));
        }

        var cleaned = CleanPhoneNumber(value);
        if (!IsValidPhoneNumber(cleaned))
        {
            throw new ArgumentException("Invalid phone number format.", nameof(value));
        }

        Value = cleaned;
    }

    private static string CleanPhoneNumber(string phoneNumber)
    {
        return phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace(".", "");
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        // Basic validation for North American phone numbers
        return phoneNumber.Length >= 10 && phoneNumber.All(char.IsDigit);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
