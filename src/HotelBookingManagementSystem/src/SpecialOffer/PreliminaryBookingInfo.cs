using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.SpecialOffer;

public class PreliminaryBookingInfo : ValueObject
{
    public Guid BookingId { get; }
    public Money Price { get; }
    public string CityArea { get; }
    public HotelRating HotelRating { get; }
    public DateRange StayPeriod { get; }
    public TravelPreferences TravellerPreferences { get; }
    public ReliabilityRating TravellerReliabilityRating { get; }

    public PreliminaryBookingInfo(
        Guid bookingId,
        Money price,
        string cityArea,
        HotelRating hotelRating,
        DateRange stayPeriod,
        TravelPreferences travellerPreferences,
        ReliabilityRating travellerReliabilityRating)
    {
        if (bookingId == Guid.Empty)
            throw new ArgumentException("Booking ID cannot be empty.", nameof(bookingId));
        if (string.IsNullOrWhiteSpace(cityArea))
            throw new ArgumentException("City area cannot be empty.", nameof(cityArea));

        BookingId = bookingId;
        Price = price ?? throw new ArgumentNullException(nameof(price));
        CityArea = cityArea.Trim();
        HotelRating = hotelRating ?? throw new ArgumentNullException(nameof(hotelRating));
        StayPeriod = stayPeriod ?? throw new ArgumentNullException(nameof(stayPeriod));
        TravellerPreferences = travellerPreferences ?? throw new ArgumentNullException(nameof(travellerPreferences));
        TravellerReliabilityRating = travellerReliabilityRating ?? throw new ArgumentNullException(nameof(travellerReliabilityRating));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BookingId;
        yield return Price;
        yield return CityArea;
        yield return HotelRating;
        yield return StayPeriod;
        yield return TravellerPreferences;
        yield return TravellerReliabilityRating;
    }
}
