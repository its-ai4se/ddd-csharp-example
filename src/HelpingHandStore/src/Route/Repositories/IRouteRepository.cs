using HelpingHandStore.Domain.Route;

namespace HelpingHandStore.Domain.Route.Repositories;

public interface IRouteRepository
{
    Task<RouteAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteAggregate>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteAggregate>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteAggregate>> GetByVolunteerIdAsync(Guid volunteerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteAggregate>> GetPlannedRoutesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(RouteAggregate route, CancellationToken cancellationToken = default);
    Task UpdateAsync(RouteAggregate route, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
