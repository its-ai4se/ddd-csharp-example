using CelebrationOrganizationSystem.Domain.Event;

namespace CelebrationOrganizationSystem.Domain.Event.Repositories;

public interface IEventRepository
{
    System.Threading.Tasks.Task<EventAggregate?> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetAllAsync();
    System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetByOrganizerIdAsync(Guid organizerId);
    System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetByAttendeeIdAsync(Guid attendeeId);
    System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetByEventTypeAsync(string eventTypeName);
    System.Threading.Tasks.Task AddAsync(EventAggregate eventAggregate);
    System.Threading.Tasks.Task UpdateAsync(EventAggregate eventAggregate);
    System.Threading.Tasks.Task DeleteAsync(Guid id);
    System.Threading.Tasks.Task<bool> ExistsAsync(Guid id);
}
