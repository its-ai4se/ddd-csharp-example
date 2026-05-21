using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Task;
using CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class TaskManagementTests
{
    private static readonly Address ValidAddress = new("Jl. A", "Jakarta", "DKI", "10110", "Indonesia");
    private static readonly DateTimeRange ValidRange = new(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(4));

    private async Task<(EventManagementService service, FakeEventTypeRepository eventTypeRepo, FakeTaskRepository taskRepo, Guid eventId)> SetupAsync(string eventTypeName = "Birthday Party")
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventTypeRepo = new FakeEventTypeRepository();
        var taskRepo = new FakeTaskRepository();
        var eventRepo = new FakeEventRepository();
        var service = new EventManagementService(eventRepo, persons, new FakeInvitationRepository(), taskRepo, eventTypeRepo, new FakeLocationRepository());

        var location = new Location("Aula A", ValidAddress);
        var organizers = new[] { new EventOrganizer(organizer.Id, false) };
        var ev = await service.CreateEventAsync("Test Event", new EventType(eventTypeName), ValidRange, location, organizers);

        return (service, eventTypeRepo, taskRepo, ev.Id);
    }

    // TM-001: Organizer marks task as 'Needs to be Done'
    [Fact]
    public async System.Threading.Tasks.Task TM001_SetTaskStatus_NeedsToBeDone_StatusIsUpdated()
    {
        var (service, _, taskRepo, eventId) = await SetupAsync();
        var task = await service.AddChecklistTaskAsync(eventId, "Beli kue ulang tahun");

        await service.SetChecklistTaskStatusAsync(eventId, task.Id, ChecklistTaskStatus.NeedsToBeDone);

        var updated = await taskRepo.GetByIdAsync(task.Id);
        Assert.Equal(ChecklistTaskStatus.NeedsToBeDone, updated!.Status);
    }

    // TM-002: Organizer marks task as 'Has Been Done'
    [Fact]
    public async System.Threading.Tasks.Task TM002_SetTaskStatus_Done_StatusIsUpdated()
    {
        var (service, _, taskRepo, eventId) = await SetupAsync();
        var task = await service.AddChecklistTaskAsync(eventId, "Beli kue ulang tahun");

        await service.SetChecklistTaskStatusAsync(eventId, task.Id, ChecklistTaskStatus.Done);

        var updated = await taskRepo.GetByIdAsync(task.Id);
        Assert.Equal(ChecklistTaskStatus.Done, updated!.Status);
        Assert.True(updated.IsDone);
    }

    // TM-003: Organizer marks task as 'Not Applicable'
    [Fact]
    public async System.Threading.Tasks.Task TM003_SetTaskStatus_NotApplicable_StatusIsUpdated()
    {
        var (service, _, taskRepo, eventId) = await SetupAsync();
        var task = await service.AddChecklistTaskAsync(eventId, "Sewa photobooth");

        await service.SetChecklistTaskStatusAsync(eventId, task.Id, ChecklistTaskStatus.NotApplicable);

        var updated = await taskRepo.GetByIdAsync(task.Id);
        Assert.Equal(ChecklistTaskStatus.NotApplicable, updated!.Status);
        Assert.True(updated.IsNotApplicable);
    }

    // TM-004: Organizer adds a new task to the current event's checklist
    [Fact]
    public async System.Threading.Tasks.Task TM004_AddChecklistTask_NewTask_IsAddedToEventChecklist()
    {
        var (service, _, taskRepo, eventId) = await SetupAsync();

        var task = await service.AddChecklistTaskAsync(eventId, "Siapkan lilin ulang tahun");

        Assert.NotNull(task);
        Assert.Equal("Siapkan lilin ulang tahun", task.Title);
        var tasks = (await taskRepo.GetByEventIdAsync(eventId)).ToList();
        Assert.Contains(tasks, t => t.Title == "Siapkan lilin ulang tahun");
    }

    // TM-005: New task added to Birthday Party appears in next Birthday Party event's checklist
    [Fact]
    public async System.Threading.Tasks.Task TM005_AddChecklistTask_NewTask_AvailableForFutureEventsOfSameType()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice5@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventTypeRepo = new FakeEventTypeRepository();
        var taskRepo = new FakeTaskRepository();
        var eventRepo = new FakeEventRepository();
        var service = new EventManagementService(eventRepo, persons, new FakeInvitationRepository(), taskRepo, eventTypeRepo, new FakeLocationRepository());

        var location = new Location("Aula A", ValidAddress);
        var organizers = new[] { new EventOrganizer(organizer.Id, false) };

        // Create first Birthday Party event and add a new task
        var ev1 = await service.CreateEventAsync("Birthday 1", new EventType("Birthday Party"), ValidRange, location, organizers);
        await service.AddChecklistTaskAsync(ev1.Id, "Siapkan lilin ulang tahun");

        // Create second Birthday Party event — new task should appear automatically
        var ev2 = await service.CreateEventAsync("Birthday 2", new EventType("Birthday Party"), ValidRange, location, organizers);

        var tasks2 = (await taskRepo.GetByEventIdAsync(ev2.Id)).ToList();
        Assert.Contains(tasks2, t => t.Title == "Siapkan lilin ulang tahun");
    }

    // TM-006: New task added to Birthday Party does NOT appear in Graduation Party checklist
    [Fact]
    public async System.Threading.Tasks.Task TM006_AddChecklistTask_NewTask_NotAvailableForDifferentEventType()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice6@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventTypeRepo = new FakeEventTypeRepository();
        var taskRepo = new FakeTaskRepository();
        var eventRepo = new FakeEventRepository();
        var service = new EventManagementService(eventRepo, persons, new FakeInvitationRepository(), taskRepo, eventTypeRepo, new FakeLocationRepository());

        var location = new Location("Aula A", ValidAddress);
        var organizers = new[] { new EventOrganizer(organizer.Id, false) };

        var birthdayEv = await service.CreateEventAsync("Birthday", new EventType("Birthday Party"), ValidRange, location, organizers);
        await service.AddChecklistTaskAsync(birthdayEv.Id, "Siapkan lilin ulang tahun");

        var graduationEv = await service.CreateEventAsync("Graduation", new EventType("Graduation Party"), ValidRange, location, organizers);

        var graduationTasks = (await taskRepo.GetByEventIdAsync(graduationEv.Id)).ToList();
        Assert.DoesNotContain(graduationTasks, t => t.Title == "Siapkan lilin ulang tahun");
    }

    // TM-007: Organizer designates a task for attendees
    [Fact]
    public async System.Threading.Tasks.Task TM007_DesignateTaskForAttendees_TaskIsMarkedAttendeeAccomplishable()
    {
        var (service, _, taskRepo, eventId) = await SetupAsync();
        var task = await service.AddChecklistTaskAsync(eventId, "Bawa kue ulang tahun");

        await service.DesignateTaskForAttendeesAsync(eventId, task.Id);

        var updated = await taskRepo.GetByIdAsync(task.Id);
        Assert.True(updated!.IsAttendeeAccomplishable);
    }

    // TM-008: Task not designated for attendees is only visible to organizer
    [Fact]
    public async System.Threading.Tasks.Task TM008_Task_NotDesignatedForAttendees_IsNotAttendeeAccomplishable()
    {
        var (service, _, taskRepo, eventId) = await SetupAsync();
        var task = await service.AddChecklistTaskAsync(eventId, "Beli dekorasi");

        // Not calling DesignateTaskForAttendeesAsync
        var stored = await taskRepo.GetByIdAsync(task.Id);
        Assert.False(stored!.IsAttendeeAccomplishable);

        var attendeeTasks = (await taskRepo.GetAttendeeAccomplishableByEventIdAsync(eventId)).ToList();
        Assert.DoesNotContain(attendeeTasks, t => t.Id == task.Id);
    }
}
