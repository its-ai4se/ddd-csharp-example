using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Booking.Repositories;
using HotelBookingManagementSystem.Domain.Hotel.Repositories;
using HotelBookingManagementSystem.Domain.Room.Repositories;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.SpecialOffer.Repositories;
using HotelBookingManagementSystem.Domain.Traveller.Repositories;

namespace HotelBookingManagementSystem.Domain.Services;

public class BookingService
{
    private readonly ITravellerRepository _travellerRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ISpecialOfferRepository _specialOfferRepository;

    public BookingService(
        ITravellerRepository travellerRepository,
        IHotelRepository hotelRepository,
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        ISpecialOfferRepository specialOfferRepository)
    {
        _travellerRepository = travellerRepository ?? throw new ArgumentNullException(nameof(travellerRepository));
        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _specialOfferRepository = specialOfferRepository ?? throw new ArgumentNullException(nameof(specialOfferRepository));
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
        var traveller = _travellerRepository.GetById(travellerId) ?? throw new ArgumentException("Traveller not found.", nameof(travellerId));
        var hotel = _hotelRepository.GetById(hotelId) ?? throw new ArgumentException("Hotel not found.", nameof(hotelId));

        var room = _roomRepository.GetById(roomId) ?? throw new ArgumentException("Room not found.", nameof(roomId));
        if (room.HotelId != hotelId)
        {
            throw new ArgumentException("Room does not belong to the specified hotel.");
        }

        if (!room.IsAvailable(stayPeriod, numberOfRooms))
        {
            throw new InvalidOperationException("Room is not available for the requested period and number of rooms.");
        }

        var totalPrice = room.CalculateTotalPrice(stayPeriod, numberOfRooms);

        var booking = new BookingAggregate(
            travellerId,
            hotelId,
            roomId,
            stayPeriod,
            numberOfRooms,
            totalPrice,
            paymentType,
            cancellationDeadline);

        return booking;
    }

    public void FinalizeBooking(Guid bookingId, CreditCardInfo creditCardInfo)
    {
        var booking = _bookingRepository.GetById(bookingId) ?? throw new ArgumentException("Booking not found.", nameof(bookingId));
        booking.FinalizeBooking(creditCardInfo);
    }

    public void ConfirmBooking(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId) ?? throw new ArgumentException("Booking not found.", nameof(bookingId));
        booking.ConfirmBooking();
    }

    public void CancelByHotel(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId)
            ?? throw new ArgumentException("Booking not found.", nameof(bookingId));

        booking.CancelBooking(CancellationInitiator.Hotel);
    }

    public void CancelByTraveller(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId)
            ?? throw new ArgumentException("Booking not found.", nameof(bookingId));

        booking.CancelBooking(CancellationInitiator.Traveller);
    }

    public void ProcessExpiredBookings()
    {
        var expired = _bookingRepository.GetAll().Where(b => b.IsExpired());
        foreach (var booking in expired)
            booking.ExpireBooking();
    }

    public BookingAggregate SwitchToSpecialOffer(Guid originalBookingId, Guid acceptedOfferId)
    {
        var original = _bookingRepository.GetById(originalBookingId)
            ?? throw new ArgumentException("Booking not found.", nameof(originalBookingId));

        if (original.Status != BookingStatus.Preliminary)
            throw new InvalidOperationException("Only preliminary bookings can be switched to a special offer.");

        var offer = _specialOfferRepository.GetById(acceptedOfferId)
            ?? throw new ArgumentException("Special offer not found.", nameof(acceptedOfferId));

        if (!offer.IsPending())
            throw new InvalidOperationException("Cannot switch to an expired or already-responded offer.");

        offer.AcceptOffer();
        original.CancelBooking(CancellationInitiator.Traveller);

        var newBooking = new BookingAggregate(
            original.TravellerId,
            offer.CompetingHotelId,
            offer.CompetingRoomId,
            offer.StayPeriod,
            offer.NumberOfRooms,
            offer.OfferedPrice,
            original.PaymentType,
            original.CancellationDeadline);

        return newBooking;
    }
}
