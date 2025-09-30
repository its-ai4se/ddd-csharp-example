using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.SpecialOffer;

public enum OfferStatus
{
    Pending,
    Accepted,
    Rejected,
    Expired
}

public class SpecialOfferAggregate : AggregateRoot
{
    public Guid OriginalBookingId { get; private set; }
    public Guid CompetingHotelId { get; private set; }
    public Guid CompetingRoomId { get; private set; }
    public Money OfferedPrice { get; private set; }
    public DateRange StayPeriod { get; private set; }
    public int NumberOfRooms { get; private set; }
    public TravelPreferences OfferedAmenities { get; private set; }
    public string Description { get; private set; }
    public OfferStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }

    public SpecialOfferAggregate(
        Guid id,
        Guid originalBookingId,
        Guid competingHotelId,
        Guid competingRoomId,
        Money offeredPrice,
        DateRange stayPeriod,
        int numberOfRooms,
        TravelPreferences offeredAmenities,
        string description) : base(id)
    {
        OriginalBookingId = originalBookingId != Guid.Empty ? originalBookingId : throw new ArgumentException("Original booking ID cannot be empty.", nameof(originalBookingId));
        CompetingHotelId = competingHotelId != Guid.Empty ? competingHotelId : throw new ArgumentException("Competing hotel ID cannot be empty.", nameof(competingHotelId));
        CompetingRoomId = competingRoomId != Guid.Empty ? competingRoomId : throw new ArgumentException("Competing room ID cannot be empty.", nameof(competingRoomId));
        OfferedPrice = offeredPrice ?? throw new ArgumentNullException(nameof(offeredPrice));
        StayPeriod = stayPeriod ?? throw new ArgumentNullException(nameof(stayPeriod));
        NumberOfRooms = numberOfRooms > 0 ? numberOfRooms : throw new ArgumentException("Number of rooms must be greater than 0.", nameof(numberOfRooms));
        OfferedAmenities = offeredAmenities ?? throw new ArgumentNullException(nameof(offeredAmenities));
        Description = !string.IsNullOrWhiteSpace(description) ? description.Trim() : throw new ArgumentException("Description cannot be empty.", nameof(description));
        
        Status = OfferStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddHours(24); // 24-hour deadline
    }

    public SpecialOfferAggregate(
        Guid originalBookingId,
        Guid competingHotelId,
        Guid competingRoomId,
        Money offeredPrice,
        DateRange stayPeriod,
        int numberOfRooms,
        TravelPreferences offeredAmenities,
        string description) : base()
    {
        OriginalBookingId = originalBookingId != Guid.Empty ? originalBookingId : throw new ArgumentException("Original booking ID cannot be empty.", nameof(originalBookingId));
        CompetingHotelId = competingHotelId != Guid.Empty ? competingHotelId : throw new ArgumentException("Competing hotel ID cannot be empty.", nameof(competingHotelId));
        CompetingRoomId = competingRoomId != Guid.Empty ? competingRoomId : throw new ArgumentException("Competing room ID cannot be empty.", nameof(competingRoomId));
        OfferedPrice = offeredPrice ?? throw new ArgumentNullException(nameof(offeredPrice));
        StayPeriod = stayPeriod ?? throw new ArgumentNullException(nameof(stayPeriod));
        NumberOfRooms = numberOfRooms > 0 ? numberOfRooms : throw new ArgumentException("Number of rooms must be greater than 0.", nameof(numberOfRooms));
        OfferedAmenities = offeredAmenities ?? throw new ArgumentNullException(nameof(offeredAmenities));
        Description = !string.IsNullOrWhiteSpace(description) ? description.Trim() : throw new ArgumentException("Description cannot be empty.", nameof(description));
        
        Status = OfferStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddHours(24); // 24-hour deadline
    }

    public void AcceptOffer()
    {
        if (Status != OfferStatus.Pending)
        {
            throw new InvalidOperationException("Only pending offers can be accepted.");
        }

        if (IsExpired())
        {
            throw new InvalidOperationException("Cannot accept an expired offer.");
        }

        Status = OfferStatus.Accepted;
        RespondedAt = DateTime.UtcNow;
    }

    public void RejectOffer()
    {
        if (Status != OfferStatus.Pending)
        {
            throw new InvalidOperationException("Only pending offers can be rejected.");
        }

        Status = OfferStatus.Rejected;
        RespondedAt = DateTime.UtcNow;
    }

    public void ExpireOffer()
    {
        if (Status != OfferStatus.Pending)
        {
            throw new InvalidOperationException("Only pending offers can be expired.");
        }

        Status = OfferStatus.Expired;
        RespondedAt = DateTime.UtcNow;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }

    public bool IsPending()
    {
        return Status == OfferStatus.Pending && !IsExpired();
    }

    public TimeSpan TimeUntilExpiry()
    {
        return ExpiresAt - DateTime.UtcNow;
    }

    public Money CalculateSavings(Money originalPrice)
    {
        if (originalPrice.Currency != OfferedPrice.Currency)
        {
            throw new InvalidOperationException("Cannot compare prices with different currencies.");
        }

        var savings = originalPrice.Amount - OfferedPrice.Amount;
        return new Money(Math.Max(0, savings), originalPrice.Currency);
    }

    public decimal CalculateDiscountPercentage(Money originalPrice)
    {
        if (originalPrice.Amount <= 0)
            return 0;

        var savings = CalculateSavings(originalPrice);
        return (savings.Amount / originalPrice.Amount) * 100;
    }

    public void UpdateDescription(string newDescription)
    {
        if (Status != OfferStatus.Pending)
        {
            throw new InvalidOperationException("Description can only be updated for pending offers.");
        }

        if (string.IsNullOrWhiteSpace(newDescription))
        {
            throw new ArgumentException("Description cannot be empty.", nameof(newDescription));
        }

        Description = newDescription.Trim();
    }

    public string GetStatusDescription()
    {
        return Status switch
        {
            OfferStatus.Pending => IsExpired() ? "Expired" : "Pending",
            OfferStatus.Accepted => "Accepted",
            OfferStatus.Rejected => "Rejected",
            OfferStatus.Expired => "Expired",
            _ => "Unknown status"
        };
    }

    public string GetOfferSummary()
    {
        var amenities = OfferedAmenities.GetActivePreferences();
        var amenitiesText = amenities.Count > 0 ? $" with {string.Join(", ", amenities)}" : "";
        
        return $"{OfferedPrice} for {NumberOfRooms} room{(NumberOfRooms == 1 ? "" : "s")} for {StayPeriod.NumberOfNights} night{(StayPeriod.NumberOfNights == 1 ? "" : "s")}{amenitiesText}";
    }
}
