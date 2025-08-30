using BusTransportManagementSystem.Domain.ValueObject;

namespace BusTransportManagementSystem.Domain.Entity;

public class DriverShiftAssignment : IEquatable<DriverShiftAssignment>
{
    public Guid DriverId { get; }
    public Guid BusId { get; }
    public Guid RouteId { get; }
    public ShiftPeriod ShiftPeriod { get; }
    public ScheduledDate Date { get; }

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
    }

    public bool IsForDate(ScheduledDate date) => Date.Equals(date);

    public bool IsForDriver(Guid driverId) => DriverId == driverId;

    public bool IsForBus(Guid busId) => BusId == busId;

    public bool IsForRoute(Guid routeId) => RouteId == routeId;

    public bool IsForShift(ShiftPeriod shiftPeriod) => ShiftPeriod.Equals(shiftPeriod);

    public bool IsConflictingWith(DriverShiftAssignment other)
    {
        if (other == null) return false;
        
        // Same driver, same date, same shift period
        return DriverId == other.DriverId 
               && Date.Equals(other.Date) 
               && ShiftPeriod.Equals(other.ShiftPeriod);
    }

    public bool Equals(DriverShiftAssignment? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return DriverId == other.DriverId
               && BusId == other.BusId
               && RouteId == other.RouteId
               && Date.Equals(other.Date)
               && ShiftPeriod.Equals(other.ShiftPeriod);
    }

    public override bool Equals(object? obj) => obj is DriverShiftAssignment other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(DriverId, BusId, RouteId, Date, ShiftPeriod);

    public override string ToString() => $"Driver {DriverId} assigned to {ShiftPeriod} shift on Bus {BusId}, Route {RouteId} on {Date}";

    public static bool operator ==(DriverShiftAssignment left, DriverShiftAssignment right) => Equals(left, right);

    public static bool operator !=(DriverShiftAssignment left, DriverShiftAssignment right) => !Equals(left, right);
}