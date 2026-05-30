using HotelBookingManagementSystem.Domain.Booking.Repositories;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;
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

    public void UpdateTravellerReliabilityRating(Guid travellerId)
    {
        var traveller = _travellerRepository.GetById(travellerId)
            ?? throw new ArgumentException("Traveller not found.", nameof(travellerId));

        var bookings = _bookingRepository.GetByTravellerId(travellerId);
        var total = bookings.Count;
        var completed = bookings.Count(b => b.Status == BookingStatus.Confirmed);
        var cancelled = bookings.Count(b => b.Status == BookingStatus.Cancelled);

        traveller.UpdateReliabilityRating(total, completed, cancelled);
    }

    public Money CalculateCancellationFee(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId) ?? throw new ArgumentException("Booking not found.", nameof(bookingId));
        return booking.CalculateCancellationFee();
    }

    public bool ShouldOfferCompensation(Guid bookingId)
    {
        var booking = _bookingRepository.GetById(bookingId)
            ?? throw new ArgumentException("Booking not found.", nameof(bookingId));

        return booking.RequiresHotelCompensation();
    }
}
