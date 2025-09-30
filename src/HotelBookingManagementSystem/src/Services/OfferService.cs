using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Booking.Repositories;
using HotelBookingManagementSystem.Domain.Hotel;
using HotelBookingManagementSystem.Domain.Hotel.Repositories;
using HotelBookingManagementSystem.Domain.Room;
using HotelBookingManagementSystem.Domain.Room.Repositories;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.SpecialOffer;
using HotelBookingManagementSystem.Domain.SpecialOffer.Repositories;
using HotelBookingManagementSystem.Domain.Traveller;
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

    public void SendPreliminaryBookingToCompetitors(Guid preliminaryBookingId)
    {
        var booking = _bookingRepository.GetById(preliminaryBookingId);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found.", nameof(preliminaryBookingId));
        }

        if (booking.Status != BookingStatus.Preliminary)
        {
            throw new InvalidOperationException("Only preliminary bookings can be sent to competitors.");
        }

        var traveller = _travellerRepository.GetById(booking.TravellerId);
        if (traveller == null)
        {
            throw new ArgumentException("Traveller not found.");
        }

        // Get all hotels in the same city as the original booking
        var originalHotel = _hotelRepository.GetById(booking.HotelId);
        if (originalHotel == null)
        {
            throw new ArgumentException("Original hotel not found.");
        }

        var competingHotels = _hotelRepository.GetByCity(originalHotel.Address.City)
            .Where(h => h.Id != booking.HotelId)
            .ToList();

        // Send booking information to competing hotels
        foreach (var competingHotel in competingHotels)
        {
            // This would typically trigger an event or call an external service
            // For now, we'll just log the action
            Console.WriteLine($"Sending preliminary booking {booking.Id} to competing hotel {competingHotel.Name}");
        }
    }

    public SpecialOfferAggregate CreateSpecialOffer(
        Guid originalBookingId,
        Guid competingHotelId,
        Guid competingRoomId,
        Money offeredPrice,
        TravelPreferences offeredAmenities,
        string description)
    {
        var originalBooking = _bookingRepository.GetById(originalBookingId);
        if (originalBooking == null)
        {
            throw new ArgumentException("Original booking not found.", nameof(originalBookingId));
        }

        if (originalBooking.Status != BookingStatus.Preliminary)
        {
            throw new InvalidOperationException("Can only create offers for preliminary bookings.");
        }

        var competingHotel = _hotelRepository.GetById(competingHotelId);
        if (competingHotel == null)
        {
            throw new ArgumentException("Competing hotel not found.", nameof(competingHotelId));
        }

        var competingRoom = _roomRepository.GetById(competingRoomId);
        if (competingRoom == null)
        {
            throw new ArgumentException("Competing room not found.", nameof(competingRoomId));
        }

        if (competingRoom.HotelId != competingHotelId)
        {
            throw new ArgumentException("Room does not belong to the competing hotel.");
        }

        // Check if room is available for the same period
        if (!competingRoom.IsAvailable(originalBooking.StayPeriod, originalBooking.NumberOfRooms))
        {
            throw new InvalidOperationException("Competing room is not available for the requested period.");
        }

        var offer = new SpecialOfferAggregate(
            originalBookingId,
            competingHotelId,
            competingRoomId,
            offeredPrice,
            originalBooking.StayPeriod,
            originalBooking.NumberOfRooms,
            offeredAmenities,
            description);

        return offer;
    }

    public List<SpecialOfferAggregate> GetBestOffers(Guid preliminaryBookingId, int maxOffers = 5)
    {
        var originalBooking = _bookingRepository.GetById(preliminaryBookingId);
        if (originalBooking == null)
        {
            throw new ArgumentException("Booking not found.", nameof(preliminaryBookingId));
        }

        var offers = _specialOfferRepository.GetByOriginalBookingId(preliminaryBookingId)
            .Where(o => o.IsPending())
            .OrderByDescending(o => o.CalculateSavings(originalBooking.TotalPrice).Amount)
            .Take(maxOffers)
            .ToList();

        return offers;
    }

    public void AcceptOffer(Guid offerId)
    {
        var offer = _specialOfferRepository.GetById(offerId);
        if (offer == null)
        {
            throw new ArgumentException("Offer not found.", nameof(offerId));
        }

        offer.AcceptOffer();
    }

    public void RejectOffer(Guid offerId)
    {
        var offer = _specialOfferRepository.GetById(offerId);
        if (offer == null)
        {
            throw new ArgumentException("Offer not found.", nameof(offerId));
        }

        offer.RejectOffer();
    }

    public List<SpecialOfferAggregate> GetExpiredOffers()
    {
        var allOffers = _specialOfferRepository.GetAll();
        return allOffers.Where(o => o.IsExpired() && o.Status == OfferStatus.Pending).ToList();
    }

    public void ProcessExpiredOffers()
    {
        var expiredOffers = GetExpiredOffers();
        foreach (var offer in expiredOffers)
        {
            offer.ExpireOffer();
        }
    }
}
