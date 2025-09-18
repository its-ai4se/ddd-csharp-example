using BusTransportManagementSystem.Domain.Driver;

namespace BusTransportManagementSystem.Domain.Driver.Repositories;

public interface IDriverRepository
{
    Task<DriverAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DriverAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DriverAggregate>> GetAvailableDriversAsync(CancellationToken cancellationToken = default);
    Task AddAsync(DriverAggregate driver, CancellationToken cancellationToken = default);
    Task UpdateAsync(DriverAggregate driver, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
