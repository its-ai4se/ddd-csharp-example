using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class CreditCardInfo : ValueObject
{
    public string CardNumber { get; }  // masked
    public string CardHolderName { get; }
    public DateTime ExpiryDate { get; }

    public CreditCardInfo(string cardNumber, string cardHolderName, DateTime expiryDate, string cvv)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card number cannot be empty.", nameof(cardNumber));
        if (string.IsNullOrWhiteSpace(cardHolderName))
            throw new ArgumentException("Card holder name cannot be empty.", nameof(cardHolderName));
        if (string.IsNullOrWhiteSpace(cvv))
            throw new ArgumentException("CVV cannot be empty.", nameof(cvv));
        if (expiryDate < DateTime.Now.Date)
            throw new ArgumentException("Credit card has expired.", nameof(expiryDate));
        if (!IsValidCardNumber(cardNumber))
            throw new ArgumentException("Invalid card number format.", nameof(cardNumber));
        if (!IsValidCvv(cvv))
            throw new ArgumentException("Invalid CVV format.", nameof(cvv));

        CardNumber = MaskCardNumber(cardNumber);
        CardHolderName = cardHolderName.Trim();
        ExpiryDate = expiryDate.Date;
        // CVV is validated but not stored (security)
    }

    private static bool IsValidCardNumber(string cardNumber)
    {
        var cleaned = cardNumber.Replace(" ", "").Replace("-", "");
        return cleaned.Length >= 13 && cleaned.Length <= 19 && cleaned.All(char.IsDigit);
    }

    private static bool IsValidCvv(string cvv) =>
        cvv.Length >= 3 && cvv.Length <= 4 && cvv.All(char.IsDigit);

    private static string MaskCardNumber(string cardNumber)
    {
        var cleaned = cardNumber.Replace(" ", "").Replace("-", "");
        if (cleaned.Length < 8) return cleaned;
        return cleaned[..4] + new string('*', cleaned.Length - 8) + cleaned[^4..];
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CardNumber;
        yield return CardHolderName;
        yield return ExpiryDate;
    }

    public override string ToString() => $"{CardHolderName} - {CardNumber}";
}
