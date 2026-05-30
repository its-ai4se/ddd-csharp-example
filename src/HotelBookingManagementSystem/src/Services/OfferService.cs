using HotelBookingManagementSystem.Domain.Booking.Repositories;
using HotelBookingManagementSystem.Domain.Hotel.Repositories;
using HotelBookingManagementSystem.Domain.Room.Repositories;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.SpecialOffer;
using HotelBookingManagementSystem.Domain.SpecialOffer.Repositories;
using HotelBookingManagementSystem.Domain.Traveller.Repositories;

namespace HotelBookingManagementSystem.Domain.Services;

public class OfferService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ISpecialOfferRepository _specialOfferRepository;
    private readonly ITravellerRepository _travellerRepository;

    public OfferService(
        IBookingRepository bookingRepository,
        IHotelRepository hotelRepository,
        IRoomRepository roomRepository,
        ISpecialOfferRepository specialOfferRepository,
        ITravellerRepository travellerRepository)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        _specialOfferRepository = specialOfferRepository ?? throw new ArgumentNullException(nameof(specialOfferRepository));
        _travellerRepository = travellerRepository ?? throw new ArgumentNullException(nameof(travellerRepository));
    }

    public PreliminaryBookingInfo SendPreliminaryBookingToCompetitors(Guid preliminaryBookingId)
    {
        var booking = _bookingRepository.GetById(preliminaryBookingId) ?? throw new ArgumentException("Booking not found.", nameof(preliminaryBookingId));
        if (booking.Status != BookingStatus.Preliminary)
            throw new InvalidOperationException("Only preliminary bookings can be sent to competitors.");

        var traveller = _travellerRepository.GetById(booking.TravellerId) ?? throw new ArgumentException("Traveller not found.");

        var originalHotel = _hotelRepository.GetById(booking.HotelId) ?? throw new ArgumentException("Original hotel not found.");
        var info = new PreliminaryBookingInfo(
            booking.Id,
            booking.TotalPrice,
            originalHotel.Address.City,
            originalHotel.Rating,
            booking.StayPeriod,
            traveller.TravelPreferences,
            traveller.ReliabilityRating);

        // Competing hotels in the same city receive the info (excluding the original hotel)
        // In a real system this would publish a domain event or call an external service
        var _ = _hotelRepository.GetByCity(originalHotel.Address.City)
            .Where(h => h.Id != booking.HotelId)
            .ToList();

        return info;
    }

    public SpecialOfferAggregate CreateSpecialOffer(
        Guid originalBookingId,
        Guid competingHotelId,
        Guid competingRoomId,
        Money offeredPrice,
        TravelPreferences offeredAmenities)
    {
        var originalBooking = _bookingRepository.GetById(originalBookingId)
            ?? throw new ArgumentException("Original booking not found.", nameof(originalBookingId));

        if (originalBooking.Status != BookingStatus.Preliminary)
            throw new InvalidOperationException("Can only create offers for preliminary bookings.");

        var competingHotel = _hotelRepository.GetById(competingHotelId)
            ?? throw new ArgumentException("Competing hotel not found.", nameof(competingHotelId));

        var competingRoom = _roomRepository.GetById(competingRoomId)
            ?? throw new ArgumentException("Competing room not found.", nameof(competingRoomId));

        if (competingRoom.HotelId != competingHotelId)
            throw new ArgumentException("Room does not belong to the competing hotel.");

        if (!competingRoom.IsAvailable(originalBooking.StayPeriod, originalBooking.NumberOfRooms))
            throw new InvalidOperationException("Competing room is not available for the requested period.");

        return new SpecialOfferAggregate(
            originalBookingId,
            competingHotelId,
            competingRoomId,
            offeredPrice,
            originalBooking.StayPeriod,
            originalBooking.NumberOfRooms,
            offeredAmenities);
    }

    public List<SpecialOfferAggregate> GetBestOffers(Guid preliminaryBookingId, int maxOffers = 5)
    {
        var originalBooking = _bookingRepository.GetById(preliminaryBookingId) ?? throw new ArgumentException("Booking not found.", nameof(preliminaryBookingId));
        var offers = _specialOfferRepository.GetByOriginalBookingId(preliminaryBookingId)
                .Where(o => o.IsPending())
                .OrderByDescending(o => o.CalculateSavings(originalBooking.TotalPrice).Amount)
                .Take(maxOffers)
                .ToList();

        return offers;
    }

    public void ProcessExpiredOffers()
    {
        var expired = _specialOfferRepository.GetAll()
            .Where(o => o.IsExpired() && o.Status == OfferStatus.Pending);
        foreach (var offer in expired)
            offer.ExpireOffer();
    }
}
