using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Hotel.Repositories;

public interface IHotelRepository
{
    HotelAggregate? GetById(Guid id);
    List<HotelAggregate> GetAll();
    List<HotelAggregate> GetByCity(string city);
    List<HotelAggregate> GetByChain(string chainName);
    List<HotelAggregate> GetByRating(int minStars);
    List<HotelAggregate> GetByAmenities(TravelPreferences amenities);
    void Save(HotelAggregate hotel);
    void Delete(Guid id);
}
