using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Booking.Repositories;

public interface IBookingRepository
{
    BookingAggregate? GetById(Guid id);
    List<BookingAggregate> GetAll();
    List<BookingAggregate> GetByTravellerId(Guid travellerId);
    List<BookingAggregate> GetByHotelId(Guid hotelId);
    List<BookingAggregate> GetByStatus(BookingStatus status);
    List<BookingAggregate> GetByDateRange(DateRange dateRange);
    List<BookingAggregate> GetExpiredBookings();
    void Save(BookingAggregate booking);
    void Delete(Guid id);
}
