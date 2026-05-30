using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Search;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.Tests.TestHelpers;
using Xunit;

namespace HotelBookingManagementSystem.Domain.Tests;

public class AccommodationSearchTests
{
    private readonly InMemoryHotelRepository _hotelRepo = new();
    private readonly InMemoryRoomRepository _roomRepo = new();
    private readonly AccommodationSearchService _searchService;

    public AccommodationSearchTests()
    {
        _searchService = new AccommodationSearchService(_hotelRepo, _roomRepo);
        SeedData();
    }

    private void SeedData()
    {
        // Hotel in Jakarta, 4 stars
        var hotel1 = new HotelAggregate(
            "Hotel Grand Jakarta",
            new Address("Jl. MH Thamrin No.1", "Jakarta"),
            new HotelRating(4),
            availableAmenities: new TravelPreferences(freeWifi: true, breakfastIncluded: true));
        _hotelRepo.Save(hotel1);

        var room1 = new RoomAggregate(hotel1.Id, new RoomType("double"), new Money(1_200_000, "IDR"), 10);
        _roomRepo.Save(room1);

        // Hotel in Jakarta, 3 stars
        var hotel2 = new HotelAggregate(
            "Hotel Sudirman",
            new Address("Jl. Sudirman No.5", "Jakarta"),
            new HotelRating(3));
        _hotelRepo.Save(hotel2);

        var room2 = new RoomAggregate(hotel2.Id, new RoomType("double"), new Money(800_000, "IDR"), 5);
        _roomRepo.Save(room2);

        // Hotel in Bali, 3 stars with wifi
        var hotel3 = new HotelAggregate(
            "Hotel Kuta Bali",
            new Address("Jl. Kuta No.1", "Bali"),
            new HotelRating(3),
            availableAmenities: new TravelPreferences(freeWifi: true, breakfastIncluded: true));
        _hotelRepo.Save(hotel3);

        var room3 = new RoomAggregate(hotel3.Id, new RoomType("single"), new Money(700_000, "IDR"), 8);
        _roomRepo.Save(room3);
    }

    private static DateRange JulyPeriod() => new(new DateTime(2025, 7, 1), new DateTime(2025, 7, 5));

    [Fact]
    public void AS001_SearchWithAllRequiredParams_ReturnsResults()
    {
        var criteria = new SearchCriteria(
            "Jakarta",
            JulyPeriod(),
            2,
            new RoomType("double"),
            new HotelRating(4),
            new Money(1_500_000, "IDR"));

        var results = _searchService.Search(criteria);

        Assert.NotEmpty(results);
    }

    [Fact]
    public void AS002_SearchWithPreferences_FiltersResults()
    {
        var criteria = new SearchCriteria(
            "Bali",
            new DateRange(new DateTime(2025, 8, 10), new DateTime(2025, 8, 14)),
            1,
            new RoomType("single"),
            new HotelRating(3),
            new Money(800_000, "IDR"),
            new TravelPreferences(freeWifi: true, breakfastIncluded: true));

        var results = _searchService.Search(criteria);

        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.True(r.Hotel.AvailableAmenities.FreeWifi));
    }

    [Fact]
    public void AS003_SearchWithEmptyCity_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new SearchCriteria(
            "",
            JulyPeriod(),
            1,
            new RoomType("single"),
            new HotelRating(3),
            new Money(500_000, "IDR")));
    }

    [Fact]
    public void AS004_SearchWithNullStayPeriod_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new SearchCriteria(
            "Surabaya",
            null!,
            1,
            new RoomType("single"),
            new HotelRating(3),
            new Money(500_000, "IDR")));
    }

    [Fact]
    public void AS005_SearchWithInvalidDateRange_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new DateRange(
            new DateTime(2025, 7, 5),
            new DateTime(2025, 7, 1)));
    }

    [Fact]
    public void AS006_SearchWithZeroRooms_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new SearchCriteria(
            "Yogyakarta",
            new DateRange(new DateTime(2025, 7, 1), new DateTime(2025, 7, 3)),
            0,
            new RoomType("single"),
            new HotelRating(3),
            new Money(500_000, "IDR")));
    }

    [Fact]
    public void AS007_SearchWithNullRoomType_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new SearchCriteria(
            "Yogyakarta",
            new DateRange(new DateTime(2025, 7, 1), new DateTime(2025, 7, 3)),
            2,
            null!,
            new HotelRating(3),
            new Money(500_000, "IDR")));
    }

    [Fact]
    public void AS008_SearchWithNullMinRating_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new SearchCriteria(
            "Yogyakarta",
            new DateRange(new DateTime(2025, 7, 1), new DateTime(2025, 7, 3)),
            1,
            new RoomType("twin"),
            null!,
            new Money(500_000, "IDR")));
    }

    [Fact]
    public void AS009_SearchWithNullBudget_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new SearchCriteria(
            "Yogyakarta",
            new DateRange(new DateTime(2025, 7, 1), new DateTime(2025, 7, 3)),
            1,
            new RoomType("twin"),
            new HotelRating(4),
            null!));
    }

    [Fact]
    public void AS010_SearchWithNoMatchingHotels_ReturnsEmpty()
    {
        var criteria = new SearchCriteria(
            "Kota Terpencil XYZ",
            new DateRange(new DateTime(2025, 7, 1), new DateTime(2025, 7, 3)),
            1,
            new RoomType("single"),
            new HotelRating(5),
            new Money(100_000, "IDR"));

        var results = _searchService.Search(criteria);

        Assert.Empty(results);
    }

    [Fact]
    public void AS011_SearchWithMinRating4_OnlyReturns4StarAndAbove()
    {
        var criteria = new SearchCriteria(
            "Jakarta",
            new DateRange(new DateTime(2025, 7, 1), new DateTime(2025, 7, 3)),
            1,
            new RoomType("double"),
            new HotelRating(4),
            new Money(2_000_000, "IDR"));

        var results = _searchService.Search(criteria);

        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.True(r.Hotel.Rating.Stars >= 4));
    }
}
