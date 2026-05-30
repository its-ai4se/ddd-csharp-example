using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.SpecialOffer;

public class SpecialOfferAggregate : AggregateRoot
{
    public Guid OriginalBookingId { get; private set; }
    public Guid CompetingHotelId { get; private set; }
    public Guid CompetingRoomId { get; private set; }
    public Money OfferedPrice { get; private set; }
    public DateRange StayPeriod { get; private set; }
    public int NumberOfRooms { get; private set; }
    public TravelPreferences OfferedAmenities { get; private set; }
    public OfferStatus Status { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    public SpecialOfferAggregate(
        Guid id,
        Guid originalBookingId,
        Guid competingHotelId,
        Guid competingRoomId,
        Money offeredPrice,
        DateRange stayPeriod,
        int numberOfRooms,
        TravelPreferences offeredAmenities) : base(id)
    {
        OriginalBookingId = originalBookingId != Guid.Empty ? originalBookingId : throw new ArgumentException("Original booking ID cannot be empty.", nameof(originalBookingId));
        CompetingHotelId = competingHotelId != Guid.Empty ? competingHotelId : throw new ArgumentException("Competing hotel ID cannot be empty.", nameof(competingHotelId));
        CompetingRoomId = competingRoomId != Guid.Empty ? competingRoomId : throw new ArgumentException("Competing room ID cannot be empty.", nameof(competingRoomId));
        OfferedPrice = offeredPrice ?? throw new ArgumentNullException(nameof(offeredPrice));
        StayPeriod = stayPeriod ?? throw new ArgumentNullException(nameof(stayPeriod));
        NumberOfRooms = numberOfRooms > 0 ? numberOfRooms : throw new ArgumentException("Number of rooms must be greater than 0.", nameof(numberOfRooms));
        OfferedAmenities = offeredAmenities ?? throw new ArgumentNullException(nameof(offeredAmenities));
        Status = OfferStatus.Pending;
        ExpiresAt = DateTime.UtcNow.AddHours(24);
    }

    public SpecialOfferAggregate(
        Guid originalBookingId,
        Guid competingHotelId,
        Guid competingRoomId,
        Money offeredPrice,
        DateRange stayPeriod,
        int numberOfRooms,
        TravelPreferences offeredAmenities) : base()
    {
        OriginalBookingId = originalBookingId != Guid.Empty ? originalBookingId : throw new ArgumentException("Original booking ID cannot be empty.", nameof(originalBookingId));
        CompetingHotelId = competingHotelId != Guid.Empty ? competingHotelId : throw new ArgumentException("Competing hotel ID cannot be empty.", nameof(competingHotelId));
        CompetingRoomId = competingRoomId != Guid.Empty ? competingRoomId : throw new ArgumentException("Competing room ID cannot be empty.", nameof(competingRoomId));
        OfferedPrice = offeredPrice ?? throw new ArgumentNullException(nameof(offeredPrice));
        StayPeriod = stayPeriod ?? throw new ArgumentNullException(nameof(stayPeriod));
        NumberOfRooms = numberOfRooms > 0 ? numberOfRooms : throw new ArgumentException("Number of rooms must be greater than 0.", nameof(numberOfRooms));
        OfferedAmenities = offeredAmenities ?? throw new ArgumentNullException(nameof(offeredAmenities));
        Status = OfferStatus.Pending;
        ExpiresAt = DateTime.UtcNow.AddHours(24);
    }

    public void AcceptOffer()
    {
        if (Status != OfferStatus.Pending)
            throw new InvalidOperationException("Only pending offers can be accepted.");
        if (IsExpired())
            throw new InvalidOperationException("Cannot accept an expired offer.");
        Status = OfferStatus.Accepted;
    }

    public void ExpireOffer()
    {
        if (Status != OfferStatus.Pending)
            throw new InvalidOperationException("Only pending offers can be expired.");
        Status = OfferStatus.Expired;
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    public bool IsPending() => Status == OfferStatus.Pending && !IsExpired();

    public Money CalculateSavings(Money originalPrice)
    {
        if (originalPrice.Currency != OfferedPrice.Currency)
            throw new InvalidOperationException("Cannot compare prices with different currencies.");
        var savings = originalPrice.Amount - OfferedPrice.Amount;
        return new Money(Math.Max(0, savings), originalPrice.Currency);
    }
}
