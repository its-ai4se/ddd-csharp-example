using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Search;

public class SearchCriteria : ValueObject
{
    public string City { get; }
    public DateRange StayPeriod { get; }
    public int NumberOfRooms { get; }
    public RoomType RoomType { get; }
    public HotelRating MinHotelRating { get; }
    public Money MaxCostPerNight { get; }
    public TravelPreferences? Preferences { get; }

    public SearchCriteria(
        string city,
        DateRange stayPeriod,
        int numberOfRooms,
        RoomType roomType,
        HotelRating minHotelRating,
        Money maxCostPerNight,
        TravelPreferences? preferences = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.", nameof(city));
        if (numberOfRooms <= 0)
            throw new ArgumentException("Number of rooms must be greater than 0.", nameof(numberOfRooms));

        City = city.Trim();
        StayPeriod = stayPeriod ?? throw new ArgumentNullException(nameof(stayPeriod));
        NumberOfRooms = numberOfRooms;
        RoomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
        MinHotelRating = minHotelRating ?? throw new ArgumentNullException(nameof(minHotelRating));
        MaxCostPerNight = maxCostPerNight ?? throw new ArgumentNullException(nameof(maxCostPerNight));
        Preferences = preferences;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return City;
        yield return StayPeriod;
        yield return NumberOfRooms;
        yield return RoomType;
        yield return MinHotelRating;
        yield return MaxCostPerNight;
        yield return Preferences ?? new TravelPreferences();
    }
}
