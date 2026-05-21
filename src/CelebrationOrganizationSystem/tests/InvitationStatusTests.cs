using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class InvitationStatusTests
{
    private static readonly Address ValidAddress = new("Jl. A", "Jakarta", "DKI", "10110", "Indonesia");

    private async Task<(InvitationService service, Guid eventId)> SetupEventWithInvitationsAsync()
    {
        var persons = new FakePersonRepository();
        var organizer = new PersonAggregate(new PersonName("Alice", "Smith"), ValidAddress, new PhoneNumber("08123456789"), new EmailAddress("alice@mail.com"), new Password("Pass@123"));
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await persons.AddAsync(organizer);

        var eventRepo = new FakeEventRepository();
        var invitationRepo = new FakeInvitationRepository();
        var eventMgmt = new EventManagementService(eventRepo, persons, invitationRepo, new FakeTaskRepository(), new FakeEventTypeRepository(), new FakeLocationRepository());
        var location = new Location("Aula A", ValidAddress);
        var ev = await eventMgmt.CreateEventAsync("Test Event", new EventType("Birthday Party"), new DateTimeRange(DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(4)), location, [new EventOrganizer(organizer.Id, false)]);

        var invitationService = new InvitationService(eventRepo, invitationRepo);

        // Seed 10 invitations: 4 WillAttend, 2 Maybe, 1 Cannot, 3 unreplied
        var statuses = new (string email, InvitationStatus? status)[]
        {
            ("a1@mail.com", InvitationStatus.WillAttend),
            ("a2@mail.com", InvitationStatus.WillAttend),
            ("a3@mail.com", InvitationStatus.WillAttend),
            ("a4@mail.com", InvitationStatus.WillAttend),
            ("b1@mail.com", InvitationStatus.MaybeWillAttend),
            ("b2@mail.com", InvitationStatus.MaybeWillAttend),
            ("c1@mail.com", InvitationStatus.CannotAttend),
            ("d1@mail.com", null),
            ("d2@mail.com", null),
            ("d3@mail.com", null),
        };

        foreach (var (email, status) in statuses)
        {
            var inv = await invitationService.InviteAttendeeAsync(ev.Id, new PersonName("Guest", "User"), new EmailAddress(email));
            if (status.HasValue)
                await invitationService.RespondToInvitationAsync(inv.Id, status.Value);
        }

        return (invitationService, ev.Id);
    }

    // IS-001: Organizer views invitation status summary with accurate counts
    [Fact]
    public async System.Threading.Tasks.Task IS001_GetInvitationStatus_ReturnsAccurateCounts()
    {
        var (service, eventId) = await SetupEventWithInvitationsAsync();

        var summary = await service.GetInvitationStatusForEventAsync(eventId);

        Assert.Equal(10, summary.TotalInvitations);
        Assert.Equal(7, summary.RepliedCount);
        Assert.Equal(3, summary.UnrepliedCount);
        Assert.Equal(4, summary.ConfirmedAttendees.Count);
        Assert.Equal(2, summary.TentativeAttendees.Count);
        Assert.Single(summary.DeclinedAttendees);
    }

    // IS-002: Organizer views list of confirmed attendees (Will Attend)
    [Fact]
    public async System.Threading.Tasks.Task IS002_GetConfirmedAttendees_ReturnsOnlyWillAttend()
    {
        var (service, eventId) = await SetupEventWithInvitationsAsync();

        var confirmed = (await service.GetConfirmedAttendeesAsync(eventId)).ToList();

        Assert.Equal(4, confirmed.Count);
        Assert.All(confirmed, i => Assert.True(i.IsWillAttend));
    }

    // IS-003: Organizer views list of attendees who have not responded
    [Fact]
    public async System.Threading.Tasks.Task IS003_GetUnrepliedInvitations_ReturnsOnlyUnreplied()
    {
        var (service, eventId) = await SetupEventWithInvitationsAsync();

        var unreplied = (await service.GetUnrepliedInvitationsAsync(eventId)).ToList();

        Assert.Equal(3, unreplied.Count);
        Assert.All(unreplied, i => Assert.True(i.IsUnreplied));
    }
}
