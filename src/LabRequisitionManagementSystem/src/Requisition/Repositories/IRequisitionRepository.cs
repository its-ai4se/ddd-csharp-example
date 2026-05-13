using LabRequisitionManagementSystem.Domain.Requisition;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Requisition.Repositories;

public interface IRequisitionRepository
{
    Task<RequisitionAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequisitionAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RequisitionAggregate>> GetByDoctorPractitionerNumberAsync(PractitionerNumber doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequisitionAggregate>> GetByPatientHealthNumberAsync(HealthNumber patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequisitionAggregate>> GetValidRequisitionsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(RequisitionAggregate requisition, CancellationToken cancellationToken = default);
    Task UpdateAsync(RequisitionAggregate requisition, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
