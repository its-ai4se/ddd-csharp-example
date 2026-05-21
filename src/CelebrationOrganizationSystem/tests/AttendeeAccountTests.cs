using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Services;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using CelebrationOrganizationSystem.Domain.Tests.TestHelpers;

namespace CelebrationOrganizationSystem.Domain.Tests;

public class AttendeeAccountTests
{
    private static readonly Guid EventId = Guid.NewGuid();

    private static (RegistrationService service, FakePersonRepository persons, FakeInvitationRepository invitations) CreateService()
    {
        var persons = new FakePersonRepository();
        var invitations = new FakeInvitationRepository();
        return (new RegistrationService(persons, invitations), persons, invitations);
    }

    private static async Task<InvitationAggregate> SeedInvitationAsync(FakeInvitationRepository invitations, string email, string firstName = "Budi", string lastName = "Santoso")
    {
        var invitation = new InvitationAggregate(EventId, new EmailAddress(email), new PersonName(firstName, lastName));
        await invitations.AddAsync(invitation);
        return invitation;
    }

    // AC-001: New attendee creates account using invitation email as username
    [Fact]
    public async System.Threading.Tasks.Task AC001_RegisterAttendeeFromInvitation_NewAccount_CreatesWithInvitationEmail()
    {
        var (service, _, invitations) = CreateService();
        var invitation = await SeedInvitationAsync(invitations, "guest@mail.com");

        var attendee = await service.RegisterAttendeeFromInvitationAsync(invitation.Id, new Password("Pass@456"));

        Assert.NotNull(attendee);
        Assert.Equal("guest@mail.com", attendee.EmailAddress.Value);
        Assert.True(attendee.IsAttendee);
    }

    // AC-002: Account creation fails when using a different email than the invitation
    [Fact]
    public async System.Threading.Tasks.Task AC002_LinkExistingAccount_WithDifferentEmail_ThrowsDomainException()
    {
        var (service, persons, invitations) = CreateService();
        var invitation = await SeedInvitationAsync(invitations, "guest@mail.com");

        // Create an account with a different email
        var otherPerson = new PersonAggregate(new PersonName("Other", "User"), null, null, new EmailAddress("other@mail.com"), new Password("Pass@123"));
        otherPerson.AddRole(new AttendeeRole(otherPerson.Id));
        await persons.AddAsync(otherPerson);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.LinkExistingAttendeeAccountAsync(invitation.Id, otherPerson.Id));
    }

    // AC-003: Attendee who already has an account does not need to create a new one
    [Fact]
    public async System.Threading.Tasks.Task AC003_RegisterAttendeeFromInvitation_ExistingAccount_ReusesExistingAccount()
    {
        var (service, persons, invitations) = CreateService();
        var invitation = await SeedInvitationAsync(invitations, "existing@mail.com");

        // Pre-existing account with same email
        var existing = new PersonAggregate(new PersonName("Existing", "User"), null, null, new EmailAddress("existing@mail.com"), new Password("OldPass@1"));
        existing.AddRole(new AttendeeRole(existing.Id));
        await persons.AddAsync(existing);

        var attendee = await service.RegisterAttendeeFromInvitationAsync(invitation.Id, new Password("NewPass@1"));

        // Should return the existing account (same ID)
        Assert.Equal(existing.Id, attendee.Id);
    }

    // AC-004: Account creation fails when password is empty
    [Fact]
    public void AC004_Password_WithEmptyValue_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Password(""));
    }
}
