namespace HelpingHandStore.Domain.Route.Repositories;

public interface IRouteRepository
{
    Task<RouteAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteAggregate>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task AddAsync(RouteAggregate route, CancellationToken cancellationToken = default);
}
