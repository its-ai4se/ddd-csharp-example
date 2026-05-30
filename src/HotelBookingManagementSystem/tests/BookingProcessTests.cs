using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Services;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.Tests.TestHelpers;
using HotelBookingManagementSystem.Domain.Traveller;
using Xunit;

namespace HotelBookingManagementSystem.Domain.Tests;

public class BookingProcessTests
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

    public BookingProcessTests()
    {
        _bookingService = new BookingService(_travellerRepo, _hotelRepo, _roomRepo, _bookingRepo, _offerRepo);

        _traveller = new TravellerAggregate(new PersonName("John", "Doe"), new Address("Jl. Sudirman No.1", "Jakarta"), "PT ABC");
        _travellerRepo.Save(_traveller);

        _hotel = new HotelAggregate("Hotel Grand Jakarta", new Address("Jl. MH Thamrin No.1", "Jakarta"), new HotelRating(4));
        _hotelRepo.Save(_hotel);

        _room = new RoomAggregate(_hotel.Id, new RoomType("double"), new Money(1_200_000, "IDR"), 10);
        _roomRepo.Save(_room);
    }

    [Fact]
    public void BP001_CreateBookingRegularBooking_CreatesWithPreliminaryStatus()
    {
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, PaymentType.PayAtHotel);

        Assert.NotNull(booking);
        Assert.Equal(BookingStatus.Preliminary, booking.Status);
    }

    [Fact]
    public void BP002_CreatePreliminaryBooking_CreatesWithPreliminaryStatus()
    {
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, PaymentType.PrePaid);

        Assert.NotNull(booking);
        Assert.Equal(BookingStatus.Preliminary, booking.Status);
        Assert.Equal(_traveller.Id, booking.TravellerId);
        Assert.Equal(_hotel.Id, booking.HotelId);
    }
}
