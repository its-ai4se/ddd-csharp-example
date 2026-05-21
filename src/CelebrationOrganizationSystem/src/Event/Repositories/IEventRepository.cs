using CelebrationOrganizationSystem.Domain.Event;

namespace CelebrationOrganizationSystem.Domain.Event.Repositories;

public interface IEventRepository
{
    Task<EventAggregate?> GetByIdAsync(Guid id);
    Task<IEnumerable<EventAggregate>> GetByOrganizerIdAsync(Guid organizerId);
    Task<IEnumerable<EventAggregate>> GetByAttendeeIdAsync(Guid attendeeId);
    Task<IEnumerable<EventAggregate>> GetByEventTypeAsync(string eventTypeName);
    System.Threading.Tasks.Task AddAsync(EventAggregate eventAggregate);
    System.Threading.Tasks.Task UpdateAsync(EventAggregate eventAggregate);
}
