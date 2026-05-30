using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Services;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.Tests.TestHelpers;
using HotelBookingManagementSystem.Domain.Traveller;
using Xunit;

namespace HotelBookingManagementSystem.Domain.Tests;

public class BookingCancellationTests
{
    private readonly InMemoryTravellerRepository _travellerRepo = new();
    private readonly InMemoryHotelRepository _hotelRepo = new();
    private readonly InMemoryRoomRepository _roomRepo = new();
    private readonly InMemoryBookingRepository _bookingRepo = new();
    private readonly InMemorySpecialOfferRepository _offerRepo = new();
    private readonly BookingService _bookingService;
    private readonly ReliabilityService _reliabilityService;

    private readonly TravellerAggregate _traveller;
    private readonly HotelAggregate _hotel;
    private readonly RoomAggregate _room;
    private static readonly DateRange StayPeriod = new(new DateTime(2025, 7, 1), new DateTime(2025, 7, 5));

    public BookingCancellationTests()
    {
        _bookingService = new BookingService(_travellerRepo, _hotelRepo, _roomRepo, _bookingRepo, _offerRepo);
        _reliabilityService = new ReliabilityService(_travellerRepo, _bookingRepo);

        _traveller = new TravellerAggregate(new PersonName("John", "Doe"), new Address("Jl. Sudirman No.1", "Jakarta"), "PT ABC");
        _travellerRepo.Save(_traveller);

        _hotel = new HotelAggregate("Hotel Grand Jakarta", new Address("Jl. MH Thamrin No.1", "Jakarta"), new HotelRating(4));
        _hotelRepo.Save(_hotel);

        _room = new RoomAggregate(_hotel.Id, new RoomType("double"), new Money(1_200_000, "IDR"), 10);
        _roomRepo.Save(_room);
    }

    private BookingAggregate CreateConfirmedBooking(DateTime? cancellationDeadline = null)
    {
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, PaymentType.PayAtHotel, cancellationDeadline);
        _bookingRepo.Save(booking);
        _bookingService.FinalizeBooking(booking.Id, new CreditCardInfo("4111111111111111", "John Doe", new DateTime(2026, 12, 31), "123"));
        _bookingService.ConfirmBooking(booking.Id);
        return booking;
    }

    [Fact]
    public void BL001_CancelBeforeDeadline_NoCancellationFee()
    {
        var booking = CreateConfirmedBooking(DateTime.UtcNow.AddDays(3));

        booking.CancelBooking(CancellationInitiator.Traveller);

        Assert.False(booking.IsCancellationAfterDeadline());
        Assert.Equal(0, booking.CalculateCancellationFee().Amount);
    }

    [Fact]
    public void BL002_CancelAfterDeadline_OneNightFeeCharged()
    {
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, PaymentType.PayAtHotel, DateTime.UtcNow.AddDays(-1));
        _bookingRepo.Save(booking);
        _bookingService.FinalizeBooking(booking.Id, new CreditCardInfo("4111111111111111", "John Doe", new DateTime(2026, 12, 31), "123"));
        _bookingService.ConfirmBooking(booking.Id);

        booking.CancelBooking(CancellationInitiator.Traveller);

        Assert.True(_reliabilityService.CalculateCancellationFee(booking.Id).Amount > 0);
    }

    [Fact]
    public void BL003_CancelWithNoDeadline_NoCancellationFee()
    {
        var booking = CreateConfirmedBooking(null);

        booking.CancelBooking(CancellationInitiator.Traveller);

        Assert.Equal(0, booking.CalculateCancellationFee().Amount);
    }

    [Fact]
    public void BL004_CancelAtDeadline_BoundaryBehavior()
    {
        var booking = CreateConfirmedBooking(DateTime.UtcNow.AddSeconds(1));

        booking.CancelBooking(CancellationInitiator.Traveller);

        Assert.Equal(0, booking.CalculateCancellationFee().Amount);
    }

    [Fact]
    public void BL005_HotelCancelsBooking_CompensationRequired()
    {
        var booking = CreateConfirmedBooking();

        _bookingService.CancelByHotel(booking.Id);

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.True(booking.RequiresHotelCompensation());
    }

    [Fact]
    public void BL006_HotelCancelsBooking_CancellationInitiatorIsHotel()
    {
        var booking = CreateConfirmedBooking();

        _bookingService.CancelByHotel(booking.Id);

        Assert.Equal(CancellationInitiator.Hotel, booking.CancelledBy);
        Assert.True(_reliabilityService.ShouldOfferCompensation(booking.Id));
    }
}
