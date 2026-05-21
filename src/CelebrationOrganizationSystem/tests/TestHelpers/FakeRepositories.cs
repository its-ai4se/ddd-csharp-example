using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Event.Repositories;
using CelebrationOrganizationSystem.Domain.EventTypeCatalog.Repositories;
using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Invitation.Repositories;
using CelebrationOrganizationSystem.Domain.LocationCatalog.Repositories;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Person.Repositories;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Task;
using CelebrationOrganizationSystem.Domain.Task.Repositories;

namespace CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

public class FakePersonRepository : IPersonRepository
{
    private readonly Dictionary<Guid, PersonAggregate> _store = [];

    public Task<PersonAggregate?> GetByIdAsync(Guid id) =>
        System.Threading.Tasks.Task.FromResult(_store.TryGetValue(id, out var p) ? p : null);

    public Task<PersonAggregate?> GetByEmailAsync(string email) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.FirstOrDefault(p => p.EmailAddress.Value == email.ToLowerInvariant()));

    public System.Threading.Tasks.Task AddAsync(PersonAggregate person) { _store[person.Id] = person; return System.Threading.Tasks.Task.CompletedTask; }
    public System.Threading.Tasks.Task UpdateAsync(PersonAggregate person) { _store[person.Id] = person; return System.Threading.Tasks.Task.CompletedTask; }

    public Task<bool> ExistsByEmailAsync(string email) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.Any(p => p.EmailAddress.Value == email.ToLowerInvariant()));
}

public class FakeInvitationRepository : IInvitationRepository
{
    private readonly Dictionary<Guid, InvitationAggregate> _store = [];

    public Task<InvitationAggregate?> GetByIdAsync(Guid id) =>
        System.Threading.Tasks.Task.FromResult(_store.TryGetValue(id, out var i) ? i : null);

    public Task<IEnumerable<InvitationAggregate>> GetByEventIdAsync(Guid eventId) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.Where(i => i.EventId == eventId));

    public Task<IEnumerable<InvitationAggregate>> GetByAttendeeIdAsync(Guid attendeeId) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.Where(i => i.AttendeeId == attendeeId));

    public Task<InvitationAggregate?> GetByEventAndAttendeeAsync(Guid eventId, Guid attendeeId) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.FirstOrDefault(i => i.EventId == eventId && i.AttendeeId == attendeeId));

    public System.Threading.Tasks.Task AddAsync(InvitationAggregate invitation) { _store[invitation.Id] = invitation; return System.Threading.Tasks.Task.CompletedTask; }
    public System.Threading.Tasks.Task UpdateAsync(InvitationAggregate invitation) { _store[invitation.Id] = invitation; return System.Threading.Tasks.Task.CompletedTask; }

    public Task<bool> ExistsByEventAndEmailAsync(Guid eventId, string attendeeEmail) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.Any(i => i.EventId == eventId && i.AttendeeEmail.Value == attendeeEmail.ToLowerInvariant()));
}

public class FakeEventRepository : IEventRepository
{
    private readonly Dictionary<Guid, EventAggregate> _store = [];

    public Task<EventAggregate?> GetByIdAsync(Guid id) =>
        System.Threading.Tasks.Task.FromResult(_store.TryGetValue(id, out var e) ? e : null);

    public Task<IEnumerable<EventAggregate>> GetByOrganizerIdAsync(Guid organizerId) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.Where(e => e.OrganizerIds.Contains(organizerId)));

    public Task<IEnumerable<EventAggregate>> GetByAttendeeIdAsync(Guid attendeeId) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.Where(e => e.AttendeeIds.Contains(attendeeId)));

    public Task<IEnumerable<EventAggregate>> GetByEventTypeAsync(string eventTypeName) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.Where(e => e.EventType.Name == eventTypeName));

    public System.Threading.Tasks.Task AddAsync(EventAggregate ev) { _store[ev.Id] = ev; return System.Threading.Tasks.Task.CompletedTask; }
    public System.Threading.Tasks.Task UpdateAsync(EventAggregate ev) { _store[ev.Id] = ev; return System.Threading.Tasks.Task.CompletedTask; }
}

public class FakeTaskRepository : ITaskRepository
{
    private readonly Dictionary<Guid, ChecklistTaskAggregate> _store = [];

    public Task<ChecklistTaskAggregate?> GetByIdAsync(Guid id) =>
        System.Threading.Tasks.Task.FromResult(_store.TryGetValue(id, out var t) ? t : null);

    public Task<IEnumerable<ChecklistTaskAggregate>> GetByEventIdAsync(Guid eventId) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.Where(t => t.EventId == eventId));

    public Task<IEnumerable<ChecklistTaskAggregate>> GetAttendeeAccomplishableByEventIdAsync(Guid eventId) =>
        System.Threading.Tasks.Task.FromResult(_store.Values.Where(t => t.EventId == eventId && t.IsAttendeeAccomplishable));

    public System.Threading.Tasks.Task AddAsync(ChecklistTaskAggregate task) { _store[task.Id] = task; return System.Threading.Tasks.Task.CompletedTask; }
    public System.Threading.Tasks.Task UpdateAsync(ChecklistTaskAggregate task) { _store[task.Id] = task; return System.Threading.Tasks.Task.CompletedTask; }
}

public class FakeEventTypeRepository : IEventTypeRepository
{
    private readonly Dictionary<string, EventType> _store = [];
    private readonly List<ChecklistTaskTemplate> _templates = [];

    public Task<EventType?> GetByNameAsync(string name) =>
        System.Threading.Tasks.Task.FromResult(_store.TryGetValue(name, out var et) ? et : null);

    public Task<IEnumerable<EventType>> GetPredefinedAsync() =>
        System.Threading.Tasks.Task.FromResult(_store.Values.AsEnumerable());

    public System.Threading.Tasks.Task AddAsync(EventType eventType) { _store[eventType.Name] = eventType; return System.Threading.Tasks.Task.CompletedTask; }

    public Task<bool> ExistsAsync(string name) =>
        System.Threading.Tasks.Task.FromResult(_store.ContainsKey(name));

    public Task<IEnumerable<ChecklistTaskTemplate>> GetChecklistTemplatesAsync(string eventTypeName) =>
        System.Threading.Tasks.Task.FromResult(_templates.Where(t => t.EventTypeName == eventTypeName));

    public System.Threading.Tasks.Task AddChecklistTemplateAsync(ChecklistTaskTemplate template) { _templates.Add(template); return System.Threading.Tasks.Task.CompletedTask; }

    public void SeedEventType(EventType eventType) => _store[eventType.Name] = eventType;
    public void SeedTemplate(ChecklistTaskTemplate template) => _templates.Add(template);
}

public class FakeLocationRepository : ILocationRepository
{
    private readonly Dictionary<string, Location> _store = [];

    public Task<Location?> GetByNameAsync(string name) =>
        System.Threading.Tasks.Task.FromResult(_store.TryGetValue(name, out var l) ? l : null);

    public Task<IEnumerable<Location>> GetPredefinedAsync() =>
        System.Threading.Tasks.Task.FromResult(_store.Values.AsEnumerable());

    public System.Threading.Tasks.Task AddAsync(Location location) { _store[location.Name] = location; return System.Threading.Tasks.Task.CompletedTask; }

    public Task<bool> ExistsAsync(string name) =>
        System.Threading.Tasks.Task.FromResult(_store.ContainsKey(name));

    public void SeedLocation(Location location) => _store[location.Name] = location;
}
