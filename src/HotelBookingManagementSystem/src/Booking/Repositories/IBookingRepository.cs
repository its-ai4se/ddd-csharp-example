namespace HotelBookingManagementSystem.Domain.Booking.Repositories;

public interface IBookingRepository
{
    BookingAggregate? GetById(Guid id);
    List<BookingAggregate> GetAll();
    List<BookingAggregate> GetByTravellerId(Guid travellerId);
    void Save(BookingAggregate booking);
    void Delete(Guid id);
}
