namespace HelpingHandStore.Domain.Vehicle.Repositories;

public interface IVehicleRepository
{
    Task<VehicleAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(VehicleAggregate vehicle, CancellationToken cancellationToken = default);
}
