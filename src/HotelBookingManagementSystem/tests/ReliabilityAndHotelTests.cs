using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Search;
using HotelBookingManagementSystem.Domain.Services;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.Tests.TestHelpers;
using HotelBookingManagementSystem.Domain.Traveller;
using Xunit;

namespace HotelBookingManagementSystem.Domain.Tests;

public class ReliabilityRatingTests
{
    private readonly InMemoryTravellerRepository _travellerRepo = new();
    private readonly InMemoryHotelRepository _hotelRepo = new();
    private readonly InMemoryRoomRepository _roomRepo = new();
    private readonly InMemoryBookingRepository _bookingRepo = new();
    private readonly InMemorySpecialOfferRepository _offerRepo = new();
    private readonly BookingService _bookingService;
    private readonly OfferService _offerService;
    private readonly ReliabilityService _reliabilityService;

    private readonly TravellerAggregate _traveller;
    private readonly HotelAggregate _hotel;
    private readonly RoomAggregate _room;
    private static readonly DateRange StayPeriod = new(new DateTime(2025, 7, 1), new DateTime(2025, 7, 5));

    public ReliabilityRatingTests()
    {
        _bookingService = new BookingService(_travellerRepo, _hotelRepo, _roomRepo, _bookingRepo, _offerRepo);
        _offerService = new OfferService(_bookingRepo, _hotelRepo, _roomRepo, _offerRepo, _travellerRepo);
        _reliabilityService = new ReliabilityService(_travellerRepo, _bookingRepo);

        _traveller = new TravellerAggregate(
            new PersonName("John", "Doe"),
            new Address("Jl. Sudirman No.1", "Jakarta"),
            "PT ABC",
            new TravelPreferences(freeWifi: true));
        _travellerRepo.Save(_traveller);

        _hotel = new HotelAggregate("Hotel Grand Jakarta", new Address("Jl. MH Thamrin No.1", "Jakarta"), new HotelRating(4));
        _hotelRepo.Save(_hotel);

        _room = new RoomAggregate(_hotel.Id, new RoomType("double"), new Money(1_200_000, "IDR"), 10);
        _roomRepo.Save(_room);
    }

    private BookingAggregate CreateConfirmedBooking()
    {
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, PaymentType.PayAtHotel);
        _bookingRepo.Save(booking);
        _bookingService.FinalizeBooking(booking.Id, new CreditCardInfo("4111111111111111", "John Doe", new DateTime(2026, 12, 31), "123"));
        _bookingService.ConfirmBooking(booking.Id);
        return booking;
    }

    private BookingAggregate CreateCancelledBooking()
    {
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, PaymentType.PayAtHotel);
        _bookingRepo.Save(booking);
        _bookingService.FinalizeBooking(booking.Id, new CreditCardInfo("4111111111111111", "John Doe", new DateTime(2026, 12, 31), "123"));
        _bookingService.ConfirmBooking(booking.Id);
        _bookingService.CancelByTraveller(booking.Id);
        return booking;
    }

    [Fact]
    public void RR001_UpdateReliabilityRating_StoresBookingHistory()
    {
        CreateConfirmedBooking();
        CreateCancelledBooking();

        _reliabilityService.UpdateTravellerReliabilityRating(_traveller.Id);

        Assert.True(_traveller.ReliabilityRating.Score >= 0);
    }

    [Fact]
    public void RR002_SendPreliminaryBooking_IncludesReliabilityRating()
    {
        _traveller.UpdateReliabilityRating(10, 8, 2);
        var booking = _bookingService.CreatePreliminaryBooking(
            _traveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, PaymentType.PayAtHotel);
        _bookingRepo.Save(booking);

        var info = _offerService.SendPreliminaryBookingToCompetitors(booking.Id);

        Assert.NotNull(info.TravellerReliabilityRating);
        Assert.True(info.TravellerReliabilityRating.Score > 0);
    }

    [Fact]
    public void RR003_NewTraveller_HasDefaultReliabilityRating()
    {
        var newTraveller = new TravellerAggregate(
            new PersonName("New", "User"),
            new Address("Jl. Test No.1", "Jakarta"),
            "PT Test");
        _travellerRepo.Save(newTraveller);

        var booking = _bookingService.CreatePreliminaryBooking(
            newTraveller.Id, _hotel.Id, _room.Id, StayPeriod, 2, PaymentType.PayAtHotel);
        _bookingRepo.Save(booking);

        var info = _offerService.SendPreliminaryBookingToCompetitors(booking.Id);

        Assert.Equal(0, info.TravellerReliabilityRating.Score);
    }
}

public class HotelInformationTests
{
    [Fact]
    public void HI001_RegisterHotel_WithChainName_Succeeds()
    {
        var hotel = new HotelAggregate(
            "Grand Hyatt Jakarta",
            new Address("Jl. MH Thamrin No.28", "Jakarta"),
            new HotelRating(5),
            chainName: "Hyatt Hotels Corporation");

        Assert.Equal("Jakarta", hotel.Address.City);
        Assert.Equal("Jl. MH Thamrin No.28", hotel.Address.StreetAddress);
        Assert.Equal("Hyatt Hotels Corporation", hotel.ChainName);
    }

    [Fact]
    public void HI002_RegisterHotel_WithoutChainName_Succeeds()
    {
        var hotel = new HotelAggregate(
            "Hotel Melati Indah",
            new Address("Jl. Malioboro No.77", "Yogyakarta"),
            new HotelRating(3));

        Assert.Equal("Yogyakarta", hotel.Address.City);
        Assert.Null(hotel.ChainName);
    }

    [Fact]
    public void HI003_RegisterHotel_WithEmptyCity_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Address("Jl. Sudirman No.5", ""));
    }

    [Fact]
    public void HI004_RegisterHotel_WithEmptyStreetAddress_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Address("", "Bandung"));
    }
}

public class HotelRoomManagementTests
{
    private readonly InMemoryHotelRepository _hotelRepo = new();
    private readonly InMemoryRoomRepository _roomRepo = new();
    private readonly AccommodationSearchService _searchService;

    private readonly HotelAggregate _hotel;
    private readonly RoomAggregate _room;

    public HotelRoomManagementTests()
    {
        _searchService = new AccommodationSearchService(_hotelRepo, _roomRepo);

        _hotel = new HotelAggregate("Hotel Grand Jakarta", new Address("Jl. MH Thamrin No.1", "Jakarta"), new HotelRating(4));
        _hotelRepo.Save(_hotel);

        _room = new RoomAggregate(_hotel.Id, new RoomType("double"), new Money(1_200_000, "IDR"), 10);
        _roomRepo.Save(_room);
    }

    [Fact]
    public void HR001_AnnounceAvailability_UpdatesRoomInventory()
    {
        var period = new DateRange(new DateTime(2025, 7, 1), new DateTime(2025, 7, 31));

        _room.AnnounceAvailability(period, 10);

        Assert.Equal(10, _room.GetAvailableRooms(period));
    }

    [Fact]
    public void HR002_MarkFullyBooked_SetsAvailabilityToZero()
    {
        var period = new DateRange(new DateTime(2025, 7, 15), new DateTime(2025, 7, 20));

        _room.MarkFullyBooked(period);

        Assert.Equal(0, _room.GetAvailableRooms(period));
    }

    [Fact]
    public void HR003_FullyBookedRoom_NotInSearchResults()
    {
        var period = new DateRange(new DateTime(2025, 7, 16), new DateTime(2025, 7, 18));
        _room.MarkFullyBooked(period);

        var criteria = new SearchCriteria(
            "Jakarta",
            period,
            1,
            new RoomType("double"),
            new HotelRating(4),
            new Money(2_000_000, "IDR"));

        var results = _searchService.Search(criteria);

        Assert.Empty(results);
    }

    [Fact]
    public void HR004_UpdateAvailabilityAfterCancellation_RoomAppearsInSearch()
    {
        var period = new DateRange(new DateTime(2025, 7, 15), new DateTime(2025, 7, 20));
        _room.MarkFullyBooked(period);

        // Cancellation happens, rooms become available again
        _room.AnnounceAvailability(period, 3);

        Assert.Equal(3, _room.GetAvailableRooms(period));

        var criteria = new SearchCriteria(
            "Jakarta",
            period,
            1,
            new RoomType("double"),
            new HotelRating(4),
            new Money(2_000_000, "IDR"));

        var results = _searchService.Search(criteria);

        Assert.NotEmpty(results);
    }
}
