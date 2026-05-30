using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Booking;

public class BookingAggregate : AggregateRoot
{
    public Guid TravellerId { get; private set; }
    public Guid HotelId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateRange StayPeriod { get; private set; }
    public int NumberOfRooms { get; private set; }
    public Money TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }
    public PaymentType PaymentType { get; private set; }
    public CreditCardInfo? CreditCardInfo { get; private set; }
    public DateTime? CancellationDeadline { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public CancellationInitiator? CancelledBy { get; private set; }

    public BookingAggregate(
        Guid id,
        Guid travellerId,
        Guid hotelId,
        Guid roomId,
        DateRange stayPeriod,
        int numberOfRooms,
        Money totalPrice,
        PaymentType paymentType,
        DateTime? cancellationDeadline = null) : base(id)
    {
        TravellerId = travellerId != Guid.Empty ? travellerId : throw new ArgumentException("Traveller ID cannot be empty.", nameof(travellerId));
        HotelId = hotelId != Guid.Empty ? hotelId : throw new ArgumentException("Hotel ID cannot be empty.", nameof(hotelId));
        RoomId = roomId != Guid.Empty ? roomId : throw new ArgumentException("Room ID cannot be empty.", nameof(roomId));
        StayPeriod = stayPeriod ?? throw new ArgumentNullException(nameof(stayPeriod));
        NumberOfRooms = numberOfRooms > 0 ? numberOfRooms : throw new ArgumentException("Number of rooms must be greater than 0.", nameof(numberOfRooms));
        TotalPrice = totalPrice ?? throw new ArgumentNullException(nameof(totalPrice));
        PaymentType = paymentType;
        CancellationDeadline = cancellationDeadline;
        Status = BookingStatus.Preliminary;
        CreatedAt = DateTime.UtcNow;
    }

    public BookingAggregate(
        Guid travellerId,
        Guid hotelId,
        Guid roomId,
        DateRange stayPeriod,
        int numberOfRooms,
        Money totalPrice,
        PaymentType paymentType,
        DateTime? cancellationDeadline = null) : base()
    {
        TravellerId = travellerId != Guid.Empty ? travellerId : throw new ArgumentException("Traveller ID cannot be empty.", nameof(travellerId));
        HotelId = hotelId != Guid.Empty ? hotelId : throw new ArgumentException("Hotel ID cannot be empty.", nameof(hotelId));
        RoomId = roomId != Guid.Empty ? roomId : throw new ArgumentException("Room ID cannot be empty.", nameof(roomId));
        StayPeriod = stayPeriod ?? throw new ArgumentNullException(nameof(stayPeriod));
        NumberOfRooms = numberOfRooms > 0 ? numberOfRooms : throw new ArgumentException("Number of rooms must be greater than 0.", nameof(numberOfRooms));
        TotalPrice = totalPrice ?? throw new ArgumentNullException(nameof(totalPrice));
        PaymentType = paymentType;
        CancellationDeadline = cancellationDeadline;
        Status = BookingStatus.Preliminary;
        CreatedAt = DateTime.UtcNow;
    }

    public void FinalizeBooking(CreditCardInfo creditCardInfo)
    {
        if (Status != BookingStatus.Preliminary)
        {
            throw new InvalidOperationException("Only preliminary bookings can be finalized.");
        }

        CreditCardInfo = creditCardInfo ?? throw new ArgumentNullException(nameof(creditCardInfo));
        Status = BookingStatus.Finalized;
    }

    public void ConfirmBooking()
    {
        if (Status != BookingStatus.Finalized)
            throw new InvalidOperationException("Only finalized bookings can be confirmed.");

        Status = BookingStatus.Confirmed;
        AddDomainEvent(new BookingConfirmedEvent(Id, TravellerId, HotelId, StayPeriod));
    }

    public void CancelBooking(CancellationInitiator initiator)
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled.");
        if (Status == BookingStatus.Expired)
            throw new InvalidOperationException("Cannot cancel an expired booking.");

        Status = BookingStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancelledBy = initiator;
    }

    public bool RequiresHotelCompensation() =>
        Status == BookingStatus.Cancelled && CancelledBy == CancellationInitiator.Hotel;

    public void ExpireBooking()
    {
        if (Status == BookingStatus.Confirmed || Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Cannot expire confirmed or cancelled bookings.");

        Status = BookingStatus.Expired;
        AddDomainEvent(new BookingAutoExpiredEvent(Id, PaymentType == PaymentType.PrePaid, TotalPrice));
    }

    public bool IsCancellationAfterDeadline()
    {
        if (Status != BookingStatus.Cancelled || CancellationDeadline == null)
            return false;

        return CancelledAt > CancellationDeadline.Value;
    }

    public Money CalculateCancellationFee()
    {
        if (Status != BookingStatus.Cancelled)
            return new Money(0, TotalPrice.Currency);

        if (!IsCancellationAfterDeadline())
            return new Money(0, TotalPrice.Currency);

        return TotalPrice / StayPeriod.NumberOfNights;
    }

    public bool IsExpired() =>
        Status == BookingStatus.Finalized && DateTime.UtcNow > CreatedAt.AddHours(24);
}
