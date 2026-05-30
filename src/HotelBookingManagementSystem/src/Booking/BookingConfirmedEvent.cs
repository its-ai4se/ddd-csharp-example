using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Booking;

public class BookingConfirmedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid BookingId { get; }
    public Guid TravellerId { get; }
    public Guid HotelId { get; }
    public DateRange StayPeriod { get; }

    public BookingConfirmedEvent(Guid bookingId, Guid travellerId, Guid hotelId, DateRange stayPeriod)
    {
        BookingId = bookingId;
        TravellerId = travellerId;
        HotelId = hotelId;
        StayPeriod = stayPeriod;
    }
}
