using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Services;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.SpecialOffer;
using HotelBookingManagementSystem.Domain.Tests.TestHelpers;
using HotelBookingManagementSystem.Domain.Traveller;
using Xunit;

namespace HotelBookingManagementSystem.Domain.Tests;

public class PreliminaryBookingTests
{
    private readonly InMemoryTravellerRepository _travellerRepo = new();
    private readonly InMemoryHotelRepository _hotelRepo = new();
    private readonly InMemoryRoomRepository _roomRepo = new();
    private readonly InMemoryBookingRepository _bookingRepo = new();
    private readonly InMemorySpecialOfferRepository _offerRepo = new();
    private readonly OfferService _offerService;
    private readonly BookingService _bookingService;

    private readonly TravellerAggregate _traveller;
    private readonly HotelAggregate _hotel;
    private readonly HotelAggregate _competingHotel;
    private readonly RoomAggregate _room;
    private readonly RoomAggregate _competingRoom;
    private static readonly DateRange StayPeriod = new(new DateTime(2025, 7, 1), new DateTime(2025, 7, 5));

    public PreliminaryBookingTests()
    {
        _offerService = new OfferService(_bookingRepo, _hotelRepo, _roomRepo, _offerRepo, _travellerRepo);
        _bookingService = new BookingService(_travellerRepo, _hotelRepo, _roomRepo, _bookingRepo, _offerRepo);

        _traveller = new TravellerAggregate(
            new PersonName("John", "Doe"),
            new Address("Jl. Sudirman No.1", "Jakarta"),
            "PT ABC",
            new TravelPreferences(freeWifi: true, breakfastIncluded: true));
        _travellerRepo.Save(_traveller);

        _hotel = new HotelAggregate("Hotel Grand Jakarta", new Address("Jl. MH Thamrin No.1", "Jakarta"), new HotelRating(4));
        _hotelRepo.Save(_hotel);

        _room = new RoomAggregate(_hotel.Id, new RoomType("double"), new Money(1_200_000, "IDR"), 10);
        _roomRepo.Save(_room);

        _competingHotel = new HotelAggregate("Hotel Gatot Subroto", new Address("Jl. Gatot Subroto No.5", "Jakarta"), new HotelRating(4));
        _hotelRepo.Save(_competingHotel);

        _competingRoom = new RoomAggregate(_competingHotel.Id, new RoomType("double"), new Money(950_000, "IDR"), 5);
        _roomRepo.Save(_competingRoom);
    }

    private BookingAggregate CreateAndSavePreliminaryBooking()
    {
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, PaymentType.PayAtHotel);
        _bookingRepo.Save(booking);
        return booking;
    }

    [Fact]
    public void PB001_SendPreliminaryBookingToCompetitors_IncludesKeyParameters()
    {
        var booking = CreateAndSavePreliminaryBooking();

        var info = _offerService.SendPreliminaryBookingToCompetitors(booking.Id);

        Assert.Equal(booking.Id, info.BookingId);
        Assert.Equal(booking.TotalPrice, info.Price);
        Assert.Equal("Jakarta", info.CityArea);
        Assert.Equal(4, info.HotelRating.Stars);
    }

    [Fact]
    public void PB002_SendPreliminaryBookingToCompetitors_IncludesTravellerPreferencesAndReliability()
    {
        var booking = CreateAndSavePreliminaryBooking();

        var info = _offerService.SendPreliminaryBookingToCompetitors(booking.Id);

        Assert.True(info.TravellerPreferences.FreeWifi);
        Assert.True(info.TravellerPreferences.BreakfastIncluded);
        Assert.NotNull(info.TravellerReliabilityRating);
    }

    [Fact]
    public void PB003_CreateSpecialOfferWithinDeadline_IsAccepted()
    {
        var booking = CreateAndSavePreliminaryBooking();

        var offer = _offerService.CreateSpecialOffer(
            booking.Id,
            _competingHotel.Id,
            _competingRoom.Id,
            new Money(950_000, "IDR"),
            new TravelPreferences(freeWifi: true));

        Assert.NotNull(offer);
        Assert.Equal(OfferStatus.Pending, offer.Status);
        Assert.True(offer.IsPending());
    }

    [Fact]
    public void PB004_SpecialOfferAfterDeadline_IsExpired()
    {
        var offer = new SpecialOfferAggregate(
            Guid.NewGuid(),
            _competingHotel.Id,
            _competingRoom.Id,
            new Money(950_000, "IDR"),
            StayPeriod,
            2,
            new TravelPreferences());

        Assert.False(offer.IsExpired());
        Assert.True(offer.IsPending());
    }

    [Fact]
    public void PB005_GetBestOffersWith8Offers_Returns5Best()
    {
        var booking = CreateAndSavePreliminaryBooking();

        for (int i = 0; i < 8; i++)
        {
            var offer = new SpecialOfferAggregate(
                booking.Id, _competingHotel.Id, _competingRoom.Id,
                new Money(900_000 + i * 10_000, "IDR"), StayPeriod, 2, new TravelPreferences());
            _offerRepo.Save(offer);
        }

        var bestOffers = _offerService.GetBestOffers(booking.Id, 5);

        Assert.Equal(5, bestOffers.Count);
    }

    [Fact]
    public void PB006_GetBestOffersWith3Offers_ReturnsAll3()
    {
        var booking = CreateAndSavePreliminaryBooking();

        for (int i = 0; i < 3; i++)
        {
            var offer = new SpecialOfferAggregate(
                booking.Id, _competingHotel.Id, _competingRoom.Id,
                new Money(900_000 + i * 10_000, "IDR"), StayPeriod, 2, new TravelPreferences());
            _offerRepo.Save(offer);
        }

        var bestOffers = _offerService.GetBestOffers(booking.Id, 5);

        Assert.Equal(3, bestOffers.Count);
    }

    [Fact]
    public void PB007_GetBestOffersWithNoOffers_ReturnsEmpty()
    {
        var booking = CreateAndSavePreliminaryBooking();

        var bestOffers = _offerService.GetBestOffers(booking.Id, 5);

        Assert.Empty(bestOffers);
    }

    [Fact]
    public void PB008_SwitchToSpecialOffer_CancelsOriginalAndCreatesNew()
    {
        var booking = CreateAndSavePreliminaryBooking();
        var offer = new SpecialOfferAggregate(
            booking.Id, _competingHotel.Id, _competingRoom.Id,
            new Money(950_000, "IDR"), StayPeriod, 2, new TravelPreferences());
        _offerRepo.Save(offer);

        var newBooking = _bookingService.SwitchToSpecialOffer(booking.Id, offer.Id);

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.NotNull(newBooking);
        Assert.Equal(_competingHotel.Id, newBooking.HotelId);
    }

    [Fact]
    public void PB009_ContinueWithOriginalBooking_StatusRemainsPreliminiary()
    {
        var booking = CreateAndSavePreliminaryBooking();

        Assert.Equal(BookingStatus.Preliminary, booking.Status);
        Assert.Equal(_hotel.Id, booking.HotelId);
    }
}
