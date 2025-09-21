using CelebrationOrganizationSystem.Domain.Task;

namespace CelebrationOrganizationSystem.Domain.Task.Repositories;

public interface ITaskRepository
{
    System.Threading.Tasks.Task<TaskAggregate?> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetAllAsync();
    System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetByEventIdAsync(Guid eventId);
    System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetByAssignedAttendeeIdAsync(Guid attendeeId);
    System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetByStatusAsync(TaskStatus status);
    System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetByTypeAsync(TaskType type);
    System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetAvailableTasksAsync();
    System.Threading.Tasks.Task AddAsync(TaskAggregate task);
    System.Threading.Tasks.Task UpdateAsync(TaskAggregate task);
    System.Threading.Tasks.Task DeleteAsync(Guid id);
    System.Threading.Tasks.Task<bool> ExistsAsync(Guid id);
}
