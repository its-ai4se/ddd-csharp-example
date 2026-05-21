using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Task;

namespace CelebrationOrganizationSystem.Domain.EventTypeCatalog.Repositories;

public interface IEventTypeRepository
{
    Task<EventType?> GetByNameAsync(string name);
    Task<IEnumerable<EventType>> GetPredefinedAsync();
    System.Threading.Tasks.Task AddAsync(EventType eventType);
    Task<bool> ExistsAsync(string name);
    Task<IEnumerable<ChecklistTaskTemplate>> GetChecklistTemplatesAsync(string eventTypeName);
    System.Threading.Tasks.Task AddChecklistTemplateAsync(ChecklistTaskTemplate template);
}
