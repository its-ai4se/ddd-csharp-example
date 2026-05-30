using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Services;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.Tests.TestHelpers;
using HotelBookingManagementSystem.Domain.Traveller;
using Xunit;

namespace HotelBookingManagementSystem.Domain.Tests;

public class BookingConfirmationTests
{
    private readonly InMemoryTravellerRepository _travellerRepo = new();
    private readonly InMemoryHotelRepository _hotelRepo = new();
    private readonly InMemoryRoomRepository _roomRepo = new();
    private readonly InMemoryBookingRepository _bookingRepo = new();
    private readonly InMemorySpecialOfferRepository _offerRepo = new();
    private readonly BookingService _bookingService;

    private readonly TravellerAggregate _traveller;
    private readonly HotelAggregate _hotel;
    private readonly RoomAggregate _room;
    private static readonly DateRange StayPeriod = new(new DateTime(2025, 7, 1), new DateTime(2025, 7, 5));

    public BookingConfirmationTests()
    {
        _bookingService = new BookingService(_travellerRepo, _hotelRepo, _roomRepo, _bookingRepo, _offerRepo);

        _traveller = new TravellerAggregate(new PersonName("John", "Doe"), new Address("Jl. Sudirman No.1", "Jakarta"), "PT ABC");
        _travellerRepo.Save(_traveller);

        _hotel = new HotelAggregate("Hotel Grand Jakarta", new Address("Jl. MH Thamrin No.1", "Jakarta"), new HotelRating(4));
        _hotelRepo.Save(_hotel);

        _room = new RoomAggregate(_hotel.Id, new RoomType("double"), new Money(1_200_000, "IDR"), 10);
        _roomRepo.Save(_room);
    }

    private BookingAggregate CreateFinalizedBooking(PaymentType? paymentType = null)
    {
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, paymentType ?? PaymentType.PayAtHotel);
        _bookingRepo.Save(booking);
        _bookingService.FinalizeBooking(booking.Id, new CreditCardInfo("4111111111111111", "John Doe", new DateTime(2026, 12, 31), "123"));
        return booking;
    }

    [Fact]
    public void BC001_ConfirmBookingWithinDeadline_StatusBecomesConfirmed()
    {
        var booking = CreateFinalizedBooking();

        _bookingService.ConfirmBooking(booking.Id);

        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }

    [Fact]
    public void BC002_ExpireBookingWhenNotConfirmedIn24Hours_StatusBecomesExpired()
    {
        var booking = CreateFinalizedBooking();

        booking.ExpireBooking();

        Assert.Equal(BookingStatus.Expired, booking.Status);
    }

    [Fact]
    public void BC003_ExpirePrePaidBooking_RaisesAutoExpiredEvent()
    {
        var booking = CreateFinalizedBooking(PaymentType.PrePaid);

        booking.ExpireBooking();

        Assert.Equal(BookingStatus.Expired, booking.Status);
        var expiredEvent = booking.DomainEvents.OfType<BookingAutoExpiredEvent>().FirstOrDefault();
        Assert.NotNull(expiredEvent);
        Assert.True(expiredEvent.RequiresReimbursement);
    }

    [Fact]
    public void BC004_ExpirePayAtHotelBooking_NoReimbursementNeeded()
    {
        var booking = CreateFinalizedBooking(PaymentType.PayAtHotel);

        booking.ExpireBooking();

        Assert.Equal(BookingStatus.Expired, booking.Status);
        var expiredEvent = booking.DomainEvents.OfType<BookingAutoExpiredEvent>().FirstOrDefault();
        Assert.NotNull(expiredEvent);
        Assert.False(expiredEvent.RequiresReimbursement);
    }

    [Fact]
    public void BC005_ConfirmBooking_RaisesBookingConfirmedEvent()
    {
        var booking = CreateFinalizedBooking();

        _bookingService.ConfirmBooking(booking.Id);

        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        var confirmedEvent = booking.DomainEvents.OfType<BookingConfirmedEvent>().FirstOrDefault();
        Assert.NotNull(confirmedEvent);
    }
}
