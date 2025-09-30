using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Booking.Repositories;
using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Hotel.Repositories;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Room.Repositories;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.Traveller;
using HotelBookingManagementSystem.Domain.Traveller.Repositories;

namespace HotelBookingManagementSystem.Domain.Services;

public class BookingService
{
    private readonly ITravellerRepository _travellerRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public BookingService(
        ITravellerRepository travellerRepository,
        IHotelRepository hotelRepository,
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository)
    {
        _travellerRepository = travellerRepository ?? throw new ArgumentNullException(nameof(travellerRepository));
        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
    }

    public BookingAggregate CreatePreliminaryBooking(
        Guid travellerId,
        Guid hotelId,
        Guid roomId,
        DateRange stayPeriod,
        int numberOfRooms,
        PaymentType paymentType,
        DateTime? cancellationDeadline = null)
    {
        // Validate traveller exists
        var traveller = _travellerRepository.GetById(travellerId);
        if (traveller == null)
        {
            throw new ArgumentException("Traveller not found.", nameof(travellerId));
        }

        // Validate hotel exists
        var hotel = _hotelRepository.GetById(hotelId);
        if (hotel == null)
        {
            throw new ArgumentException("Hotel not found.", nameof(hotelId));
        }

        // Validate room exists and belongs to hotel
        var room = _roomRepository.GetById(roomId);
        if (room == null)
        {
            throw new ArgumentException("Room not found.", nameof(roomId));
        }

        if (room.HotelId != hotelId)
        {
            throw new ArgumentException("Room does not belong to the specified hotel.");
        }

        // Check room availability
        if (!room.IsAvailable(stayPeriod, numberOfRooms))
        {
            throw new InvalidOperationException("Room is not available for the requested period and number of rooms.");
        }

        // Calculate total price
        var totalPrice = room.CalculateTotalPrice(stayPeriod, numberOfRooms);

        // Create preliminary booking
        var booking = new BookingAggregate(
            travellerId,
            hotelId,
            roomId,
            stayPeriod,
            numberOfRooms,
            totalPrice,
            paymentType,
            cancellationDeadline);

        // Add booking to traveller
        traveller.AddBooking(booking.Id);

        // Add booking to hotel
        hotel.AddBooking(booking.Id);

        return booking;
    }

    public void FinalizeBooking(Guid bookingId, CreditCardInfo creditCardInfo)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found.", nameof(bookingId));
        }

        booking.FinalizeBooking(creditCardInfo);
    }

    public void ConfirmBooking(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found.", nameof(bookingId));
        }

        booking.ConfirmBooking();
    }

    public void CancelBooking(Guid bookingId, string reason)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found.", nameof(bookingId));
        }

        booking.CancelBooking(reason);
    }

    public List<BookingAggregate> GetExpiredBookings()
    {
        var allBookings = _bookingRepository.GetAll();
        return allBookings.Where(b => b.IsExpired()).ToList();
    }

    public void ProcessExpiredBookings()
    {
        var expiredBookings = GetExpiredBookings();
        foreach (var booking in expiredBookings)
        {
            booking.ExpireBooking();
        }
    }
}
