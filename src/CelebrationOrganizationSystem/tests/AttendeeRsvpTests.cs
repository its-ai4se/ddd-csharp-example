using CelebrationOrganizationSystem.Domain.Event;
using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class AttendeeRsvpTests
{
    private static readonly Address ValidAddress = new("Jl. A", "Jakarta", "DKI", "10110", "Indonesia");

    private async Task<(InvitationService service, FakeInvitationRepository invitationRepo, InvitationAggregate invitation)> SetupAsync()
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
        var invitation = await invitationService.InviteAttendeeAsync(ev.Id, new PersonName("Budi", "Santoso"), new EmailAddress("budi@mail.com"));

        return (invitationService, invitationRepo, invitation);
    }

    // AR-001: Attendee responds with 'Will Attend'
    [Fact]
    public async System.Threading.Tasks.Task AR001_RespondToInvitation_WillAttend_StatusIsWillAttend()
    {
        var (service, invitationRepo, invitation) = await SetupAsync();

        await service.RespondToInvitationAsync(invitation.Id, InvitationStatus.WillAttend);

        var updated = await invitationRepo.GetByIdAsync(invitation.Id);
        Assert.True(updated!.IsWillAttend);
    }

    // AR-002: Attendee responds with 'Maybe Will Attend'
    [Fact]
    public async System.Threading.Tasks.Task AR002_RespondToInvitation_MaybeWillAttend_StatusIsMaybeWillAttend()
    {
        var (service, invitationRepo, invitation) = await SetupAsync();

        await service.RespondToInvitationAsync(invitation.Id, InvitationStatus.MaybeWillAttend);

        var updated = await invitationRepo.GetByIdAsync(invitation.Id);
        Assert.True(updated!.IsMaybeWillAttend);
    }

    // AR-003: Attendee responds with 'Cannot Attend'
    [Fact]
    public async System.Threading.Tasks.Task AR003_RespondToInvitation_CannotAttend_StatusIsCannotAttend()
    {
        var (service, invitationRepo, invitation) = await SetupAsync();

        await service.RespondToInvitationAsync(invitation.Id, InvitationStatus.CannotAttend);

        var updated = await invitationRepo.GetByIdAsync(invitation.Id);
        Assert.True(updated!.IsCannotAttend);
        Assert.False(updated.IsWillAttend);
    }

    // AR-004: Attendee has not responded yet — default status is unreplied
    [Fact]
    public async System.Threading.Tasks.Task AR004_Invitation_WithNoResponse_IsUnreplied()
    {
        var (_, invitationRepo, invitation) = await SetupAsync();

        var stored = await invitationRepo.GetByIdAsync(invitation.Id);
        Assert.True(stored!.IsUnreplied);
        Assert.False(stored.HasResponded);
    }
}
