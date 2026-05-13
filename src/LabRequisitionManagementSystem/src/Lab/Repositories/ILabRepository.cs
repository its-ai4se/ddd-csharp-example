using LabRequisitionManagementSystem.Domain.Lab;

namespace LabRequisitionManagementSystem.Domain.Lab.Repositories;

public interface ILabRepository
{
    Task<LabAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LabAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LabAggregate?> GetByRegistrationNumberAsync(string registrationNumber, CancellationToken cancellationToken = default);
    Task AddAsync(LabAggregate lab, CancellationToken cancellationToken = default);
    Task UpdateAsync(LabAggregate lab, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
