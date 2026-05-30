using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public sealed class PaymentType : ValueObject
{
    public static readonly PaymentType PrePaid = new("PrePaid", skipValidation: true);
    public static readonly PaymentType PayAtHotel = new("PayAtHotel", skipValidation: true);

    private static readonly HashSet<string> _valid = ["PrePaid", "PayAtHotel"];

    public string Value { get; }

    public PaymentType(string value) : this(value, skipValidation: false) { }

    private PaymentType(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Payment type cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Payment type '{value}' is not valid.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
