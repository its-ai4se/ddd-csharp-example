using HotelBookingManagementSystem.Domain.Booking;
using HotelBookingManagementSystem.Domain.Booking.Repositories;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
using HotelBookingManagementSystem.Domain.Traveller;
using HotelBookingManagementSystem.Domain.Traveller.Repositories;

namespace HotelBookingManagementSystem.Domain.Services;

public class ReliabilityService
{
    private readonly ITravellerRepository _travellerRepository;
    private readonly IBookingRepository _bookingRepository;

    public ReliabilityService(
        ITravellerRepository travellerRepository,
        IBookingRepository bookingRepository)
    {
        _travellerRepository = travellerRepository ?? throw new ArgumentNullException(nameof(travellerRepository));
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
    }

    public ReliabilityRating CalculateReliabilityRating(Guid travellerId)
    {
        var traveller = _travellerRepository.GetById(travellerId);
        if (traveller == null)
        {
            throw new ArgumentException("Traveller not found.", nameof(travellerId));
        }

        var bookings = _bookingRepository.GetByTravellerId(travellerId);
        
        var totalBookings = bookings.Count;
        var completedBookings = bookings.Count(b => b.Status == BookingStatus.Confirmed);
        var cancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled);

        return new ReliabilityRating(totalBookings, completedBookings, cancelledBookings);
    }

    public void UpdateTravellerReliabilityRating(Guid travellerId)
    {
        var traveller = _travellerRepository.GetById(travellerId);
        if (traveller == null)
        {
            throw new ArgumentException("Traveller not found.", nameof(travellerId));
        }

        var reliabilityRating = CalculateReliabilityRating(travellerId);
        traveller.UpdateReliabilityRating(
            reliabilityRating.Score == 0 ? 0 : GetTotalBookings(travellerId),
            GetCompletedBookings(travellerId),
            GetCancelledBookings(travellerId));
    }

    public List<TravellerAggregate> GetTravellersByReliability(decimal minRating)
    {
        var allTravellers = _travellerRepository.GetAll();
        return allTravellers.Where(t => t.ReliabilityRating.Score >= minRating).ToList();
    }

    public List<TravellerAggregate> GetTopReliableTravellers(int count = 10)
    {
        var allTravellers = _travellerRepository.GetAll();
        return allTravellers
            .Where(t => t.HasReliabilityRating())
            .OrderByDescending(t => t.ReliabilityRating.Score)
            .Take(count)
            .ToList();
    }

    public Money CalculateCancellationFee(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found.", nameof(bookingId));
        }

        return booking.CalculateCancellationFee();
    }

    public bool ShouldOfferCompensation(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId);
        if (booking == null)
        {
            throw new ArgumentException("Booking not found.", nameof(bookingId));
        }

        // Offer compensation if booking was cancelled after deadline
        return booking.IsCancellationAfterDeadline();
    }

    private int GetTotalBookings(Guid travellerId)
    {
        return _bookingRepository.GetByTravellerId(travellerId).Count;
    }

    private int GetCompletedBookings(Guid travellerId)
    {
        return _bookingRepository.GetByTravellerId(travellerId)
            .Count(b => b.Status == BookingStatus.Confirmed);
    }

    private int GetCancelledBookings(Guid travellerId)
    {
        return _bookingRepository.GetByTravellerId(travellerId)
            .Count(b => b.Status == BookingStatus.Cancelled);
    }

    public string GetReliabilityReport(Guid travellerId)
    {
        var traveller = _travellerRepository.GetById(travellerId);
        if (traveller == null)
        {
            throw new ArgumentException("Traveller not found.", nameof(travellerId));
        }

        var totalBookings = GetTotalBookings(travellerId);
        var completedBookings = GetCompletedBookings(travellerId);
        var cancelledBookings = GetCancelledBookings(travellerId);

        return $"Traveller: {traveller.Name.FullName}\n" +
               $"Total Bookings: {totalBookings}\n" +
               $"Completed: {completedBookings}\n" +
               $"Cancelled: {cancelledBookings}\n" +
               $"Reliability Rating: {traveller.ReliabilityRating}\n" +
               $"Rating Description: {traveller.GetReliabilityDescription()}";
    }
}
