using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Services;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.Tests.TestHelpers;
using HotelBookingManagementSystem.Domain.Traveller;
using Xunit;

namespace HotelBookingManagementSystem.Domain.Tests;

public class BookingFinalizationTests
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

    public BookingFinalizationTests()
    {
        _bookingService = new BookingService(_travellerRepo, _hotelRepo, _roomRepo, _bookingRepo, _offerRepo);

        _traveller = new TravellerAggregate(new PersonName("John", "Doe"), new Address("Jl. Sudirman No.1", "Jakarta"), "PT ABC");
        _travellerRepo.Save(_traveller);

        _hotel = new HotelAggregate("Hotel Grand Jakarta", new Address("Jl. MH Thamrin No.1", "Jakarta"), new HotelRating(4));
        _hotelRepo.Save(_hotel);

        _room = new RoomAggregate(_hotel.Id, new RoomType("double"), new Money(1_200_000, "IDR"), 10);
        _roomRepo.Save(_room);
    }

    private BookingAggregate CreateAndSaveBooking(PaymentType? paymentType = null)
    {
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, paymentType ?? PaymentType.PayAtHotel);
        _bookingRepo.Save(booking);
        return booking;
    }

    private static CreditCardInfo ValidCard() =>
        new("4111111111111111", "John Doe", new DateTime(2026, 12, 31), "123");

    [Fact]
    public void BF001_FinalizeBookingWithValidCard_StatusBecomesFinalized()
    {
        var booking = CreateAndSaveBooking();

        _bookingService.FinalizeBooking(booking.Id, ValidCard());

        Assert.Equal(BookingStatus.Finalized, booking.Status);
        Assert.NotNull(booking.CreditCardInfo);
    }

    [Fact]
    public void BF002_FinalizeBookingWithNullCard_ThrowsException()
    {
        var booking = CreateAndSaveBooking();

        Assert.Throws<ArgumentNullException>(() => _bookingService.FinalizeBooking(booking.Id, null!));
    }

    [Fact]
    public void BF003_FinalizeBookingWithPrePaid_StatusBecomesFinalized()
    {
        var booking = CreateAndSaveBooking(PaymentType.PrePaid);

        _bookingService.FinalizeBooking(booking.Id, ValidCard());

        Assert.Equal(BookingStatus.Finalized, booking.Status);
        Assert.Equal(PaymentType.PrePaid, booking.PaymentType);
    }

    [Fact]
    public void BF004_FinalizeBookingWithPayAtHotel_StatusBecomesFinalized()
    {
        var booking = CreateAndSaveBooking();

        _bookingService.FinalizeBooking(booking.Id, ValidCard());

        Assert.Equal(BookingStatus.Finalized, booking.Status);
        Assert.Equal(PaymentType.PayAtHotel, booking.PaymentType);
    }

    [Fact]
    public void BF005_PrePaidBookingNormalReimbursementRequest_NotAllowed()
    {
        var booking = CreateAndSaveBooking(PaymentType.PrePaid);
        _bookingService.FinalizeBooking(booking.Id, ValidCard());
        _bookingService.ConfirmBooking(booking.Id);

        Assert.False(booking.RequiresHotelCompensation());
    }
}
