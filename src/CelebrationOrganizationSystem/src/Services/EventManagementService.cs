using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Event.Repositories;
using CelebrationOrganizationSystem.Domain.EventTypeCatalog.Repositories;
using CelebrationOrganizationSystem.Domain.Invitation.Repositories;
using CelebrationOrganizationSystem.Domain.LocationCatalog.Repositories;
using CelebrationOrganizationSystem.Domain.Person.Repositories;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Task;
using CelebrationOrganizationSystem.Domain.Task.Repositories;

namespace CelebrationOrganizationSystem.Domain.Services;

public class EventManagementService
{
    private readonly IEventRepository _eventRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly ILocationRepository _locationRepository;

    public EventManagementService(
        IEventRepository eventRepository,
        IPersonRepository personRepository,
        IInvitationRepository invitationRepository,
        ITaskRepository taskRepository,
        IEventTypeRepository eventTypeRepository,
        ILocationRepository locationRepository)
    {
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _eventTypeRepository = eventTypeRepository ?? throw new ArgumentNullException(nameof(eventTypeRepository));
        _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));
    }

    public async Task<EventAggregate> CreateEventAsync(
        string occasion,
        EventType eventType,
        DateTimeRange dateTimeRange,
        Location location,
        IEnumerable<EventOrganizer> organizers)
    {
        if (string.IsNullOrWhiteSpace(occasion))
        {
            throw new ArgumentException("Occasion cannot be empty or whitespace.", nameof(occasion));
        }

        if (eventType is null)
        {
            throw new ArgumentNullException(nameof(eventType));
        }

        if (dateTimeRange is null)
        {
            throw new ArgumentNullException(nameof(dateTimeRange));
        }

        if (location is null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        var organizerList = organizers?.ToList() ?? throw new ArgumentNullException(nameof(organizers));
        if (organizerList.Count == 0)
        {
            throw new DomainException("An event must have at least one organizer.");
        }

        foreach (var organizerMembership in organizerList)
        {
            var organizer = await _personRepository.GetByIdAsync(organizerMembership.OrganizerId);
            if (organizer is null)
            {
                throw new DomainException($"Organizer with ID {organizerMembership.OrganizerId} not found.");
            }

            if (!organizer.IsOrganizer)
            {
                throw new DomainException($"Person {organizerMembership.OrganizerId} is not an organizer.");
            }
        }
        
        if (!await _eventTypeRepository.ExistsAsync(eventType.Name))
        {
            await _eventTypeRepository.AddAsync(eventType);
        }

        if (!await _locationRepository.ExistsAsync(location.Name))
        {
            await _locationRepository.AddAsync(location);
        }

        var eventAggregate = new EventAggregate(occasion, eventType, dateTimeRange, location, organizerList);
        await _eventRepository.AddAsync(eventAggregate);

        var templates = await _eventTypeRepository.GetChecklistTemplatesAsync(eventType.Name);
        foreach (var template in templates)
        {
            var task = new ChecklistTaskAggregate(
                eventAggregate.Id,
                template.Title,
                template.Description,
                template.IsAttendeeAccomplishableByDefault);
            await _taskRepository.AddAsync(task);
            eventAggregate.AddChecklistTask(task.Id);
        }

        if (eventAggregate.ChecklistTaskIds.Count > 0)
        {
            await _eventRepository.UpdateAsync(eventAggregate);
        }

        return eventAggregate;
    }

    public async System.Threading.Tasks.Task AddOrganizerAsync(Guid eventId, Guid organizerId, bool isAttending)
    {
        var eventAggregate = await GetEventOrThrowAsync(eventId);
        var organizer = await _personRepository.GetByIdAsync(organizerId);
        if (organizer is null || !organizer.IsOrganizer)
        {
            throw new DomainException($"Person {organizerId} is not an organizer.");
        }

        eventAggregate.AddOrganizer(organizerId, isAttending);
        await _eventRepository.UpdateAsync(eventAggregate);
    }
public async System.Threading.Tasks.Task SetOrganizerAttendanceAsync(Guid eventId, Guid organizerId, bool isAttending)
    {
        var eventAggregate = await GetEventOrThrowAsync(eventId);
        eventAggregate.SetOrganizerAttendance(organizerId, isAttending);
        await _eventRepository.UpdateAsync(eventAggregate);
    }

    public async Task<ChecklistTaskAggregate> AddChecklistTaskAsync(
        Guid eventId,
        string title,
        string? description = null,
        bool attendeeAccomplishable = false)
    {
        var eventAggregate = await GetEventOrThrowAsync(eventId);
        var task = new ChecklistTaskAggregate(eventId, title, description, attendeeAccomplishable);
        await _taskRepository.AddAsync(task);
        eventAggregate.AddChecklistTask(task.Id);
        await _eventRepository.UpdateAsync(eventAggregate);

        var template = new ChecklistTaskTemplate(eventAggregate.EventType.Name, title, description, attendeeAccomplishable);
        await _eventTypeRepository.AddChecklistTemplateAsync(template);

        return task;
    }

    public async System.Threading.Tasks.Task SetChecklistTaskStatusAsync(Guid eventId, Guid taskId, ChecklistTaskStatus status)
    {
        var task = await GetChecklistTaskForEventOrThrowAsync(eventId, taskId);
        task.SetStatus(status);
        await _taskRepository.UpdateAsync(task);
    }

    public async System.Threading.Tasks.Task DesignateTaskForAttendeesAsync(Guid eventId, Guid taskId)
    {
        var task = await GetChecklistTaskForEventOrThrowAsync(eventId, taskId);
        task.DesignateForAttendees();
        await _taskRepository.UpdateAsync(task);
    }

    public async Task<IEnumerable<ChecklistTaskAggregate>> GetVisibleTasksForConfirmedAttendeeAsync(Guid eventId, Guid attendeeId)
    {
        var invitation = await _invitationRepository.GetByEventAndAttendeeAsync(eventId, attendeeId);
        if (invitation is null || !invitation.IsWillAttend)
        {
            throw new DomainException($"Attendee {attendeeId} has not confirmed attendance for event {eventId}.");
        }

        var tasks = await _taskRepository.GetAttendeeAccomplishableByEventIdAsync(eventId);
        return tasks.Where(t => !t.IsNotApplicable);
    }

    public async System.Threading.Tasks.Task SelectTaskForAttendeeAsync(Guid eventId, Guid taskId, Guid attendeeId)
    {
        var invitation = await _invitationRepository.GetByEventAndAttendeeAsync(eventId, attendeeId);
        if (invitation is null || !invitation.IsWillAttend)
        {
            throw new DomainException($"Attendee {attendeeId} has not confirmed attendance for event {eventId}.");
        }

        var task = await GetChecklistTaskForEventOrThrowAsync(eventId, taskId);
        task.SelectByAttendee(attendeeId);
        await _taskRepository.UpdateAsync(task);
    }

    public async Task<EventSummary> GetEventSummaryAsync(Guid eventId)
    {
        var eventAggregate = await GetEventOrThrowAsync(eventId);
        var invitations = (await _invitationRepository.GetByEventIdAsync(eventId)).ToList();
        var tasks = (await _taskRepository.GetByEventIdAsync(eventId)).ToList();

        return new EventSummary(
            eventAggregate,
            invitations.Count(i => i.HasResponded),
            invitations.Count(i => i.IsUnreplied),
            invitations.Where(i => i.IsWillAttend).ToList().AsReadOnly(),
            invitations.Where(i => i.IsMaybeWillAttend).ToList().AsReadOnly(),
            invitations.Where(i => i.IsCannotAttend).ToList().AsReadOnly(),
            tasks.Count(t => t.IsDone),
            tasks.Count,
            tasks.Where(t => t.IsAttendeeAccomplishable && t.SelectedByAttendeeId.HasValue).ToList().AsReadOnly());
    }

    private async Task<EventAggregate> GetEventOrThrowAsync(Guid eventId)
    {
        return await _eventRepository.GetByIdAsync(eventId)
            ?? throw new DomainException($"Event with ID {eventId} not found.");
    }

    private async Task<ChecklistTaskAggregate> GetChecklistTaskForEventOrThrowAsync(Guid eventId, Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId)
            ?? throw new DomainException($"Checklist task with ID {taskId} not found.");

        if (task.EventId != eventId)
        {
            throw new DomainException($"Checklist task {taskId} does not belong to event {eventId}.");
        }

        return task;
    }
}

public record EventSummary(
    EventAggregate Event,
    int RepliedInvitations,
    int UnrepliedInvitations,
    IReadOnlyList<Invitation.InvitationAggregate> ConfirmedAttendees,
    IReadOnlyList<Invitation.InvitationAggregate> TentativeAttendees,
    IReadOnlyList<Invitation.InvitationAggregate> DeclinedAttendees,
    int CompletedTasks,
    int TotalTasks,
    IReadOnlyList<ChecklistTaskAggregate> AttendeeTaskSelections
);



