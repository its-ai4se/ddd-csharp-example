using BusTransportManagementSystem.Domain.Route;

namespace BusTransportManagementSystem.Domain.Route.Repositories;

public interface IRouteRepository
{
    Task<RouteAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RouteAggregate?> GetByRouteNumberAsync(string routeNumber, CancellationToken cancellationToken = default);
    Task AddAsync(RouteAggregate route, CancellationToken cancellationToken = default);
    Task UpdateAsync(RouteAggregate route, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
