using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class AttendeeInvitationTests
{
    private static readonly Address ValidAddress = new("Jl. A", "Jakarta", "DKI", "10110", "Indonesia");

    private async Task<(InvitationService service, FakeEventRepository eventRepo, FakeInvitationRepository invitationRepo, Guid eventId)> SetupAsync()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventRepo = new FakeEventRepository();
        var invitationRepo = new FakeInvitationRepository();

        var eventMgmt = new EventManagementService(eventRepo, persons, invitationRepo, new FakeTaskRepository(), new FakeEventTypeRepository(), new FakeLocationRepository());
        var location = new Location("Aula A", ValidAddress);
        var organizers = new[] { new EventOrganizer(organizer.Id, false) };
        var ev = await eventMgmt.CreateEventAsync("Test Event", new EventType("Birthday Party"), new DateTimeRange(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(4)), location, organizers);

        var invitationService = new InvitationService(eventRepo, invitationRepo);
        return (invitationService, eventRepo, invitationRepo, ev.Id);
    }

    // AI-001: Organizer successfully invites attendee with complete data
    [Fact]
    public async System.Threading.Tasks.Task AI001_InviteAttendee_WithAllFields_CreatesInvitation()
    {
        var (service, _, invitationRepo, eventId) = await SetupAsync();

        var invitation = await service.InviteAttendeeAsync(eventId, new PersonName("Budi", "Santoso"), new EmailAddress("budi@mail.com"));

        Assert.NotNull(invitation);
        Assert.Equal("budi@mail.com", invitation.AttendeeEmail.Value);
        Assert.Equal(eventId, invitation.EventId);

        var stored = await invitationRepo.GetByEventIdAsync(eventId);
        Assert.Contains(stored, i => i.AttendeeEmail.Value == "budi@mail.com");
    }

    // AI-002: Invitation fails when attendee email is empty
    [Fact]
    public void AI002_EmailAddress_WithEmptyValue_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new EmailAddress(""));
    }

    // AI-003: Invitation fails when attendee first name is empty
    [Fact]
    public void AI003_PersonName_WithEmptyFirstName_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new PersonName("", "Santoso"));
    }
}
