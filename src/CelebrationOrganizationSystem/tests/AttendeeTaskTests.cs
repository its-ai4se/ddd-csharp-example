using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Task;
using CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class AttendeeTaskTests
{
    private static readonly Address ValidAddress = new("Jl. A", "Jakarta", "DKI", "10110", "Indonesia");
    private static readonly DateTimeRange ValidRange = new(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(4));

    private async Task<(EventManagementService service, InvitationService invitationService, FakeInvitationRepository invitationRepo, FakeTaskRepository taskRepo, Guid eventId, Guid attendeeTaskId)> SetupAsync()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventRepo = new FakeEventRepository();
        var invitationRepo = new FakeInvitationRepository();
        var taskRepo = new FakeTaskRepository();
        var eventTypeRepo = new FakeEventTypeRepository();

        var service = new EventManagementService(eventRepo, persons, invitationRepo, taskRepo, eventTypeRepo, new FakeLocationRepository());
        var invitationService = new InvitationService(eventRepo, invitationRepo);

        var location = new Location("Aula A", ValidAddress);
        var organizers = new[] { new EventOrganizer(organizer.Id, false) };
        var ev = await service.CreateEventAsync("Test Event", new EventType("Birthday Party"), ValidRange, location, organizers);

        // Add an attendee-designated task
        var task = await service.AddChecklistTaskAsync(ev.Id, "Bawa kue ulang tahun");
        await service.DesignateTaskForAttendeesAsync(ev.Id, task.Id);

        return (service, invitationService, invitationRepo, taskRepo, ev.Id, task.Id);
    }

    private async Task<Guid> InviteAndConfirmAttendeeAsync(InvitationService invitationService, FakeInvitationRepository invitationRepo, Guid eventId, string email)
    {
        var invitation = await invitationService.InviteAttendeeAsync(eventId, new PersonName("Budi", "Santoso"), new EmailAddress(email));
        // Link to a fake attendee ID
        var attendeeId = Guid.NewGuid();
        invitation.LinkToAttendee(attendeeId);
        await invitationRepo.UpdateAsync(invitation);
        await invitationService.RespondToInvitationAsync(invitation.Id, InvitationStatus.WillAttend);
        return attendeeId;
    }

    private async Task<Guid> InviteAttendeeWithStatusAsync(InvitationService invitationService, FakeInvitationRepository invitationRepo, Guid eventId, string email, InvitationStatus? status)
    {
        var invitation = await invitationService.InviteAttendeeAsync(eventId, new PersonName("Guest", "User"), new EmailAddress(email));
        var attendeeId = Guid.NewGuid();
        invitation.LinkToAttendee(attendeeId);
        await invitationRepo.UpdateAsync(invitation);
        if (status.HasValue)
            await invitationService.RespondToInvitationAsync(invitation.Id, status.Value);
        return attendeeId;
    }

    // AT-001: Confirmed attendee (WillAttend) can see the list of attendee-designated tasks
    [Fact]
    public async System.Threading.Tasks.Task AT001_GetVisibleTasks_ConfirmedAttendee_CanSeeTasks()
    {
        var (service, invitationService, invitationRepo, _, eventId, _) = await SetupAsync();
        var attendeeId = await InviteAndConfirmAttendeeAsync(invitationService, invitationRepo, eventId, "budi@mail.com");

        var tasks = (await service.GetVisibleTasksForConfirmedAttendeeAsync(eventId, attendeeId)).ToList();

        Assert.NotEmpty(tasks);
        Assert.Contains(tasks, t => t.Title == "Bawa kue ulang tahun");
    }

    // AT-002: Attendee with 'Maybe Will Attend' cannot see the task list
    [Fact]
    public async System.Threading.Tasks.Task AT002_GetVisibleTasks_MaybeAttendee_ThrowsDomainException()
    {
        var (service, invitationService, invitationRepo, _, eventId, _) = await SetupAsync();
        var attendeeId = await InviteAttendeeWithStatusAsync(invitationService, invitationRepo, eventId, "maybe@mail.com", InvitationStatus.MaybeWillAttend);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.GetVisibleTasksForConfirmedAttendeeAsync(eventId, attendeeId));
    }

    // AT-003: Attendee who declined cannot see the task list
    [Fact]
    public async System.Threading.Tasks.Task AT003_GetVisibleTasks_DeclinedAttendee_ThrowsDomainException()
    {
        var (service, invitationService, invitationRepo, _, eventId, _) = await SetupAsync();
        var attendeeId = await InviteAttendeeWithStatusAsync(invitationService, invitationRepo, eventId, "declined@mail.com", InvitationStatus.CannotAttend);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.GetVisibleTasksForConfirmedAttendeeAsync(eventId, attendeeId));
    }

    // AT-004: Attendee who has not responded cannot see the task list
    [Fact]
    public async System.Threading.Tasks.Task AT004_GetVisibleTasks_UnrepliedAttendee_ThrowsDomainException()
    {
        var (service, invitationService, invitationRepo, _, eventId, _) = await SetupAsync();
        var attendeeId = await InviteAttendeeWithStatusAsync(invitationService, invitationRepo, eventId, "unreplied@mail.com", null);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.GetVisibleTasksForConfirmedAttendeeAsync(eventId, attendeeId));
    }

    // AT-005: Confirmed attendee selects a task they will accomplish
    [Fact]
    public async System.Threading.Tasks.Task AT005_SelectTask_ConfirmedAttendee_TaskIsRegisteredUnderAttendee()
    {
        var (service, invitationService, invitationRepo, taskRepo, eventId, taskId) = await SetupAsync();
        var attendeeId = await InviteAndConfirmAttendeeAsync(invitationService, invitationRepo, eventId, "budi2@mail.com");

        await service.SelectTaskForAttendeeAsync(eventId, taskId, attendeeId);

        var task = await taskRepo.GetByIdAsync(taskId);
        Assert.Equal(attendeeId, task!.SelectedByAttendeeId);
    }

    // AT-006: Organizer can see which tasks have been selected by which attendees
    [Fact]
    public async System.Threading.Tasks.Task AT006_GetEventSummary_ShowsAttendeeTaskSelections()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice6@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventRepo = new FakeEventRepository();
        var invitationRepo = new FakeInvitationRepository();
        var taskRepo = new FakeTaskRepository();
        var service = new EventManagementService(eventRepo, persons, invitationRepo, taskRepo, new FakeEventTypeRepository(), new FakeLocationRepository());
        var invitationService = new InvitationService(eventRepo, invitationRepo);

        var location = new Location("Aula A", ValidAddress);
        var organizers = new[] { new EventOrganizer(organizer.Id, false) };
        var ev = await service.CreateEventAsync("Test", new EventType("Birthday Party"), ValidRange, location, organizers);

        var kueTask = await service.AddChecklistTaskAsync(ev.Id, "Bawa kue");
        await service.DesignateTaskForAttendeesAsync(ev.Id, kueTask.Id);
        var minumanTask = await service.AddChecklistTaskAsync(ev.Id, "Bawa minuman");
        await service.DesignateTaskForAttendeesAsync(ev.Id, minumanTask.Id);

        // Budi selects kue, Ani selects minuman
        var budiInv = await invitationService.InviteAttendeeAsync(ev.Id, new PersonName("Budi", "S"), new EmailAddress("budi6@mail.com"));
        var budiId = Guid.NewGuid();
        budiInv.LinkToAttendee(budiId);
        await invitationRepo.UpdateAsync(budiInv);
        await invitationService.RespondToInvitationAsync(budiInv.Id, InvitationStatus.WillAttend);
        await service.SelectTaskForAttendeeAsync(ev.Id, kueTask.Id, budiId);

        var aniInv = await invitationService.InviteAttendeeAsync(ev.Id, new PersonName("Ani", "S"), new EmailAddress("ani6@mail.com"));
        var aniId = Guid.NewGuid();
        aniInv.LinkToAttendee(aniId);
        await invitationRepo.UpdateAsync(aniInv);
        await invitationService.RespondToInvitationAsync(aniInv.Id, InvitationStatus.WillAttend);
        await service.SelectTaskForAttendeeAsync(ev.Id, minumanTask.Id, aniId);

        var summary = await service.GetEventSummaryAsync(ev.Id);

        Assert.Equal(2, summary.AttendeeTaskSelections.Count);
        Assert.Contains(summary.AttendeeTaskSelections, t => t.Id == kueTask.Id && t.SelectedByAttendeeId == budiId);
        Assert.Contains(summary.AttendeeTaskSelections, t => t.Id == minumanTask.Id && t.SelectedByAttendeeId == aniId);
    }

    // AT-007: Confirmed attendee can select multiple tasks
    [Fact]
    public async System.Threading.Tasks.Task AT007_SelectMultipleTasks_ConfirmedAttendee_BothTasksRegistered()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice7@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventRepo = new FakeEventRepository();
        var invitationRepo = new FakeInvitationRepository();
        var taskRepo = new FakeTaskRepository();
        var service = new EventManagementService(eventRepo, persons, invitationRepo, taskRepo, new FakeEventTypeRepository(), new FakeLocationRepository());
        var invitationService = new InvitationService(eventRepo, invitationRepo);

        var location = new Location("Aula A", ValidAddress);
        var organizers = new[] { new EventOrganizer(organizer.Id, false) };
        var ev = await service.CreateEventAsync("Test", new EventType("Birthday Party"), ValidRange, location, organizers);

        var kueTask = await service.AddChecklistTaskAsync(ev.Id, "Bawa kue");
        await service.DesignateTaskForAttendeesAsync(ev.Id, kueTask.Id);
        var minumanTask = await service.AddChecklistTaskAsync(ev.Id, "Bawa minuman");
        await service.DesignateTaskForAttendeesAsync(ev.Id, minumanTask.Id);

        var inv = await invitationService.InviteAttendeeAsync(ev.Id, new PersonName("Budi", "S"), new EmailAddress("budi7@mail.com"));
        var attendeeId = Guid.NewGuid();
        inv.LinkToAttendee(attendeeId);
        await invitationRepo.UpdateAsync(inv);
        await invitationService.RespondToInvitationAsync(inv.Id, InvitationStatus.WillAttend);

        await service.SelectTaskForAttendeeAsync(ev.Id, kueTask.Id, attendeeId);
        await service.SelectTaskForAttendeeAsync(ev.Id, minumanTask.Id, attendeeId);

        var kue = await taskRepo.GetByIdAsync(kueTask.Id);
        var minuman = await taskRepo.GetByIdAsync(minumanTask.Id);
        Assert.Equal(attendeeId, kue!.SelectedByAttendeeId);
        Assert.Equal(attendeeId, minuman!.SelectedByAttendeeId);
    }

    // AT-008: Task selected by one attendee is still visible to other attendees (with selection info)
    [Fact]
    public async System.Threading.Tasks.Task AT008_GetVisibleTasks_TaskSelectedByOther_StillVisibleWithSelectionInfo()
    {
        var (service, invitationService, invitationRepo, taskRepo, eventId, taskId) = await SetupAsync();

        // Budi selects the task
        var budiId = await InviteAndConfirmAttendeeAsync(invitationService, invitationRepo, eventId, "budi8@mail.com");
        await service.SelectTaskForAttendeeAsync(eventId, taskId, budiId);

        // Ani is also a confirmed attendee
        var aniId = await InviteAndConfirmAttendeeAsync(invitationService, invitationRepo, eventId, "ani8@mail.com");

        // Ani can still see the task list (task is still visible even though Budi selected it)
        var tasks = (await service.GetVisibleTasksForConfirmedAttendeeAsync(eventId, aniId)).ToList();

        Assert.Contains(tasks, t => t.Id == taskId);
        // The task shows who selected it
        var selectedTask = tasks.First(t => t.Id == taskId);
        Assert.Equal(budiId, selectedTask.SelectedByAttendeeId);
    }
}
