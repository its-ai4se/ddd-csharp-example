using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Hotel.Repositories;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Room.Repositories;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Search;

public record SearchResult(HotelAggregate Hotel, RoomAggregate Room, Money TotalPrice);

public class AccommodationSearchService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomRepository _roomRepository;

    public AccommodationSearchService(IHotelRepository hotelRepository, IRoomRepository roomRepository)
    {
        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    }

    public List<SearchResult> Search(SearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var results = new List<SearchResult>();

        var hotels = _hotelRepository.GetByCity(criteria.City)
            .Where(h => h.Rating.Stars >= criteria.MinHotelRating.Stars);

        foreach (var hotel in hotels)
        {
            var rooms = _roomRepository.GetByHotelId(hotel.Id)
                .Where(r =>
                    r.RoomType.Name == criteria.RoomType.Name &&
                    r.PricePerNight.Amount <= criteria.MaxCostPerNight.Amount &&
                    r.PricePerNight.Currency == criteria.MaxCostPerNight.Currency &&
                    r.IsAvailable(criteria.StayPeriod, criteria.NumberOfRooms) &&
                    MatchesPreferences(hotel, criteria.Preferences));

            foreach (var room in rooms)
            {
                var totalPrice = room.CalculateTotalPrice(criteria.StayPeriod, criteria.NumberOfRooms);
                results.Add(new SearchResult(hotel, room, totalPrice));
            }
        }

        return results;
    }

    private static bool MatchesPreferences(HotelAggregate hotel, TravelPreferences? preferences)
    {
        if (preferences == null || !preferences.HasAnyPreferences())
            return true;

        var a = hotel.AvailableAmenities;
        if (preferences.BreakfastIncluded && !a.BreakfastIncluded) return false;
        if (preferences.FreeWifi && !a.FreeWifi) return false;
        if (preferences.FrontDesk24Hours && !a.FrontDesk24Hours) return false;
        if (preferences.ParkingAvailable && !a.ParkingAvailable) return false;
        if (preferences.PetFriendly && !a.PetFriendly) return false;
        if (preferences.FitnessCenter && !a.FitnessCenter) return false;
        if (preferences.Pool && !a.Pool) return false;
        if (preferences.BusinessCenter && !a.BusinessCenter) return false;
        return true;
    }
}
