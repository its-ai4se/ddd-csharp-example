using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

namespace HotelBookingManagementSystem.Domain.Room;

public class RoomAggregate : AggregateRoot
{
    public Guid HotelId { get; private set; }
    public RoomType RoomType { get; private set; }
    public Money PricePerNight { get; private set; }
    public int TotalRooms { get; private set; }
    public string Description { get; private set; }

    private readonly List<RoomAvailability> _availability = new();

    public RoomAggregate(
        Guid id,
        Guid hotelId,
        RoomType roomType,
        Money pricePerNight,
        int totalRooms,
        string description) : base(id)
    {
        HotelId = hotelId != Guid.Empty ? hotelId : throw new ArgumentException("Hotel ID cannot be empty.", nameof(hotelId));
        RoomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
        PricePerNight = pricePerNight ?? throw new ArgumentNullException(nameof(pricePerNight));
        TotalRooms = totalRooms > 0 ? totalRooms : throw new ArgumentException("Total rooms must be greater than 0.", nameof(totalRooms));
        Description = !string.IsNullOrWhiteSpace(description) ? description.Trim() : throw new ArgumentException("Description cannot be empty.", nameof(description));
    }

    public RoomAggregate(
        Guid hotelId,
        RoomType roomType,
        Money pricePerNight,
        int totalRooms,
        string description) : base()
    {
        HotelId = hotelId != Guid.Empty ? hotelId : throw new ArgumentException("Hotel ID cannot be empty.", nameof(hotelId));
        RoomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
        PricePerNight = pricePerNight ?? throw new ArgumentNullException(nameof(pricePerNight));
        TotalRooms = totalRooms > 0 ? totalRooms : throw new ArgumentException("Total rooms must be greater than 0.", nameof(totalRooms));
        Description = !string.IsNullOrWhiteSpace(description) ? description.Trim() : throw new ArgumentException("Description cannot be empty.", nameof(description));
    }

    public IReadOnlyList<RoomAvailability> Availability => _availability.AsReadOnly();

    public void UpdatePrice(Money newPrice)
    {
        PricePerNight = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
    }

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
        {
            throw new ArgumentException("Description cannot be empty.", nameof(newDescription));
        }
        Description = newDescription.Trim();
    }

    public void UpdateTotalRooms(int newTotalRooms)
    {
        if (newTotalRooms <= 0)
        {
            throw new ArgumentException("Total rooms must be greater than 0.", nameof(newTotalRooms));
        }
        TotalRooms = newTotalRooms;
    }

    public void AnnounceAvailability(DateRange dateRange, int availableRooms)
    {
        if (availableRooms < 0 || availableRooms > TotalRooms)
        {
            throw new ArgumentException($"Available rooms must be between 0 and {TotalRooms}.", nameof(availableRooms));
        }

        var existingAvailability = _availability.FirstOrDefault(a => a.DateRange.Overlaps(dateRange));
        if (existingAvailability != null)
        {
            existingAvailability.UpdateAvailability(availableRooms);
        }
        else
        {
            _availability.Add(new RoomAvailability(dateRange, availableRooms));
        }
    }

    public void MarkFullyBooked(DateRange dateRange)
    {
        AnnounceAvailability(dateRange, 0);
    }

    public int GetAvailableRooms(DateRange dateRange)
    {
        var availability = _availability.FirstOrDefault(a => a.DateRange.Overlaps(dateRange));
        return availability?.AvailableRooms ?? TotalRooms;
    }

    public bool IsAvailable(DateRange dateRange, int requestedRooms)
    {
        return GetAvailableRooms(dateRange) >= requestedRooms;
    }

    public Money CalculateTotalPrice(DateRange dateRange, int numberOfRooms)
    {
        return PricePerNight * dateRange.NumberOfNights * numberOfRooms;
    }

    public void RemoveAvailability(DateRange dateRange)
    {
        _availability.RemoveAll(a => a.DateRange.Overlaps(dateRange));
    }
}

public class RoomAvailability : ValueObject
{
    public DateRange DateRange { get; }
    public int AvailableRooms { get; private set; }

    public RoomAvailability(DateRange dateRange, int availableRooms)
    {
        DateRange = dateRange ?? throw new ArgumentNullException(nameof(dateRange));
        AvailableRooms = availableRooms >= 0 ? availableRooms : throw new ArgumentException("Available rooms cannot be negative.", nameof(availableRooms));
    }

    public void UpdateAvailability(int newAvailableRooms)
    {
        if (newAvailableRooms < 0)
        {
            throw new ArgumentException("Available rooms cannot be negative.", nameof(newAvailableRooms));
        }
        AvailableRooms = newAvailableRooms;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DateRange;
        yield return AvailableRooms;
    }

    public override string ToString() => $"{DateRange}: {AvailableRooms} rooms available";
}
