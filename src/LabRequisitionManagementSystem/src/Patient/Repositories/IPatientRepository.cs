using LabRequisitionManagementSystem.Domain.Patient;

namespace LabRequisitionManagementSystem.Domain.Patient.Repositories;

public interface IPatientRepository
{
    Task<PatientAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PatientAggregate?> GetByHealthNumberAsync(string healthNumber, CancellationToken cancellationToken = default);
    Task AddAsync(PatientAggregate patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(PatientAggregate patient, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
