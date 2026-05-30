namespace HotelBookingManagementSystem.Domain.SpecialOffer.Repositories;

public interface ISpecialOfferRepository
{
    SpecialOfferAggregate? GetById(Guid id);
    List<SpecialOfferAggregate> GetAll();
    List<SpecialOfferAggregate> GetByOriginalBookingId(Guid originalBookingId);
    void Save(SpecialOfferAggregate offer);
    void Delete(Guid id);
}
