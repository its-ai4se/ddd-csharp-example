using CelebrationOrganizationSystem.Domain.Invitation.Repositories;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Person.Repositories;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.Services;

public class RegistrationService(
    IPersonRepository personRepository,
    IInvitationRepository invitationRepository)
{
    private readonly IPersonRepository _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    private readonly IInvitationRepository _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));

    public async Task<PersonAggregate> RegisterOrganizerAsync(
        PersonName name,
        EmailAddress email,
        Address address,
        PhoneNumber phoneNumber,
        Password password)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(phoneNumber);

        if (await _personRepository.ExistsByEmailAsync(email.Value))
        {
            throw new DomainException($"A person with email {email.Value} already exists.");
        }

        var organizer = new PersonAggregate(name, address, phoneNumber, email, password);
        organizer.AddRole(new OrganizerRole(organizer.Id));
        await _personRepository.AddAsync(organizer);
        return organizer;
    }

    public async Task<PersonAggregate> RegisterAttendeeFromInvitationAsync(Guid invitationId, Password password)
    {
        var invitation = await GetInvitationOrThrowAsync(invitationId);
        var attendee = await _personRepository.GetByEmailAsync(invitation.AttendeeEmail.Value);

        if (attendee is null)
        {
            attendee = new PersonAggregate(invitation.AttendeeName, null, null, invitation.AttendeeEmail, password);
            attendee.AddRole(new AttendeeRole(attendee.Id));
            await _personRepository.AddAsync(attendee);
        }
        else if (!attendee.IsAttendee)
        {
            attendee.AddRole(new AttendeeRole(attendee.Id));
            await _personRepository.UpdateAsync(attendee);
        }

        invitation.LinkToAttendee(attendee.Id);
        await _invitationRepository.UpdateAsync(invitation);
        return attendee;
    }

    public async System.Threading.Tasks.Task LinkExistingAttendeeAccountAsync(Guid invitationId, Guid attendeeId)
    {
        var invitation = await GetInvitationOrThrowAsync(invitationId);
        var attendee = await _personRepository.GetByIdAsync(attendeeId)
            ?? throw new DomainException($"Attendee with ID {attendeeId} not found.");

        if (attendee.EmailAddress != invitation.AttendeeEmail)
        {
            throw new DomainException("Invitation can only be linked to an account with the invited email address.");
        }

        if (!attendee.IsAttendee)
        {
            attendee.AddRole(new AttendeeRole(attendee.Id));
            await _personRepository.UpdateAsync(attendee);
        }

        invitation.LinkToAttendee(attendee.Id);
        await _invitationRepository.UpdateAsync(invitation);
    }

    private async Task<Invitation.InvitationAggregate> GetInvitationOrThrowAsync(Guid invitationId)
    {
        return await _invitationRepository.GetByIdAsync(invitationId)
            ?? throw new DomainException($"Invitation with ID {invitationId} not found.");
    }
}
