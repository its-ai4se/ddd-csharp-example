using CelebrationOrganizationSystem.Domain.Task;

namespace CelebrationOrganizationSystem.Domain.Task.Repositories;

public interface ITaskRepository
{
    Task<ChecklistTaskAggregate?> GetByIdAsync(Guid id);
    Task<IEnumerable<ChecklistTaskAggregate>> GetByEventIdAsync(Guid eventId);
    Task<IEnumerable<ChecklistTaskAggregate>> GetAttendeeAccomplishableByEventIdAsync(Guid eventId);
    System.Threading.Tasks.Task AddAsync(ChecklistTaskAggregate task);
    System.Threading.Tasks.Task UpdateAsync(ChecklistTaskAggregate task);
}
