using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Event.Repositories;
using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Invitation.Repositories;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Person.Repositories;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.Services;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Task;
using CelebrationOrganizationSystem.Domain.Task.Repositories;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.Services;

public class EventManagementServiceTests
{
    private readonly MockClock _mockClock;
    private readonly MockEventRepository _mockEventRepository;
    private readonly MockPersonRepository _mockPersonRepository;
    private readonly MockInvitationRepository _mockInvitationRepository;
    private readonly MockTaskRepository _mockTaskRepository;
    private readonly EventManagementService _service;

    public EventManagementServiceTests()
    {
        _mockClock = new MockClock();
        _mockEventRepository = new MockEventRepository();
        _mockPersonRepository = new MockPersonRepository();
        _mockInvitationRepository = new MockInvitationRepository();
        _mockTaskRepository = new MockTaskRepository();
        _service = new EventManagementService(
            _mockClock,
            _mockEventRepository,
            _mockPersonRepository,
            _mockInvitationRepository,
            _mockTaskRepository);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateEventAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var organizer = CreateValidPerson(organizerId);
        organizer.AddRole(new OrganizerRole(organizerId));
        _mockPersonRepository.AddPerson(organizer);

        var occasion = "Sarah's Birthday";
        var eventType = new EventType("Birthday Party");
        var dateTimeRange = new DateTimeRange(
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(7).AddHours(4)
        );
        var location = new Location("Community Center", new Address("123 Main St", "Anytown", "CA", "12345", "USA"));

        // Act
        var result = await _service.CreateEventAsync(occasion, eventType, dateTimeRange, location, organizerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(occasion, result.Occasion);
        Assert.Equal(eventType, result.EventType);
        Assert.Equal(dateTimeRange, result.DateTimeRange);
        Assert.Equal(location, result.Location);
        Assert.Equal(organizerId, result.OrganizerId);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateEventAsync_WithNonExistentOrganizer_ShouldThrowException()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var occasion = "Sarah's Birthday";
        var eventType = new EventType("Birthday Party");
        var dateTimeRange = new DateTimeRange(
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(7).AddHours(4)
        );
        var location = new Location("Community Center", new Address("123 Main St", "Anytown", "CA", "12345", "USA"));

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _service.CreateEventAsync(occasion, eventType, dateTimeRange, location, organizerId));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateEventAsync_WithNonOrganizer_ShouldThrowException()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var organizer = CreateValidPerson(organizerId);
        // Don't add organizer role
        _mockPersonRepository.AddPerson(organizer);

        var occasion = "Sarah's Birthday";
        var eventType = new EventType("Birthday Party");
        var dateTimeRange = new DateTimeRange(
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(7).AddHours(4)
        );
        var location = new Location("Community Center", new Address("123 Main St", "Anytown", "CA", "12345", "USA"));

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _service.CreateEventAsync(occasion, eventType, dateTimeRange, location, organizerId));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateEventAsync_WithPastEvent_ShouldThrowException()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var organizer = CreateValidPerson(organizerId);
        organizer.AddRole(new OrganizerRole(organizerId));
        _mockPersonRepository.AddPerson(organizer);

        var occasion = "Sarah's Birthday";
        var eventType = new EventType("Birthday Party");
        var dateTimeRange = new DateTimeRange(
            DateTime.UtcNow.AddDays(-7), // Past event
            DateTime.UtcNow.AddDays(-7).AddHours(4)
        );
        var location = new Location("Community Center", new Address("123 Main St", "Anytown", "CA", "12345", "USA"));

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _service.CreateEventAsync(occasion, eventType, dateTimeRange, location, organizerId));
    }

    [Fact]
    public async System.Threading.Tasks.Task InviteAttendeeAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventAggregate = CreateValidEvent(eventId);
        _mockEventRepository.AddEvent(eventAggregate);

        var attendeeName = new PersonName("Jane", "Doe");
        var attendeeEmail = new EmailAddress("jane.doe@email.com");

        // Act
        var result = await _service.InviteAttendeeAsync(eventId, attendeeName, attendeeEmail);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventId, result.EventId);
        Assert.Equal(attendeeEmail, result.AttendeeEmail);
        Assert.Equal(attendeeName, result.AttendeeName);
    }

    [Fact]
    public async System.Threading.Tasks.Task InviteAttendeeAsync_WithNonExistentEvent_ShouldThrowException()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var attendeeName = new PersonName("Jane", "Doe");
        var attendeeEmail = new EmailAddress("jane.doe@email.com");

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => 
            _service.InviteAttendeeAsync(eventId, attendeeName, attendeeEmail));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventAggregate = CreateValidEvent(eventId);
        _mockEventRepository.AddEvent(eventAggregate);

        var title = "Bring Birthday Cake";
        var description = "A delicious chocolate cake";
        var type = TaskType.Food;

        // Act
        var result = await _service.CreateTaskAsync(eventId, title, description, type);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(title, result.Title);
        Assert.Equal(description, result.Description);
        Assert.Equal(type, result.Type);
    }

    [Fact]
    public async System.Threading.Tasks.Task AssignTaskToAttendeeAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var attendeeId = Guid.NewGuid();
        var task = new TaskAggregate(taskId, "Test Task");
        var attendee = CreateValidPerson(attendeeId);
        attendee.AddRole(new AttendeeRole(attendeeId));

        _mockTaskRepository.AddTask(task);
        _mockPersonRepository.AddPerson(attendee);

        // Act
        await _service.AssignTaskToAttendeeAsync(taskId, attendeeId);

        // Assert
        Assert.Equal(attendeeId, task.AssignedToAttendeeId);
        Assert.True(task.IsAssigned);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetEventSummaryAsync_WithValidEvent_ShouldReturnSummary()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventAggregate = CreateValidEvent(eventId);
        _mockEventRepository.AddEvent(eventAggregate);

        // Add some invitations
        var invitation1 = new InvitationAggregate(eventId, Guid.NewGuid(), new EmailAddress("test1@email.com"), new PersonName("Test", "One"));
        invitation1.RespondToInvitation(InvitationStatus.Accepted);
        _mockInvitationRepository.AddInvitation(invitation1);

        var invitation2 = new InvitationAggregate(eventId, Guid.NewGuid(), new EmailAddress("test2@email.com"), new PersonName("Test", "Two"));
        invitation2.RespondToInvitation(InvitationStatus.Maybe);
        _mockInvitationRepository.AddInvitation(invitation2);

        // Add some tasks
        var task1 = new TaskAggregate("Task 1");
        task1.MarkAsCompleted();
        _mockTaskRepository.AddTask(task1);

        var task2 = new TaskAggregate("Task 2");
        _mockTaskRepository.AddTask(task2);

        // Act
        var result = await _service.GetEventSummaryAsync(eventId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventAggregate, result.Event);
        Assert.Equal(1, result.AcceptedInvitations);
        Assert.Equal(1, result.MaybeInvitations);
        Assert.Equal(0, result.DeclinedInvitations);
        Assert.Equal(0, result.PendingInvitations);
        Assert.Equal(1, result.CompletedTasks);
        Assert.Equal(2, result.TotalTasks);
    }

    private PersonAggregate CreateValidPerson(Guid id)
    {
        var name = new PersonName("John", "Doe");
        var address = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
        var phoneNumber = new PhoneNumber("555-123-4567");
        var emailAddress = new EmailAddress("john.doe@email.com");
        var password = new Password("SecurePassword123!");

        return new PersonAggregate(id, name, address, phoneNumber, emailAddress, password);
    }

    private EventAggregate CreateValidEvent(Guid eventId)
    {
        var eventType = new EventType("Birthday Party");
        var dateTimeRange = new DateTimeRange(
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(7).AddHours(4)
        );
        var location = new Location("Community Center", new Address("123 Main St", "Anytown", "CA", "12345", "USA"));
        var organizerId = Guid.NewGuid();

        return new EventAggregate(eventId, "Test Event", eventType, dateTimeRange, location, organizerId);
    }
}

// Mock implementations for testing
public class MockClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
}

public class MockEventRepository : IEventRepository
{
    private readonly Dictionary<Guid, EventAggregate> _events = new();

    public void AddEvent(EventAggregate eventAggregate) => _events[eventAggregate.Id] = eventAggregate;

    public System.Threading.Tasks.Task<EventAggregate?> GetByIdAsync(Guid id) => 
        System.Threading.Tasks.Task.FromResult(_events.TryGetValue(id, out var eventAggregate) ? eventAggregate : null);

    public System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetAllAsync() => 
        System.Threading.Tasks.Task.FromResult(_events.Values.AsEnumerable());

    public System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetByOrganizerIdAsync(Guid organizerId) => 
        System.Threading.Tasks.Task.FromResult(_events.Values.Where(e => e.OrganizerId == organizerId));

    public System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetByAttendeeIdAsync(Guid attendeeId) => 
        System.Threading.Tasks.Task.FromResult(_events.Values.Where(e => e.AttendeeIds.Contains(attendeeId)));

    public System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetByDateRangeAsync(DateTime startDate, DateTime endDate) => 
        System.Threading.Tasks.Task.FromResult(_events.Values.Where(e => e.DateTimeRange.StartDateTime >= startDate && e.DateTimeRange.EndDateTime <= endDate));

    public System.Threading.Tasks.Task<IEnumerable<EventAggregate>> GetByEventTypeAsync(string eventTypeName) => 
        System.Threading.Tasks.Task.FromResult(_events.Values.Where(e => e.EventType.Name == eventTypeName));

    public System.Threading.Tasks.Task AddAsync(EventAggregate eventAggregate) => 
        System.Threading.Tasks.Task.Run(() => AddEvent(eventAggregate));

    public System.Threading.Tasks.Task UpdateAsync(EventAggregate eventAggregate) => 
        System.Threading.Tasks.Task.Run(() => AddEvent(eventAggregate));

    public System.Threading.Tasks.Task DeleteAsync(Guid id) => 
        System.Threading.Tasks.Task.Run(() => _events.Remove(id));

    public System.Threading.Tasks.Task<bool> ExistsAsync(Guid id) => 
        System.Threading.Tasks.Task.FromResult(_events.ContainsKey(id));
}

public class MockPersonRepository : IPersonRepository
{
    private readonly Dictionary<Guid, PersonAggregate> _persons = new();
    private readonly Dictionary<string, PersonAggregate> _personsByEmail = new();

    public void AddPerson(PersonAggregate person)
    {
        _persons[person.Id] = person;
        if (person.EmailAddress != null)
            _personsByEmail[person.EmailAddress.Value] = person;
    }

    public System.Threading.Tasks.Task<PersonAggregate?> GetByIdAsync(Guid id) => 
        System.Threading.Tasks.Task.FromResult(_persons.TryGetValue(id, out var person) ? person : null);

    public System.Threading.Tasks.Task<PersonAggregate?> GetByEmailAsync(string email) => 
        System.Threading.Tasks.Task.FromResult(_personsByEmail.TryGetValue(email, out var person) ? person : null);

    public System.Threading.Tasks.Task<IEnumerable<PersonAggregate>> GetAllAsync() => 
        System.Threading.Tasks.Task.FromResult(_persons.Values.AsEnumerable());

    public System.Threading.Tasks.Task<IEnumerable<PersonAggregate>> GetByRoleAsync<T>() where T : UserRole => 
        System.Threading.Tasks.Task.FromResult(_persons.Values.Where(p => p.HasRole<T>()));

    public System.Threading.Tasks.Task AddAsync(PersonAggregate person) => 
        System.Threading.Tasks.Task.Run(() => AddPerson(person));

    public System.Threading.Tasks.Task UpdateAsync(PersonAggregate person) => 
        System.Threading.Tasks.Task.Run(() => AddPerson(person));

    public System.Threading.Tasks.Task DeleteAsync(Guid id) => 
        System.Threading.Tasks.Task.Run(() => _persons.Remove(id));

    public System.Threading.Tasks.Task<bool> ExistsAsync(Guid id) => 
        System.Threading.Tasks.Task.FromResult(_persons.ContainsKey(id));

    public System.Threading.Tasks.Task<bool> ExistsByEmailAsync(string email) => 
        System.Threading.Tasks.Task.FromResult(_personsByEmail.ContainsKey(email));
}

public class MockInvitationRepository : IInvitationRepository
{
    private readonly Dictionary<Guid, InvitationAggregate> _invitations = new();

    public void AddInvitation(InvitationAggregate invitation) => _invitations[invitation.Id] = invitation;

    public System.Threading.Tasks.Task<InvitationAggregate?> GetByIdAsync(Guid id) => 
        System.Threading.Tasks.Task.FromResult(_invitations.TryGetValue(id, out var invitation) ? invitation : null);

    public System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetAllAsync() => 
        System.Threading.Tasks.Task.FromResult(_invitations.Values.AsEnumerable());

    public System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetByEventIdAsync(Guid eventId) => 
        System.Threading.Tasks.Task.FromResult(_invitations.Values.Where(i => i.EventId == eventId));

    public System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetByAttendeeIdAsync(Guid attendeeId) => 
        System.Threading.Tasks.Task.FromResult(_invitations.Values.Where(i => i.AttendeeId == attendeeId));

    public System.Threading.Tasks.Task<InvitationAggregate?> GetByEventAndAttendeeAsync(Guid eventId, Guid attendeeId) => 
        System.Threading.Tasks.Task.FromResult(_invitations.Values.FirstOrDefault(i => i.EventId == eventId && i.AttendeeId == attendeeId));

    public System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetByStatusAsync(InvitationStatus status) => 
        System.Threading.Tasks.Task.FromResult(_invitations.Values.Where(i => i.Response?.Status == status));

    public System.Threading.Tasks.Task AddAsync(InvitationAggregate invitation) => 
        System.Threading.Tasks.Task.Run(() => AddInvitation(invitation));

    public System.Threading.Tasks.Task UpdateAsync(InvitationAggregate invitation) => 
        System.Threading.Tasks.Task.Run(() => AddInvitation(invitation));

    public System.Threading.Tasks.Task DeleteAsync(Guid id) => 
        System.Threading.Tasks.Task.Run(() => _invitations.Remove(id));

    public System.Threading.Tasks.Task<bool> ExistsAsync(Guid id) => 
        System.Threading.Tasks.Task.FromResult(_invitations.ContainsKey(id));

    public System.Threading.Tasks.Task<bool> ExistsByEventAndAttendeeAsync(Guid eventId, Guid attendeeId) => 
        System.Threading.Tasks.Task.FromResult(_invitations.Values.Any(i => i.EventId == eventId && i.AttendeeId == attendeeId));
}

public class MockTaskRepository : ITaskRepository
{
    private readonly Dictionary<Guid, TaskAggregate> _tasks = new();

    public void AddTask(TaskAggregate task) => _tasks[task.Id] = task;

    public System.Threading.Tasks.Task<TaskAggregate?> GetByIdAsync(Guid id) => 
        System.Threading.Tasks.Task.FromResult(_tasks.TryGetValue(id, out var task) ? task : null);

    public System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetAllAsync() => 
        System.Threading.Tasks.Task.FromResult(_tasks.Values.AsEnumerable());

    public System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetByEventIdAsync(Guid eventId) => 
        System.Threading.Tasks.Task.FromResult(_tasks.Values.AsEnumerable());

    public System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetByAssignedAttendeeIdAsync(Guid attendeeId) => 
        System.Threading.Tasks.Task.FromResult(_tasks.Values.Where(t => t.AssignedToAttendeeId == attendeeId));

    public System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetByStatusAsync(CelebrationOrganizationSystem.Domain.Task.TaskStatus status) => 
        System.Threading.Tasks.Task.FromResult(_tasks.Values.Where(t => t.Status == status));

    public System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetByTypeAsync(CelebrationOrganizationSystem.Domain.Task.TaskType type) => 
        System.Threading.Tasks.Task.FromResult(_tasks.Values.Where(t => t.Type == type));

    public System.Threading.Tasks.Task<IEnumerable<TaskAggregate>> GetAvailableTasksAsync() => 
        System.Threading.Tasks.Task.FromResult(_tasks.Values.Where(t => !t.IsAssigned));

    public System.Threading.Tasks.Task AddAsync(TaskAggregate task) => 
        System.Threading.Tasks.Task.Run(() => AddTask(task));

    public System.Threading.Tasks.Task UpdateAsync(TaskAggregate task) => 
        System.Threading.Tasks.Task.Run(() => AddTask(task));

    public System.Threading.Tasks.Task DeleteAsync(Guid id) => 
        System.Threading.Tasks.Task.Run(() => _tasks.Remove(id));

    public System.Threading.Tasks.Task<bool> ExistsAsync(Guid id) => 
        System.Threading.Tasks.Task.FromResult(_tasks.ContainsKey(id));
}
