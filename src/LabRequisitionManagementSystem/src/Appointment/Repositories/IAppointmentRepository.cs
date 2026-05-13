using LabRequisitionManagementSystem.Domain.Appointment;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Appointment.Repositories;

public interface IAppointmentRepository
{
    Task<AppointmentAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentAggregate>> GetByRequisitionIdAsync(Guid requisitionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentAggregate>> GetByLabIdAsync(Guid labId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentAggregate>> GetByPatientIdAsync(HealthNumber patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentAggregate>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentAggregate>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task AddAsync(AppointmentAggregate appointment, CancellationToken cancellationToken = default);
    Task UpdateAsync(AppointmentAggregate appointment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
