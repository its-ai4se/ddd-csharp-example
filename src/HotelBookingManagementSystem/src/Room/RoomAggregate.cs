using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Room;

public class RoomAggregate : AggregateRoot
{
    public Guid HotelId { get; private set; }
    public RoomType RoomType { get; private set; }
    public Money PricePerNight { get; private set; }
    public int TotalRooms { get; private set; }

    private readonly List<RoomAvailability> _availability = new();

    public RoomAggregate(
        Guid id,
        Guid hotelId,
        RoomType roomType,
        Money pricePerNight,
        int totalRooms) : base(id)
    {
        HotelId = hotelId != Guid.Empty ? hotelId : throw new ArgumentException("Hotel ID cannot be empty.", nameof(hotelId));
        RoomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
        PricePerNight = pricePerNight ?? throw new ArgumentNullException(nameof(pricePerNight));
        TotalRooms = totalRooms > 0 ? totalRooms : throw new ArgumentException("Total rooms must be greater than 0.", nameof(totalRooms));
    }

    public RoomAggregate(
        Guid hotelId,
        RoomType roomType,
        Money pricePerNight,
        int totalRooms) : base()
    {
        HotelId = hotelId != Guid.Empty ? hotelId : throw new ArgumentException("Hotel ID cannot be empty.", nameof(hotelId));
        RoomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
        PricePerNight = pricePerNight ?? throw new ArgumentNullException(nameof(pricePerNight));
        TotalRooms = totalRooms > 0 ? totalRooms : throw new ArgumentException("Total rooms must be greater than 0.", nameof(totalRooms));
    }

    public IReadOnlyList<RoomAvailability> Availability => _availability.AsReadOnly();

    public void AnnounceAvailability(DateRange dateRange, int availableRooms)
    {
        if (availableRooms < 0 || availableRooms > TotalRooms)
            throw new ArgumentException($"Available rooms must be between 0 and {TotalRooms}.", nameof(availableRooms));

        var existing = _availability.FirstOrDefault(a => a.DateRange.Overlaps(dateRange));
        if (existing != null)
            existing.UpdateAvailability(availableRooms);
        else
            _availability.Add(new RoomAvailability(dateRange, availableRooms));
    }

    public void MarkFullyBooked(DateRange dateRange) => AnnounceAvailability(dateRange, 0);

    public int GetAvailableRooms(DateRange dateRange)
    {
        var availability = _availability.FirstOrDefault(a => a.DateRange.Overlaps(dateRange));
        return availability?.AvailableRooms ?? TotalRooms;
    }

    public bool IsAvailable(DateRange dateRange, int requestedRooms) =>
        GetAvailableRooms(dateRange) >= requestedRooms;

    public Money CalculateTotalPrice(DateRange dateRange, int numberOfRooms) =>
        PricePerNight * dateRange.NumberOfNights * numberOfRooms;
}
