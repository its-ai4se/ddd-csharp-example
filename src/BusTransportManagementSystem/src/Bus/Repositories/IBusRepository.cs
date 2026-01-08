using BusTransportManagementSystem.Domain.Bus;

namespace BusTransportManagementSystem.Domain.Bus.Repositories;

public interface IBusRepository
{
    Task<BusAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BusAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BusAggregate>> GetAvailableBusesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(BusAggregate bus, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SetUnderRepairAsync(Guid id, CancellationToken cancellationToken = default);
    Task SetOperationalAsync(Guid id, CancellationToken cancellationToken = default);
}
