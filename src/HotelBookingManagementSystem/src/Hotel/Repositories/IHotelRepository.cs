namespace HotelBookingManagementSystem.Domain.Hotel.Repositories;

public interface IHotelRepository
{
    HotelAggregate? GetById(Guid id);
    List<HotelAggregate> GetAll();
    List<HotelAggregate> GetByCity(string city);
    void Save(HotelAggregate hotel);
    void Delete(Guid id);
}
