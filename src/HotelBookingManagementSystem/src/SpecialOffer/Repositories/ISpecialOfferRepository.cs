using HotelBookingManagementSystem.Domain.SpecialOffer;

namespace HotelBookingManagementSystem.Domain.SpecialOffer.Repositories;

public interface ISpecialOfferRepository
{
    SpecialOfferAggregate? GetById(Guid id);
    List<SpecialOfferAggregate> GetAll();
    List<SpecialOfferAggregate> GetByOriginalBookingId(Guid originalBookingId);
    List<SpecialOfferAggregate> GetByCompetingHotelId(Guid competingHotelId);
    List<SpecialOfferAggregate> GetByStatus(OfferStatus status);
    List<SpecialOfferAggregate> GetExpiredOffers();
    List<SpecialOfferAggregate> GetPendingOffers();
    void Save(SpecialOfferAggregate offer);
    void Delete(Guid id);
}
