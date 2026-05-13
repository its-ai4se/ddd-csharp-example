using LabRequisitionManagementSystem.Domain.TestsResult;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.TestsResult.Repositories;

public interface ITestResultRepository
{
    Task<TestResultAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestResultAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TestResultAggregate>> GetByTestIdAsync(Guid testId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestResultAggregate>> GetByRequisitionIdAsync(Guid requisitionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestResultAggregate>> GetByPatientIdAsync(HealthNumber patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestResultAggregate>> GetByDoctorIdAsync(PractitionerNumber doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TestResultAggregate>> GetByResultTypeAsync(TestResultType resultType, CancellationToken cancellationToken = default);
    Task AddAsync(TestResultAggregate testResult, CancellationToken cancellationToken = default);
    Task UpdateAsync(TestResultAggregate testResult, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}


