using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Task;
using CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class EventChecklistTests
{
    private static readonly Address ValidAddress = new("Jl. A", "Jakarta", "DKI", "10110", "Indonesia");
    private static readonly DateTimeRange ValidRange = new(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(4));

    private async Task<(EventManagementService service, FakeEventTypeRepository eventTypeRepo, FakeTaskRepository taskRepo, Guid organizerId)> SetupAsync()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventTypeRepo = new FakeEventTypeRepository();
        var taskRepo = new FakeTaskRepository();
        var service = new EventManagementService(new FakeEventRepository(), persons, new FakeInvitationRepository(), taskRepo, eventTypeRepo, new FakeLocationRepository());

        return (service, eventTypeRepo, taskRepo, organizer.Id);
    }

    // EC-001: Event-type-specific checklist is automatically presented when organizer selects event
    [Fact]
    public async System.Threading.Tasks.Task EC001_CreateEvent_WithTemplates_ChecklistIsAutoPopulated()
    {
        var (service, eventTypeRepo, taskRepo, organizerId) = await SetupAsync();

        // Seed checklist templates for Birthday Party
        eventTypeRepo.SeedEventType(new EventType("Birthday Party"));
        eventTypeRepo.SeedTemplate(new ChecklistTaskTemplate("Birthday Party", "Beli kue ulang tahun"));
        eventTypeRepo.SeedTemplate(new ChecklistTaskTemplate("Birthday Party", "Siapkan dekorasi"));

        var location = new Location("Aula A", ValidAddress);
        var organizers = new[] { new EventOrganizer(organizerId, false) };
        var ev = await service.CreateEventAsync("Ulang Tahun Andi", new EventType("Birthday Party"), ValidRange, location, organizers);

        Assert.Equal(2, ev.ChecklistTaskIds.Count);
        var tasks = (await taskRepo.GetByEventIdAsync(ev.Id)).ToList();
        Assert.Equal(2, tasks.Count);
        Assert.Contains(tasks, t => t.Title == "Beli kue ulang tahun");
        Assert.Contains(tasks, t => t.Title == "Siapkan dekorasi");
    }

    // EC-002: Different event types show different checklists
    [Fact]
    public async System.Threading.Tasks.Task EC002_CreateEvent_DifferentEventTypes_ShowDifferentChecklists()
    {
        var (service, eventTypeRepo, taskRepo, organizerId) = await SetupAsync();

        eventTypeRepo.SeedEventType(new EventType("Birthday Party"));
        eventTypeRepo.SeedTemplate(new ChecklistTaskTemplate("Birthday Party", "Beli kue ulang tahun"));

        eventTypeRepo.SeedEventType(new EventType("Graduation Party"));
        eventTypeRepo.SeedTemplate(new ChecklistTaskTemplate("Graduation Party", "Sewa toga"));

        var location = new Location("Aula A", ValidAddress);
        var organizers = new[] { new EventOrganizer(organizerId, false) };

        var birthdayEvent = await service.CreateEventAsync("Birthday", new EventType("Birthday Party"), ValidRange, location, organizers);
        var graduationEvent = await service.CreateEventAsync("Graduation", new EventType("Graduation Party"), ValidRange, location, organizers);

        var birthdayTasks = (await taskRepo.GetByEventIdAsync(birthdayEvent.Id)).ToList();
        var graduationTasks = (await taskRepo.GetByEventIdAsync(graduationEvent.Id)).ToList();

        Assert.Contains(birthdayTasks, t => t.Title == "Beli kue ulang tahun");
        Assert.DoesNotContain(birthdayTasks, t => t.Title == "Sewa toga");

        Assert.Contains(graduationTasks, t => t.Title == "Sewa toga");
        Assert.DoesNotContain(graduationTasks, t => t.Title == "Beli kue ulang tahun");
    }
}
