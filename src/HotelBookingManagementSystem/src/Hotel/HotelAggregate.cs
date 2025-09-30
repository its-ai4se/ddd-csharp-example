using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Hotel;

public class HotelAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public HotelRating Rating { get; private set; }
    public string? ChainName { get; private set; }
    public EmailAddress ContactEmail { get; private set; }
    public PhoneNumber ContactPhone { get; private set; }
    public TravelPreferences AvailableAmenities { get; private set; }

    private readonly List<Guid> _roomIds = new();
    private readonly List<Guid> _bookingIds = new();

    public HotelAggregate(
        Guid id,
        string name,
        Address address,
        HotelRating rating,
        EmailAddress contactEmail,
        PhoneNumber contactPhone,
        string? chainName = null,
        TravelPreferences? availableAmenities = null) : base(id)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentException("Hotel name cannot be empty.", nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Rating = rating ?? throw new ArgumentNullException(nameof(rating));
        ContactEmail = contactEmail ?? throw new ArgumentNullException(nameof(contactEmail));
        ContactPhone = contactPhone ?? throw new ArgumentNullException(nameof(contactPhone));
        ChainName = !string.IsNullOrWhiteSpace(chainName) ? chainName.Trim() : null;
        AvailableAmenities = availableAmenities ?? new TravelPreferences();
    }

    public HotelAggregate(
        string name,
        Address address,
        HotelRating rating,
        EmailAddress contactEmail,
        PhoneNumber contactPhone,
        string? chainName = null,
        TravelPreferences? availableAmenities = null) : base()
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentException("Hotel name cannot be empty.", nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Rating = rating ?? throw new ArgumentNullException(nameof(rating));
        ContactEmail = contactEmail ?? throw new ArgumentNullException(nameof(contactEmail));
        ContactPhone = contactPhone ?? throw new ArgumentNullException(nameof(contactPhone));
        ChainName = !string.IsNullOrWhiteSpace(chainName) ? chainName.Trim() : null;
        AvailableAmenities = availableAmenities ?? new TravelPreferences();
    }

    public IReadOnlyList<Guid> RoomIds => _roomIds.AsReadOnly();
    public IReadOnlyList<Guid> BookingIds => _bookingIds.AsReadOnly();

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Hotel name cannot be empty.", nameof(newName));
        }
        Name = newName.Trim();
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
    }

    public void UpdateRating(HotelRating newRating)
    {
        Rating = newRating ?? throw new ArgumentNullException(nameof(newRating));
    }

    public void UpdateChainInfo(string? newChainName)
    {
        ChainName = !string.IsNullOrWhiteSpace(newChainName) ? newChainName.Trim() : null;
    }

    public void UpdateContactInfo(EmailAddress newEmail, PhoneNumber newPhone)
    {
        ContactEmail = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
        ContactPhone = newPhone ?? throw new ArgumentNullException(nameof(newPhone));
    }

    public void UpdateAvailableAmenities(TravelPreferences newAmenities)
    {
        AvailableAmenities = newAmenities ?? throw new ArgumentNullException(nameof(newAmenities));
    }

    public void AddRoom(Guid roomId)
    {
        if (roomId == Guid.Empty)
        {
            throw new ArgumentException("Room ID cannot be empty.", nameof(roomId));
        }

        if (!_roomIds.Contains(roomId))
        {
            _roomIds.Add(roomId);
        }
    }

    public void RemoveRoom(Guid roomId)
    {
        _roomIds.Remove(roomId);
    }

    public void AddBooking(Guid bookingId)
    {
        if (bookingId == Guid.Empty)
        {
            throw new ArgumentException("Booking ID cannot be empty.", nameof(bookingId));
        }

        if (!_bookingIds.Contains(bookingId))
        {
            _bookingIds.Add(bookingId);
        }
    }

    public bool IsPartOfChain()
    {
        return !string.IsNullOrWhiteSpace(ChainName);
    }

    public string GetDisplayName()
    {
        return IsPartOfChain() ? $"{Name} ({ChainName})" : Name;
    }

    public bool HasAmenity(string amenityName)
    {
        return amenityName.ToLowerInvariant() switch
        {
            "breakfast" => AvailableAmenities.BreakfastIncluded,
            "wifi" => AvailableAmenities.FreeWifi,
            "frontdesk" => AvailableAmenities.FrontDesk24Hours,
            "parking" => AvailableAmenities.ParkingAvailable,
            "pet" => AvailableAmenities.PetFriendly,
            "fitness" => AvailableAmenities.FitnessCenter,
            "pool" => AvailableAmenities.Pool,
            "business" => AvailableAmenities.BusinessCenter,
            _ => false
        };
    }
}
