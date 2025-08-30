using BusTransportManagementSystem.Domain.ValueObject;

namespace BusTransportManagementSystem.Domain.Entity;

public class BusRouteAssignment : IEquatable<BusRouteAssignment>
{
    public Guid BusId { get; }
    public Guid RouteId { get; }
    public ScheduledDate Date { get; }

    public BusRouteAssignment(Guid busId, Guid routeId, ScheduledDate date)
    {
        if (busId == Guid.Empty)
        {
            throw new ArgumentException("Bus ID cannot be empty.", nameof(busId));
        }

        if (routeId == Guid.Empty)
        {
            throw new ArgumentException("Route ID cannot be empty.", nameof(routeId));
        }

        BusId = busId;
        RouteId = routeId;
        Date = date ?? throw new ArgumentNullException(nameof(date));
    }

    public bool IsForDate(ScheduledDate date) => Date.Equals(date);

    public bool IsForBus(Guid busId) => BusId == busId;

    public bool IsForRoute(Guid routeId) => RouteId == routeId;

    public bool Equals(BusRouteAssignment? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return BusId == other.BusId && RouteId == other.RouteId && Date.Equals(other.Date);
    }

    public override bool Equals(object? obj) => obj is BusRouteAssignment other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BusId, RouteId, Date);

    public override string ToString() => $"Bus {BusId} assigned to Route {RouteId} on {Date}";

    public static bool operator ==(BusRouteAssignment left, BusRouteAssignment right) => Equals(left, right);

    public static bool operator !=(BusRouteAssignment left, BusRouteAssignment right) => !Equals(left, right);
}