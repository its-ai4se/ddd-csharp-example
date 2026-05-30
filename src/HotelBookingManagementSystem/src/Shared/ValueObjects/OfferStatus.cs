using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public sealed class OfferStatus : ValueObject
{
    public static readonly OfferStatus Pending = new("Pending", skipValidation: true);
    public static readonly OfferStatus Accepted = new("Accepted", skipValidation: true);
    public static readonly OfferStatus Expired = new("Expired", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Pending", "Accepted", "Expired"];

    public string Value { get; }

    public OfferStatus(string value) : this(value, skipValidation: false) { }

    private OfferStatus(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Offer status cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Offer status '{value}' is not valid.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
