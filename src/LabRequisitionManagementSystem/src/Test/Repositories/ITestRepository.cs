using LabRequisitionManagementSystem.Domain.Test;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Test.Repositories;

public interface ITestRepository
{
    Task<TestAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TestAggregate>> GetByGroupAsync(TestGroup group, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestAggregate>> GetActiveTestsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TestAggregate>> GetByAppointmentTypeAsync(AppointmentType appointmentType, CancellationToken cancellationToken = default);
    Task AddAsync(TestAggregate test, CancellationToken cancellationToken = default);
    Task UpdateAsync(TestAggregate test, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
