using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;

namespace BusTransportManagementSystem.Domain.Route;

public class RouteAggregate : AggregateRoot
{
    public RouteNumber RouteNumber { get; private set; }

    public RouteAggregate(Guid id, RouteNumber routeNumber) : base(id)
    {
        RouteNumber = routeNumber ?? throw new ArgumentNullException(nameof(routeNumber));
    }

    public RouteAggregate(RouteNumber routeNumber) : base()
    {
        RouteNumber = routeNumber ?? throw new ArgumentNullException(nameof(routeNumber));
    }

    public override string ToString() => $"Route: {RouteNumber} (ID: {Id})";
}
