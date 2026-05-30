using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Booking;

public class BookingAutoExpiredEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid BookingId { get; }
    public bool RequiresReimbursement { get; }
    public Money Amount { get; }

    public BookingAutoExpiredEvent(Guid bookingId, bool requiresReimbursement, Money amount)
    {
        BookingId = bookingId;
        RequiresReimbursement = requiresReimbursement;
        Amount = amount;
    }
}
