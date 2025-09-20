using LabRequisitionManagementSystem.Domain.Doctor;

namespace LabRequisitionManagementSystem.Domain.Doctor.Repositories;

public interface IDoctorRepository
{
    Task<DoctorAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DoctorAggregate?> GetByPractitionerNumberAsync(string practitionerNumber, CancellationToken cancellationToken = default);
    Task AddAsync(DoctorAggregate doctor, CancellationToken cancellationToken = default);
    Task UpdateAsync(DoctorAggregate doctor, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
