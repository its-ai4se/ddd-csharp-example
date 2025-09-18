using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;

namespace BusTransportManagementSystem.Domain.Schedule.ValueObjects;

public class DriverShiftAssignment : ValueObject
{
    public Guid DriverId { get; }
    public Guid BusId { get; }
    public Guid RouteId { get; }
    public ShiftPeriod ShiftPeriod { get; }
    public ScheduledDate Date { get; }
    public DateTime AssignedAt { get; }

    public DriverShiftAssignment(Guid driverId, Guid busId, Guid routeId, ShiftPeriod shiftPeriod, ScheduledDate date)
    {
        if (driverId == Guid.Empty)
        {
            throw new ArgumentException("Driver ID cannot be empty.", nameof(driverId));
        }

        if (busId == Guid.Empty)
        {
            throw new ArgumentException("Bus ID cannot be empty.", nameof(busId));
        }

        if (routeId == Guid.Empty)
        {
            throw new ArgumentException("Route ID cannot be empty.", nameof(routeId));
        }

        DriverId = driverId;
        BusId = busId;
        RouteId = routeId;
        ShiftPeriod = shiftPeriod ?? throw new ArgumentNullException(nameof(shiftPeriod));
        Date = date ?? throw new ArgumentNullException(nameof(date));
        AssignedAt = DateTime.UtcNow;
    }

    public bool IsForDate(ScheduledDate date) => Date.Equals(date);

    public bool IsForDriver(Guid driverId) => DriverId == driverId;

    public bool IsForBus(Guid busId) => BusId == busId;

    public bool IsForRoute(Guid routeId) => RouteId == routeId;

    public bool IsForShift(ShiftPeriod shiftPeriod) => ShiftPeriod.Equals(shiftPeriod);

    public bool IsConflictingWith(DriverShiftAssignment? other)
    {
        if (other == null) return false;
        
        // Same driver, same date, same shift period
        return DriverId == other.DriverId 
               && Date.Equals(other.Date) 
               && ShiftPeriod.Equals(other.ShiftPeriod);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DriverId;
        yield return BusId;
        yield return RouteId;
        yield return Date;
        yield return ShiftPeriod;
    }

    public override string ToString() => $"Driver {DriverId} assigned to {ShiftPeriod} shift on Bus {BusId}, Route {RouteId} on {Date}";
}
