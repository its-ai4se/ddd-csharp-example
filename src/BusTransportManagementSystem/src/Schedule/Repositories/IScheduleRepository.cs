using BusTransportManagementSystem.Domain.Schedule;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;

namespace BusTransportManagementSystem.Domain.Schedule.Repositories;

public interface IScheduleRepository
{
    Task<ScheduleAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduleAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduleAggregate>> GetByDateAsync(ScheduledDate date, CancellationToken cancellationToken = default);
    Task AddAsync(ScheduleAggregate schedule, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleAggregate schedule, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
