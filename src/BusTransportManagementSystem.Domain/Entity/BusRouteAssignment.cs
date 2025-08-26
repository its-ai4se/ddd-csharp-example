using BusTransportManagementSystem.Domain.ValueObject;

namespace BusTransportManagementSystem.Domain.Entity;

public class BusRouteAssignment : IEquatable<BusRouteAssignment>
{
    public Guid Id { get; }
    public Guid BusId { get; }
    public Guid RouteId { get; }
    public ScheduleDate Date { get; }
    public DateTime CreatedAt { get; }

    public BusRouteAssignment(Guid id, Guid busId, Guid routeId, ScheduleDate date)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Bus route assignment ID cannot be empty.", nameof(id));
        }

        if (busId == Guid.Empty)
        {
            throw new ArgumentException("Bus ID cannot be empty.", nameof(busId));
        }

        if (routeId == Guid.Empty)
        {
            throw new ArgumentException("Route ID cannot be empty.", nameof(routeId));
        }

        Id = id;
        BusId = busId;
        RouteId = routeId;
        Date = date ?? throw new ArgumentNullException(nameof(date));
        CreatedAt = DateTime.UtcNow;
    }

    public BusRouteAssignment(Guid busId, Guid routeId, ScheduleDate date)
        : this(Guid.NewGuid(), busId, routeId, date)
    {
    }

    public bool IsForDate(ScheduleDate date) => Date.Equals(date);

    public bool IsForBus(Guid busId) => BusId == busId;

    public bool IsForRoute(Guid routeId) => RouteId == routeId;

    public bool Equals(BusRouteAssignment? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => obj is BusRouteAssignment other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => $"Bus {BusId} assigned to Route {RouteId} on {Date}";

    public static bool operator ==(BusRouteAssignment left, BusRouteAssignment right) => Equals(left, right);

    public static bool operator !=(BusRouteAssignment left, BusRouteAssignment right) => !Equals(left, right);
}