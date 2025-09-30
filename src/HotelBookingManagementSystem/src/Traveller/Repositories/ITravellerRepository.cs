using HotelBookingManagementSystem.Domain.Traveller;

namespace HotelBookingManagementSystem.Domain.Traveller.Repositories;

public interface ITravellerRepository
{
    TravellerAggregate? GetById(Guid id);
    TravellerAggregate? GetByEmail(string email);
    List<TravellerAggregate> GetAll();
    List<TravellerAggregate> GetByCompany(string companyName);
    void Save(TravellerAggregate traveller);
    void Delete(Guid id);
}
