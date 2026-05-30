using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public sealed class BookingStatus : ValueObject
{
    public static readonly BookingStatus Preliminary = new("Preliminary", skipValidation: true);
    public static readonly BookingStatus Finalized = new("Finalized", skipValidation: true);
    public static readonly BookingStatus Confirmed = new("Confirmed", skipValidation: true);
    public static readonly BookingStatus Cancelled = new("Cancelled", skipValidation: true);
    public static readonly BookingStatus Expired = new("Expired", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Preliminary", "Finalized", "Confirmed", "Cancelled", "Expired"];

    public string Value { get; }

    public BookingStatus(string value) : this(value, skipValidation: false) { }

    private BookingStatus(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Booking status cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Booking status '{value}' is not valid.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
