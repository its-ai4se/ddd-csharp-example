using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Booking.Repositories;
using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Hotel.Repositories;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Room.Repositories;
using HotelBookingManagementSystem.Domain.Services;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.SpecialOffer;
using HotelBookingManagementSystem.Domain.SpecialOffer.Repositories;
using HotelBookingManagementSystem.Domain.Traveller;
using HotelBookingManagementSystem.Domain.Traveller.Repositories;
using Xunit;

namespace HotelBookingManagementSystem.Domain.Tests;

public class HotelBookingManagementSystemDemo
{
    private readonly BookingService _bookingService;
    private readonly OfferService _offerService;
    private readonly ReliabilityService _reliabilityService;

    public HotelBookingManagementSystemDemo()
    {
        // In a real application, these would be injected dependencies
        var travellerRepo = new InMemoryTravellerRepository();
        var hotelRepo = new InMemoryHotelRepository();
        var roomRepo = new InMemoryRoomRepository();
        var bookingRepo = new InMemoryBookingRepository();
        var offerRepo = new InMemorySpecialOfferRepository();

        _bookingService = new BookingService(travellerRepo, hotelRepo, roomRepo, bookingRepo);
        _offerService = new OfferService(bookingRepo, hotelRepo, roomRepo, offerRepo, travellerRepo);
        _reliabilityService = new ReliabilityService(travellerRepo, bookingRepo);
        
        // Store repositories for saving entities
        TravellerRepo = travellerRepo;
        HotelRepo = hotelRepo;
        RoomRepo = roomRepo;
        BookingRepo = bookingRepo;
        OfferRepo = offerRepo;
    }
    
    private InMemoryTravellerRepository TravellerRepo { get; }
    private InMemoryHotelRepository HotelRepo { get; }
    private InMemoryRoomRepository RoomRepo { get; }
    private InMemoryBookingRepository BookingRepo { get; }
    private InMemorySpecialOfferRepository OfferRepo { get; }

    [Fact]
    public void RunCompleteDemo()
    {
        Console.WriteLine("=== Hotel Booking Management System Demo ===\n");

        // 1. Create travellers
        Console.WriteLine("1. Creating travellers...");
        var traveller1 = CreateBusinessTraveller();
        var traveller2 = CreateLeisureTraveller();
        TravellerRepo.Save(traveller1);
        TravellerRepo.Save(traveller2);
        Console.WriteLine($"Created travellers: {traveller1.Name.FullName} and {traveller2.Name.FullName}\n");

        // 2. Create hotels
        Console.WriteLine("2. Creating hotels...");
        var hotel1 = CreateLuxuryHotel();
        var hotel2 = CreateBudgetHotel();
        var hotel3 = CreateBusinessHotel();
        HotelRepo.Save(hotel1);
        HotelRepo.Save(hotel2);
        HotelRepo.Save(hotel3);
        Console.WriteLine($"Created hotels: {hotel1.Name}, {hotel2.Name}, {hotel3.Name}\n");

        // 3. Create rooms
        Console.WriteLine("3. Creating rooms...");
        var room1 = CreateLuxurySuite(hotel1.Id);
        var room2 = CreateStandardRoom(hotel2.Id);
        var room3 = CreateBusinessRoom(hotel3.Id);
        RoomRepo.Save(room1);
        RoomRepo.Save(room2);
        RoomRepo.Save(room3);
        Console.WriteLine($"Created rooms: {room1.RoomType.Name} at {hotel1.Name}, {room2.RoomType.Name} at {hotel2.Name}, {room3.RoomType.Name} at {hotel3.Name}\n");

        // 4. Create preliminary booking
        Console.WriteLine("4. Creating preliminary booking...");
        var stayPeriod = new DateRange(DateTime.Now.AddDays(7), DateTime.Now.AddDays(10));
        var preliminaryBooking = _bookingService.CreatePreliminaryBooking(
            traveller1.Id,
            hotel1.Id,
            room1.Id,
            stayPeriod,
            1,
            PaymentType.PrePaid,
            DateTime.Now.AddDays(5));
        
        BookingRepo.Save(preliminaryBooking);

        Console.WriteLine($"Created preliminary booking: {preliminaryBooking.Id}");
        Console.WriteLine($"Status: {preliminaryBooking.GetStatusDescription()}");
        Console.WriteLine($"Total Price: {preliminaryBooking.TotalPrice}\n");

        // 5. Send to competitors for offers
        Console.WriteLine("5. Sending preliminary booking to competitors...");
        _offerService.SendPreliminaryBookingToCompetitors(preliminaryBooking.Id);
        Console.WriteLine("Sent booking information to competing hotels\n");

        // 6. Create special offers
        Console.WriteLine("6. Creating special offers...");
        var offer1 = _offerService.CreateSpecialOffer(
            preliminaryBooking.Id,
            hotel2.Id,
            room2.Id,
            new Money(180), // Lower price
            new TravelPreferences(breakfastIncluded: true, freeWifi: true),
            "Special business package with breakfast and WiFi included");

        var offer2 = _offerService.CreateSpecialOffer(
            preliminaryBooking.Id,
            hotel3.Id,
            room3.Id,
            new Money(200), // Even lower price
            new TravelPreferences(breakfastIncluded: true, freeWifi: true, businessCenter: true),
            "Premium business package with all amenities");
        
        OfferRepo.Save(offer1);
        OfferRepo.Save(offer2);

        Console.WriteLine($"Created offers: {offer1.GetOfferSummary()}");
        Console.WriteLine($"Savings: {offer1.CalculateSavings(preliminaryBooking.TotalPrice)}");
        Console.WriteLine($"Discount: {offer1.CalculateDiscountPercentage(preliminaryBooking.TotalPrice):F1}%\n");

        Console.WriteLine($"Created offers: {offer2.GetOfferSummary()}");
        Console.WriteLine($"Savings: {offer2.CalculateSavings(preliminaryBooking.TotalPrice)}");
        Console.WriteLine($"Discount: {offer2.CalculateDiscountPercentage(preliminaryBooking.TotalPrice):F1}%\n");

        // 7. Get best offers
        Console.WriteLine("7. Getting best offers...");
        var bestOffers = _offerService.GetBestOffers(preliminaryBooking.Id, 5);
        Console.WriteLine($"Found {bestOffers.Count} best offers:");
        foreach (var offer in bestOffers)
        {
            Console.WriteLine($"- {offer.GetOfferSummary()} (Savings: {offer.CalculateSavings(preliminaryBooking.TotalPrice)})");
        }
        Console.WriteLine();

        // 8. Accept the best offer
        if (bestOffers.Any())
        {
            Console.WriteLine("8. Accepting the best offer...");
            var bestOffer = bestOffers.First();
            _offerService.AcceptOffer(bestOffer.Id);
            Console.WriteLine($"Accepted offer: {bestOffer.GetOfferSummary()}\n");
        }

        // 9. Finalize booking
        Console.WriteLine("9. Finalizing booking...");
        var creditCard = new CreditCardInfo("4111111111111111", "John Doe", DateTime.Now.AddYears(2), "123");
        _bookingService.FinalizeBooking(preliminaryBooking.Id, creditCard);
        
        var finalizedBooking = _bookingService.GetExpiredBookings().FirstOrDefault(b => b.Id == preliminaryBooking.Id);
        Console.WriteLine($"Booking finalized. Status: {finalizedBooking?.GetStatusDescription()}\n");

        // 10. Confirm booking
        Console.WriteLine("10. Confirming booking...");
        _bookingService.ConfirmBooking(preliminaryBooking.Id);
        Console.WriteLine($"Booking confirmed\n");

        // 11. Demonstrate reliability rating
        Console.WriteLine("11. Calculating reliability rating...");
        _reliabilityService.UpdateTravellerReliabilityRating(traveller1.Id);
        var reliabilityReport = _reliabilityService.GetReliabilityReport(traveller1.Id);
        Console.WriteLine(reliabilityReport);
        Console.WriteLine();

        // 12. Demonstrate cancellation scenarios
        Console.WriteLine("12. Demonstrating cancellation scenarios...");
        
        // Create another booking for cancellation demo
        var cancellationBooking = _bookingService.CreatePreliminaryBooking(
            traveller2.Id,
            hotel2.Id,
            room2.Id,
            new DateRange(DateTime.Now.AddDays(14), DateTime.Now.AddDays(17)),
            1,
            PaymentType.PayAtHotel,
            DateTime.Now.AddDays(10));
        
        BookingRepo.Save(cancellationBooking);

        _bookingService.FinalizeBooking(cancellationBooking.Id, creditCard);
        _bookingService.ConfirmBooking(cancellationBooking.Id);

        // Cancel after deadline
        Thread.Sleep(100); // Ensure cancellation is after deadline
        _bookingService.CancelBooking(cancellationBooking.Id, "Change of plans");
        
        var cancelledBooking = _bookingService.GetExpiredBookings().FirstOrDefault(b => b.Id == cancellationBooking.Id);
        if (cancelledBooking != null)
        {
            var cancellationFee = cancelledBooking.CalculateCancellationFee();
            Console.WriteLine($"Cancelled booking after deadline. Fee: {cancellationFee}");
        }

        Console.WriteLine("\n=== Demo Complete ===");
        
        // Assertions to verify the demo worked correctly
        Assert.NotNull(traveller1);
        Assert.NotNull(traveller2);
        Assert.NotNull(hotel1);
        Assert.NotNull(hotel2);
        Assert.NotNull(hotel3);
        Assert.NotNull(room1);
        Assert.NotNull(room2);
        Assert.NotNull(room3);
        Assert.NotNull(preliminaryBooking);
        Assert.NotNull(offer1);
        Assert.NotNull(offer2);
        Assert.True(bestOffers.Count > 0);
    }

    [Fact]
    public void TestTravellerCreation()
    {
        var traveller = CreateBusinessTraveller();
        TravellerRepo.Save(traveller);
        
        Assert.NotNull(traveller);
        Assert.Equal("John", traveller.Name.FirstName);
        Assert.Equal("Doe", traveller.Name.LastName);
        Assert.Equal("TechCorp Inc", traveller.CompanyName);
        Assert.True(traveller.TravelPreferences.BreakfastIncluded);
        Assert.True(traveller.TravelPreferences.FreeWifi);
        Assert.True(traveller.TravelPreferences.BusinessCenter);
    }

    [Fact]
    public void TestHotelCreation()
    {
        var hotel = CreateLuxuryHotel();
        HotelRepo.Save(hotel);
        
        Assert.NotNull(hotel);
        Assert.Equal("The Grand Toronto", hotel.Name);
        Assert.Equal(5, hotel.Rating.Stars);
        Assert.Equal("Grand Hotels", hotel.ChainName);
        Assert.True(hotel.AvailableAmenities.BreakfastIncluded);
        Assert.True(hotel.AvailableAmenities.FreeWifi);
        Assert.True(hotel.AvailableAmenities.Pool);
    }

    [Fact]
    public void TestRoomCreation()
    {
        var hotel = CreateLuxuryHotel();
        HotelRepo.Save(hotel);
        var room = CreateLuxurySuite(hotel.Id);
        RoomRepo.Save(room);
        
        Assert.NotNull(room);
        Assert.Equal(hotel.Id, room.HotelId);
        Assert.Equal("Suite", room.RoomType.Name);
        Assert.Equal(4, room.RoomType.MaxOccupancy);
        Assert.Equal(300, room.PricePerNight.Amount);
        Assert.Equal(5, room.TotalRooms);
    }

    [Fact]
    public void TestBookingCreation()
    {
        var traveller = CreateBusinessTraveller();
        TravellerRepo.Save(traveller);
        var hotel = CreateLuxuryHotel();
        HotelRepo.Save(hotel);
        var room = CreateLuxurySuite(hotel.Id);
        RoomRepo.Save(room);
        var stayPeriod = new DateRange(DateTime.Now.AddDays(7), DateTime.Now.AddDays(10));
        
        var booking = _bookingService.CreatePreliminaryBooking(
            traveller.Id,
            hotel.Id,
            room.Id,
            stayPeriod,
            1,
            PaymentType.PrePaid,
            DateTime.Now.AddDays(5));
        
        Assert.NotNull(booking);
        Assert.Equal(BookingStatus.Preliminary, booking.Status);
        Assert.Equal(traveller.Id, booking.TravellerId);
        Assert.Equal(hotel.Id, booking.HotelId);
        Assert.Equal(room.Id, booking.RoomId);
        Assert.Equal(PaymentType.PrePaid, booking.PaymentType);
    }

    [Fact]
    public void TestSpecialOfferCreation()
    {
        var traveller = CreateBusinessTraveller();
        TravellerRepo.Save(traveller);
        var hotel1 = CreateLuxuryHotel();
        HotelRepo.Save(hotel1);
        var hotel2 = CreateBudgetHotel();
        HotelRepo.Save(hotel2);
        var room1 = CreateLuxurySuite(hotel1.Id);
        RoomRepo.Save(room1);
        var room2 = CreateStandardRoom(hotel2.Id);
        RoomRepo.Save(room2);
        var stayPeriod = new DateRange(DateTime.Now.AddDays(7), DateTime.Now.AddDays(10));
        
        var booking = _bookingService.CreatePreliminaryBooking(
            traveller.Id,
            hotel1.Id,
            room1.Id,
            stayPeriod,
            1,
            PaymentType.PrePaid);
        
        BookingRepo.Save(booking);
        
        var offer = _offerService.CreateSpecialOffer(
            booking.Id,
            hotel2.Id,
            room2.Id,
            new Money(180),
            new TravelPreferences(breakfastIncluded: true, freeWifi: true),
            "Special offer");
        
        Assert.NotNull(offer);
        Assert.Equal(booking.Id, offer.OriginalBookingId);
        Assert.Equal(hotel2.Id, offer.CompetingHotelId);
        Assert.Equal(room2.Id, offer.CompetingRoomId);
        Assert.Equal(180, offer.OfferedPrice.Amount);
        Assert.Equal(OfferStatus.Pending, offer.Status);
    }

    [Fact]
    public void TestReliabilityRating()
    {
        var traveller = CreateBusinessTraveller();
        TravellerRepo.Save(traveller);
        
        _reliabilityService.UpdateTravellerReliabilityRating(traveller.Id);
        var report = _reliabilityService.GetReliabilityReport(traveller.Id);
        
        Assert.NotNull(report);
        Assert.Contains("John Doe", report);
        Assert.Contains("Total Bookings: 0", report);
    }

    private TravellerAggregate CreateBusinessTraveller()
    {
        var name = new PersonName("John", "Doe");
        var billingAddress = new Address("123 Business St", "Toronto", "ON", "M5V 3A8");
        var companyAddress = new Address("456 Corporate Ave", "Toronto", "ON", "M5V 3B9");
        var email = new EmailAddress("john.doe@company.com");
        var phone = new PhoneNumber("416-555-0123");
        var preferences = new TravelPreferences(breakfastIncluded: true, freeWifi: true, businessCenter: true);

        return new TravellerAggregate(name, billingAddress, "TechCorp Inc", companyAddress, email, phone, preferences);
    }

    private TravellerAggregate CreateLeisureTraveller()
    {
        var name = new PersonName("Jane", "Smith");
        var billingAddress = new Address("789 Leisure Lane", "Vancouver", "BC", "V6B 1A1");
        var companyAddress = new Address("321 Vacation Blvd", "Vancouver", "BC", "V6B 1B2");
        var email = new EmailAddress("jane.smith@email.com");
        var phone = new PhoneNumber("604-555-0456");
        var preferences = new TravelPreferences(pool: true, fitnessCenter: true, petFriendly: true);

        return new TravellerAggregate(name, billingAddress, "Leisure Travel Co", companyAddress, email, phone, preferences);
    }

    private HotelAggregate CreateLuxuryHotel()
    {
        var address = new Address("1000 Luxury Blvd", "Toronto", "ON", "M5H 2N2");
        var rating = new HotelRating(5);
        var email = new EmailAddress("info@luxuryhotel.com");
        var phone = new PhoneNumber("416-555-0001");
        var amenities = new TravelPreferences(breakfastIncluded: true, freeWifi: true, frontDesk24Hours: true, 
            parkingAvailable: true, fitnessCenter: true, pool: true, businessCenter: true);

        return new HotelAggregate("The Grand Toronto", address, rating, email, phone, "Grand Hotels", amenities);
    }

    private HotelAggregate CreateBudgetHotel()
    {
        var address = new Address("200 Budget St", "Toronto", "ON", "M5H 2N3");
        var rating = new HotelRating(2);
        var email = new EmailAddress("info@budgethotel.com");
        var phone = new PhoneNumber("416-555-0002");
        var amenities = new TravelPreferences(freeWifi: true);

        return new HotelAggregate("Budget Inn", address, rating, email, phone, availableAmenities: amenities);
    }

    private HotelAggregate CreateBusinessHotel()
    {
        var address = new Address("300 Business Ave", "Toronto", "ON", "M5H 2N4");
        var rating = new HotelRating(4);
        var email = new EmailAddress("info@businesshotel.com");
        var phone = new PhoneNumber("416-555-0003");
        var amenities = new TravelPreferences(breakfastIncluded: true, freeWifi: true, businessCenter: true, 
            frontDesk24Hours: true, fitnessCenter: true);

        return new HotelAggregate("Business Plaza", address, rating, email, phone, "Business Hotels", amenities);
    }

    private RoomAggregate CreateLuxurySuite(Guid hotelId)
    {
        var roomType = RoomType.Suite;
        var price = new Money(300);
        var description = "Luxurious suite with city view, separate living area, and premium amenities";

        return new RoomAggregate(hotelId, roomType, price, 5, description);
    }

    private RoomAggregate CreateStandardRoom(Guid hotelId)
    {
        var roomType = RoomType.Double;
        var price = new Money(120);
        var description = "Comfortable standard room with modern amenities";

        return new RoomAggregate(hotelId, roomType, price, 20, description);
    }

    private RoomAggregate CreateBusinessRoom(Guid hotelId)
    {
        var roomType = RoomType.Double;
        var price = new Money(180);
        var description = "Business room with work desk, high-speed internet, and business amenities";

        return new RoomAggregate(hotelId, roomType, price, 15, description);
    }
}

// In-memory repository implementations for demo
public class InMemoryTravellerRepository : ITravellerRepository
{
    private readonly Dictionary<Guid, TravellerAggregate> _travellers = new();

    public TravellerAggregate? GetById(Guid id) => _travellers.TryGetValue(id, out var traveller) ? traveller : null;
    public TravellerAggregate? GetByEmail(string email) => _travellers.Values.FirstOrDefault(t => t.EmailAddress.Value == email);
    public List<TravellerAggregate> GetAll() => _travellers.Values.ToList();
    public List<TravellerAggregate> GetByCompany(string companyName) => _travellers.Values.Where(t => t.CompanyName == companyName).ToList();
    public void Save(TravellerAggregate traveller) => _travellers[traveller.Id] = traveller;
    public void Delete(Guid id) => _travellers.Remove(id);
}

public class InMemoryHotelRepository : IHotelRepository
{
    private readonly Dictionary<Guid, HotelAggregate> _hotels = new();

    public HotelAggregate? GetById(Guid id) => _hotels.TryGetValue(id, out var hotel) ? hotel : null;
    public List<HotelAggregate> GetAll() => _hotels.Values.ToList();
    public List<HotelAggregate> GetByCity(string city) => _hotels.Values.Where(h => h.Address.City == city).ToList();
    public List<HotelAggregate> GetByChain(string chainName) => _hotels.Values.Where(h => h.ChainName == chainName).ToList();
    public List<HotelAggregate> GetByRating(int minStars) => _hotels.Values.Where(h => h.Rating.Stars >= minStars).ToList();
    public List<HotelAggregate> GetByAmenities(TravelPreferences amenities) => _hotels.Values.Where(h => h.AvailableAmenities.HasAnyPreferences()).ToList();
    public void Save(HotelAggregate hotel) => _hotels[hotel.Id] = hotel;
    public void Delete(Guid id) => _hotels.Remove(id);
}

public class InMemoryRoomRepository : IRoomRepository
{
    private readonly Dictionary<Guid, RoomAggregate> _rooms = new();

    public RoomAggregate? GetById(Guid id) => _rooms.TryGetValue(id, out var room) ? room : null;
    public List<RoomAggregate> GetAll() => _rooms.Values.ToList();
    public List<RoomAggregate> GetByHotelId(Guid hotelId) => _rooms.Values.Where(r => r.HotelId == hotelId).ToList();
    public List<RoomAggregate> GetAvailableRooms(Guid hotelId, DateRange dateRange, int numberOfRooms) => 
        _rooms.Values.Where(r => r.HotelId == hotelId && r.IsAvailable(dateRange, numberOfRooms)).ToList();
    public List<RoomAggregate> GetByRoomType(RoomType roomType) => _rooms.Values.Where(r => r.RoomType == roomType).ToList();
    public List<RoomAggregate> GetByPriceRange(Money minPrice, Money maxPrice) => 
        _rooms.Values.Where(r => r.PricePerNight.Amount >= minPrice.Amount && r.PricePerNight.Amount <= maxPrice.Amount).ToList();
    public void Save(RoomAggregate room) => _rooms[room.Id] = room;
    public void Delete(Guid id) => _rooms.Remove(id);
}

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly Dictionary<Guid, BookingAggregate> _bookings = new();

    public BookingAggregate? GetById(Guid id) => _bookings.TryGetValue(id, out var booking) ? booking : null;
    public List<BookingAggregate> GetAll() => _bookings.Values.ToList();
    public List<BookingAggregate> GetByTravellerId(Guid travellerId) => _bookings.Values.Where(b => b.TravellerId == travellerId).ToList();
    public List<BookingAggregate> GetByHotelId(Guid hotelId) => _bookings.Values.Where(b => b.HotelId == hotelId).ToList();
    public List<BookingAggregate> GetByStatus(BookingStatus status) => _bookings.Values.Where(b => b.Status == status).ToList();
    public List<BookingAggregate> GetByDateRange(DateRange dateRange) => _bookings.Values.Where(b => b.StayPeriod.Overlaps(dateRange)).ToList();
    public List<BookingAggregate> GetExpiredBookings() => _bookings.Values.Where(b => b.IsExpired()).ToList();
    public void Save(BookingAggregate booking) => _bookings[booking.Id] = booking;
    public void Delete(Guid id) => _bookings.Remove(id);
}

public class InMemorySpecialOfferRepository : ISpecialOfferRepository
{
    private readonly Dictionary<Guid, SpecialOfferAggregate> _offers = new();

    public SpecialOfferAggregate? GetById(Guid id) => _offers.TryGetValue(id, out var offer) ? offer : null;
    public List<SpecialOfferAggregate> GetAll() => _offers.Values.ToList();
    public List<SpecialOfferAggregate> GetByOriginalBookingId(Guid originalBookingId) => _offers.Values.Where(o => o.OriginalBookingId == originalBookingId).ToList();
    public List<SpecialOfferAggregate> GetByCompetingHotelId(Guid competingHotelId) => _offers.Values.Where(o => o.CompetingHotelId == competingHotelId).ToList();
    public List<SpecialOfferAggregate> GetByStatus(OfferStatus status) => _offers.Values.Where(o => o.Status == status).ToList();
    public List<SpecialOfferAggregate> GetExpiredOffers() => _offers.Values.Where(o => o.IsExpired()).ToList();
    public List<SpecialOfferAggregate> GetPendingOffers() => _offers.Values.Where(o => o.IsPending()).ToList();
    public void Save(SpecialOfferAggregate offer) => _offers[offer.Id] = offer;
    public void Delete(Guid id) => _offers.Remove(id);
}
