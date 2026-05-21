using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class EventOrganizerTests
{
    private static readonly Address ValidAddress = new("Jl. Merdeka 1", "Jakarta", "DKI Jakarta", "10110", "Indonesia");
    private static readonly PhoneNumber ValidPhone = new("08123456789");

    private static RegistrationService CreateService(FakePersonRepository? persons = null, FakeInvitationRepository? invitations = null) =>
        new(persons ?? new FakePersonRepository(), invitations ?? new FakeInvitationRepository());

    // EO-001: Successful organizer registration with all required fields
    [Fact]
    public async System.Threading.Tasks.Task EO001_RegisterOrganizer_WithAllFields_CreatesOrganizerAccount()
    {
        var service = CreateService();
        var name = new PersonName("John", "Doe");
        var email = new EmailAddress("john@mail.com");
        var password = new Password("Secure@123");

        var organizer = await service.RegisterOrganizerAsync(name, email, ValidAddress, ValidPhone, password);

        Assert.NotNull(organizer);
        Assert.Equal("john@mail.com", organizer.EmailAddress.Value);
        Assert.True(organizer.IsOrganizer);
    }

    // EO-002: Registration fails when a required field (last name) is missing
    [Fact]
    public void EO002_RegisterOrganizer_WithEmptyLastName_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PersonName("John", ""));
    }

    // EO-003: Registration fails when email is already in use
    [Fact]
    public async System.Threading.Tasks.Task EO003_RegisterOrganizer_WithDuplicateEmail_ThrowsDomainException()
    {
        var persons = new FakePersonRepository();
        var service = CreateService(persons);
        var email = new EmailAddress("existing@mail.com");

        await service.RegisterOrganizerAsync(new PersonName("First", "User"), email, ValidAddress, ValidPhone, new Password("Pass@123"));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.RegisterOrganizerAsync(new PersonName("Second", "User"), email, ValidAddress, ValidPhone, new Password("Pass@456")));
    }

    // EO-004: Organizer selects an existing event type from the predefined list
    [Fact]
    public void EO004_EventType_SelectExisting_CanBeCreated()
    {
        var eventType = new EventType("Birthday Party");
        Assert.Equal("Birthday Party", eventType.Name);
    }

    // EO-005: Organizer creates a new event type not in the list
    [Fact]
    public async System.Threading.Tasks.Task EO005_EventType_CreateNew_IsAddedToRepository()
    {
        var eventTypeRepo = new FakeEventTypeRepository();
        var newType = new EventType("Farewell Party");

        await eventTypeRepo.AddAsync(newType);

        Assert.True(await eventTypeRepo.ExistsAsync("Farewell Party"));
    }

    // EO-006: Organizer manages event without attending
    [Fact]
    public async System.Threading.Tasks.Task EO006_CreateEvent_OrganizerNotAttending_NotInAttendeeList()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, ValidPhone, new EmailAddress("alice@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventRepo = new FakeEventRepository();
        var service = new EventManagementService(eventRepo, persons, new FakeInvitationRepository(), new FakeTaskRepository(), new FakeEventTypeRepository(), new FakeLocationRepository());

        var location = new Location("Aula A", new Address("Jl. A", "Jakarta", "DKI", "10110", "Indonesia"));
        var organizers = new[] { new Event.EventOrganizer(organizer.Id, isAttending: false) };
        var ev = await service.CreateEventAsync("Test Event", new EventType("Birthday Party"), new DateTimeRange(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(4)), location, organizers);

        Assert.DoesNotContain(organizer.Id, ev.AttendeeIds);
        Assert.Contains(organizer.Id, ev.NonAttendingOrganizerIds);
    }

    // EO-007: Organizer manages event and also attends
    [Fact]
    public async System.Threading.Tasks.Task EO007_CreateEvent_OrganizerAttending_IsInAttendingOrganizerList()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Bob", "Jones"), ValidAddress, ValidPhone, new EmailAddress("bob@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventRepo = new FakeEventRepository();
        var service = new EventManagementService(eventRepo, persons, new FakeInvitationRepository(), new FakeTaskRepository(), new FakeEventTypeRepository(), new FakeLocationRepository());

        var location = new Location("Aula B", new Address("Jl. B", "Jakarta", "DKI", "10110", "Indonesia"));
        var organizers = new[] { new Event.EventOrganizer(organizer.Id, isAttending: true) };
        var ev = await service.CreateEventAsync("Test Event", new EventType("Birthday Party"), new DateTimeRange(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(4)), location, organizers);

        Assert.Contains(organizer.Id, ev.AttendingOrganizerIds);
    }
}
