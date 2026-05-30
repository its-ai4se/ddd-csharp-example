using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Hotel;

public class HotelAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public HotelRating Rating { get; private set; }
    public string? ChainName { get; private set; }
    public TravelPreferences AvailableAmenities { get; private set; }  // BR-004: filter search by preferences

    public HotelAggregate(
        Guid id,
        string name,
        Address address,
        HotelRating rating,
        string? chainName = null,
        TravelPreferences? availableAmenities = null) : base(id)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentException("Hotel name cannot be empty.", nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Rating = rating ?? throw new ArgumentNullException(nameof(rating));
        ChainName = !string.IsNullOrWhiteSpace(chainName) ? chainName.Trim() : null;
        AvailableAmenities = availableAmenities ?? new TravelPreferences();
    }

    public HotelAggregate(
        string name,
        Address address,
        HotelRating rating,
        string? chainName = null,
        TravelPreferences? availableAmenities = null) : base()
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentException("Hotel name cannot be empty.", nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Rating = rating ?? throw new ArgumentNullException(nameof(rating));
        ChainName = !string.IsNullOrWhiteSpace(chainName) ? chainName.Trim() : null;
        AvailableAmenities = availableAmenities ?? new TravelPreferences();
    }
}
