using BusTransportManagementSystem.Domain.ValueObject;

namespace BusTransportManagementSystem.Domain.Entity;

public class Route : IEquatable<Route>
{
    public Guid Id { get; }
    public RouteNumber RouteNumber { get; private set; }

    public Route(Guid id, RouteNumber routeNumber)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Route ID cannot be empty.", nameof(id));
        }

        Id = id;
        RouteNumber = routeNumber ?? throw new ArgumentNullException(nameof(routeNumber));
    }

    public Route(RouteNumber routeNumber)
        : this(Guid.NewGuid(), routeNumber)
    {
    }

    public void UpdateRouteNumber(RouteNumber newRouteNumber)
    {
        RouteNumber = newRouteNumber ?? throw new ArgumentNullException(nameof(newRouteNumber));
    }

    public bool Equals(Route? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => obj is Route other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => $"Route: {RouteNumber} (ID: {Id})";

    public static bool operator ==(Route left, Route right) => Equals(left, right);

    public static bool operator !=(Route left, Route right) => !Equals(left, right);
}
