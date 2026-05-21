using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class EventTests
{
    private static readonly Address ValidAddress = new("Jl. A", "Jakarta", "DKI", "10110", "Indonesia");
    private static readonly Location ValidLocation = new("Aula Gedung A", ValidAddress);
    private static readonly EventType ValidEventType = new("Birthday Party");
    private static readonly DateTimeRange ValidRange = new(new DateTime(2025, 8, 1, 10, 0, 0), new DateTime(2025, 8, 1, 14, 0, 0));

    private async Task<(EventManagementService service, Guid organizerId)> CreateServiceWithOrganizerAsync()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var service = new EventManagementService(new FakeEventRepository(), persons, new FakeInvitationRepository(), new FakeTaskRepository(), new FakeEventTypeRepository(), new FakeLocationRepository());
        return (service, organizer.Id);
    }

    // EV-001: Event creation succeeds with all required fields
    [Fact]
    public async System.Threading.Tasks.Task EV001_CreateEvent_WithAllFields_Succeeds()
    {
        var (service, organizerId) = await CreateServiceWithOrganizerAsync();
        var organizers = new[] { new EventOrganizer(organizerId, false) };

        var ev = await service.CreateEventAsync("Ulang Tahun Andi", ValidEventType, ValidRange, ValidLocation, organizers);

        Assert.NotNull(ev);
        Assert.Equal("Ulang Tahun Andi", ev.Occasion);
        Assert.Equal("Aula Gedung A", ev.Location.Name);
    }

    // EV-002: Event creation fails when start date is missing (null DateTime treated as default)
    [Fact]
    public void EV002_DateTimeRange_WithDefaultStart_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new DateTimeRange(default, new DateTime(2025, 8, 1, 14, 0, 0)));
    }

    // EV-003: Event creation fails when end date is missing
    [Fact]
    public void EV003_DateTimeRange_WithDefaultEnd_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new DateTimeRange(new DateTime(2025, 8, 1, 10, 0, 0), default));
    }

    // EV-004: Event creation fails when occasion is empty
    [Fact]
    public async System.Threading.Tasks.Task EV004_CreateEvent_WithEmptyOccasion_ThrowsException()
    {
        var (service, organizerId) = await CreateServiceWithOrganizerAsync();
        var organizers = new[] { new EventOrganizer(organizerId, false) };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateEventAsync("", ValidEventType, ValidRange, ValidLocation, organizers));
    }

    // EV-005: Event creation fails when location is null
    [Fact]
    public async System.Threading.Tasks.Task EV005_CreateEvent_WithNullLocation_ThrowsException()
    {
        var (service, organizerId) = await CreateServiceWithOrganizerAsync();
        var organizers = new[] { new EventOrganizer(organizerId, false) };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.CreateEventAsync("Ulang Tahun", ValidEventType, ValidRange, null!, organizers));
    }

    // EV-006: Organizer selects location from predefined list
    [Fact]
    public async System.Threading.Tasks.Task EV006_CreateEvent_WithExistingLocation_UsesSelectedLocation()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice2@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var locationRepo = new FakeLocationRepository();
        locationRepo.SeedLocation(ValidLocation);

        var service = new EventManagementService(new FakeEventRepository(), persons, new FakeInvitationRepository(), new FakeTaskRepository(), new FakeEventTypeRepository(), locationRepo);
        var organizers = new[] { new EventOrganizer(organizer.Id, false) };

        var ev = await service.CreateEventAsync("Test", ValidEventType, ValidRange, ValidLocation, organizers);

        Assert.Equal("Aula Gedung A", ev.Location.Name);
        // Location already existed, should still be in repo (not duplicated)
        Assert.True(await locationRepo.ExistsAsync("Aula Gedung A"));
    }

    // EV-007: Organizer creates a new location with name and address
    [Fact]
    public async System.Threading.Tasks.Task EV007_CreateEvent_WithNewLocation_AddsLocationToRepository()
    {
        var locationRepo = new FakeLocationRepository();
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Bob", "Jones"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("bob2@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var svc = new EventManagementService(new FakeEventRepository(), persons, new FakeInvitationRepository(), new FakeTaskRepository(), new FakeEventTypeRepository(), locationRepo);
        var newLocation = new Location("Villa Sejuk", new Address("Jl. Puncak No. 10", "Bogor", "Jawa Barat", "16710", "Indonesia"));
        var organizers = new[] { new EventOrganizer(organizer.Id, false) };

        await svc.CreateEventAsync("Test", ValidEventType, ValidRange, newLocation, organizers);

        Assert.True(await locationRepo.ExistsAsync("Villa Sejuk"));
    }

    // EV-008: New location creation fails when name is empty
    [Fact]
    public void EV008_Location_WithEmptyName_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Location("", ValidAddress));
    }

    // EV-009: New location creation fails when address is null
    [Fact]
    public void EV009_Location_WithNullAddress_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Location("Villa Sejuk", null!));
    }

    // EV-010: Small event with one organizer is valid
    [Fact]
    public async System.Threading.Tasks.Task EV010_CreateEvent_WithOneOrganizer_Succeeds()
    {
        var (service, organizerId) = await CreateServiceWithOrganizerAsync();
        var organizers = new[] { new EventOrganizer(organizerId, false) };

        var ev = await service.CreateEventAsync("Small Birthday Party", ValidEventType, ValidRange, ValidLocation, organizers);

        Assert.Single(ev.Organizers);
    }

    // EV-011: Large event with multiple organizers is valid
    [Fact]
    public async System.Threading.Tasks.Task EV011_CreateEvent_WithMultipleOrganizers_AllHaveAccess()
    {
        var persons = new FakePersonRepository();
        var ids = new List<Guid>();
        foreach (var (fn, ln, email) in new[] { ("Alice", "A", "alice3@mail.com"), ("Bob", "B", "bob3@mail.com"), ("Carol", "C", "carol@mail.com") })
        {
            var p = new PersonAggregate(new PersonName(fn, ln), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress(email), new Password("Pass@123"));
            p.AddRole(new OrganizerRole(p.Id));
            await persons.AddAsync(p);
            ids.Add(p.Id);
        }

        var service = new EventManagementService(new FakeEventRepository(), persons, new FakeInvitationRepository(), new FakeTaskRepository(), new FakeEventTypeRepository(), new FakeLocationRepository());
        var organizers = ids.Select(id => new EventOrganizer(id, false)).ToArray();

        var ev = await service.CreateEventAsync("Big Party", ValidEventType, ValidRange, ValidLocation, organizers);

        Assert.Equal(3, ev.Organizers.Count);
        foreach (var id in ids)
            Assert.Contains(id, ev.OrganizerIds);
    }
}
