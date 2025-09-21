using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Event.Repositories;
using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Invitation.Repositories;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Person.Repositories;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.Services;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Task;
using CelebrationOrganizationSystem.Domain.Task.Repositories;

namespace CelebrationOrganizationSystem.Domain.Services;

public class EventManagementService : DomainServiceBase
{
    private readonly IEventRepository _eventRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly ITaskRepository _taskRepository;

    public EventManagementService(
        IClock clock,
        IEventRepository eventRepository,
        IPersonRepository personRepository,
        IInvitationRepository invitationRepository,
        ITaskRepository taskRepository) : base(clock)
    {
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
    }

    public async System.Threading.Tasks.Task<EventAggregate> CreateEventAsync(
        string occasion,
        EventType eventType,
        DateTimeRange dateTimeRange,
        Location location,
        Guid organizerId)
    {
        // Validate organizer exists and has organizer role
        var organizer = await _personRepository.GetByIdAsync(organizerId);
        if (organizer == null)
        {
            throw new DomainException($"Organizer with ID {organizerId} not found.");
        }

        if (!organizer.IsOrganizer)
        {
            throw new DomainException($"Person {organizerId} is not an organizer.");
        }

        // Validate event is in the future
        if (dateTimeRange.StartDateTime <= Clock.UtcNow)
        {
            throw new DomainException("Event must be scheduled in the future.");
        }

        var eventAggregate = new EventAggregate(occasion, eventType, dateTimeRange, location, organizerId);
        await _eventRepository.AddAsync(eventAggregate);

        return eventAggregate;
    }

    public async System.Threading.Tasks.Task<InvitationAggregate> InviteAttendeeAsync(
        Guid eventId,
        PersonName attendeeName,
        EmailAddress attendeeEmail)
    {
        var eventAggregate = await _eventRepository.GetByIdAsync(eventId);
        if (eventAggregate == null)
        {
            throw new DomainException($"Event with ID {eventId} not found.");
        }

        // Check if event is in the past
        if (eventAggregate.IsEventInPast())
        {
            throw new DomainException("Cannot invite attendees to past events.");
        }

        // Check if person already exists with this email
        var existingPerson = await _personRepository.GetByEmailAsync(attendeeEmail.Value);
        Guid attendeeId;

        if (existingPerson != null)
        {
            attendeeId = existingPerson.Id;
            // Ensure they have attendee role
            if (!existingPerson.IsAttendee)
            {
                existingPerson.AddRole(new AttendeeRole(existingPerson.Id));
                await _personRepository.UpdateAsync(existingPerson);
            }
        }
        else
        {
            // Create new person with attendee role
            var newPerson = new PersonAggregate(
                attendeeName,
                new Address("TBD", "TBD", "TBD", "TBD", "TBD"), // Placeholder address
                new PhoneNumber("0000000000"), // Placeholder phone
                attendeeEmail,
                new Password("TempPassword123!") // Temporary password
            );
            newPerson.AddRole(new AttendeeRole(newPerson.Id));
            await _personRepository.AddAsync(newPerson);
            attendeeId = newPerson.Id;
        }

        // Check if invitation already exists
        var existingInvitation = await _invitationRepository.GetByEventAndAttendeeAsync(eventId, attendeeId);
        if (existingInvitation != null)
        {
            throw new DomainException("Attendee has already been invited to this event.");
        }

        var invitation = new InvitationAggregate(eventId, attendeeId, attendeeEmail, attendeeName);
        await _invitationRepository.AddAsync(invitation);

        // Add attendee to event
        eventAggregate.AddAttendee(attendeeId);
        await _eventRepository.UpdateAsync(eventAggregate);

        return invitation;
    }

    public async System.Threading.Tasks.Task<TaskAggregate> CreateTaskAsync(
        Guid eventId,
        string title,
        string? description = null,
        TaskType type = TaskType.General)
    {
        var eventAggregate = await _eventRepository.GetByIdAsync(eventId);
        if (eventAggregate == null)
        {
            throw new DomainException($"Event with ID {eventId} not found.");
        }

        var task = new TaskAggregate(title, description, type);
        await _taskRepository.AddAsync(task);

        // Add task to event
        eventAggregate.AddTask(task.Id);
        await _eventRepository.UpdateAsync(eventAggregate);

        return task;
    }

    public async System.Threading.Tasks.Task AssignTaskToAttendeeAsync(Guid taskId, Guid attendeeId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            throw new DomainException($"Task with ID {taskId} not found.");
        }

        var attendee = await _personRepository.GetByIdAsync(attendeeId);
        if (attendee == null)
        {
            throw new DomainException($"Attendee with ID {attendeeId} not found.");
        }

        if (!attendee.IsAttendee)
        {
            throw new DomainException($"Person {attendeeId} is not an attendee.");
        }

        task.AssignToAttendee(attendeeId);
        await _taskRepository.UpdateAsync(task);
    }

    public async System.Threading.Tasks.Task<EventSummary> GetEventSummaryAsync(Guid eventId)
    {
        var eventAggregate = await _eventRepository.GetByIdAsync(eventId);
        if (eventAggregate == null)
        {
            throw new DomainException($"Event with ID {eventId} not found.");
        }

        var invitations = await _invitationRepository.GetByEventIdAsync(eventId);
        var tasks = await _taskRepository.GetByEventIdAsync(eventId);

        var acceptedCount = invitations.Count(i => i.IsAccepted);
        var maybeCount = invitations.Count(i => i.IsMaybe);
        var declinedCount = invitations.Count(i => i.IsDeclined);
        var pendingCount = invitations.Count(i => i.IsPending);

        var completedTasks = tasks.Count(t => t.IsCompleted);
        var totalTasks = tasks.Count();

        return new EventSummary(
            eventAggregate,
            acceptedCount,
            maybeCount,
            declinedCount,
            pendingCount,
            completedTasks,
            totalTasks
        );
    }
}

public record EventSummary(
    EventAggregate Event,
    int AcceptedInvitations,
    int MaybeInvitations,
    int DeclinedInvitations,
    int PendingInvitations,
    int CompletedTasks,
    int TotalTasks
);
