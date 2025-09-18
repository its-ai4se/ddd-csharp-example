using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;

namespace BusTransportManagementSystem.Domain.Schedule.ValueObjects;

public class BusRouteAssignment : ValueObject
{
    public Guid BusId { get; }
    public Guid RouteId { get; }
    public ScheduledDate Date { get; }
    public DateTime AssignedAt { get; }

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
        AssignedAt = DateTime.UtcNow;
    }

    public bool IsForDate(ScheduledDate date) => Date.Equals(date);

    public bool IsForBus(Guid busId) => BusId == busId;

    public bool IsForRoute(Guid routeId) => RouteId == routeId;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BusId;
        yield return RouteId;
        yield return Date;
    }

    public override string ToString() => $"Bus {BusId} assigned to Route {RouteId} on {Date}";
}
