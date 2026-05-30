namespace HotelBookingManagementSystem.Domain.Traveller.Repositories;

public interface ITravellerRepository
{
    TravellerAggregate? GetById(Guid id);
    List<TravellerAggregate> GetAll();
    void Save(TravellerAggregate traveller);
    void Delete(Guid id);
}
