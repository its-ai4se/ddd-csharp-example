using HotelBookingManagementSystem.Domain.Shared.Common;
using HotelBookingManagementSystem.Domain.Shared.ValueObjects;

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
            throw new ArgumentException("Available rooms cannot be negative.", nameof(newAvailableRooms));
        AvailableRooms = newAvailableRooms;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DateRange;
        yield return AvailableRooms;
    }
}