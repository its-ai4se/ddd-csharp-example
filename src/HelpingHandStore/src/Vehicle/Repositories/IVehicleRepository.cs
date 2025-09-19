using HelpingHandStore.Domain.Vehicle;

namespace HelpingHandStore.Domain.Vehicle.Repositories;

public interface IVehicleRepository
{
    Task<VehicleAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<VehicleAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<VehicleAggregate>> GetAvailableVehiclesAsync(CancellationToken cancellationToken = default);
    Task<VehicleAggregate?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task AddAsync(VehicleAggregate vehicle, CancellationToken cancellationToken = default);
    Task UpdateAsync(VehicleAggregate vehicle, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
