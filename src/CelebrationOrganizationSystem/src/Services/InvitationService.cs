using CelebrationOrganizationSystem.Domain.Event.Repositories;
using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Invitation.Repositories;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.Services;

public class InvitationService(
    IEventRepository eventRepository,
    IInvitationRepository invitationRepository)
{
    private readonly IEventRepository _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
    private readonly IInvitationRepository _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));

    public async Task<InvitationAggregate> InviteAttendeeAsync(
        Guid eventId,
        PersonName attendeeName,
        EmailAddress attendeeEmail)
    {
        var eventAggregate = await _eventRepository.GetByIdAsync(eventId);
        if (eventAggregate is null)
        {
            throw new DomainException($"Event with ID {eventId} not found.");
        }

        if (await _invitationRepository.ExistsByEventAndEmailAsync(eventId, attendeeEmail.Value))
        {
            throw new DomainException("Attendee email has already been invited to this event.");
        }

        var invitation = new InvitationAggregate(eventId, attendeeEmail, attendeeName);
        await _invitationRepository.AddAsync(invitation);
        return invitation;
    }

    public async System.Threading.Tasks.Task RespondToInvitationAsync(Guid invitationId, InvitationStatus status)
    {
        var invitation = await GetInvitationOrThrowAsync(invitationId);
        invitation.RespondToInvitation(status);
        await _invitationRepository.UpdateAsync(invitation);

        if (status == InvitationStatus.WillAttend && invitation.AttendeeId.HasValue)
        {
            var eventAggregate = await _eventRepository.GetByIdAsync(invitation.EventId);
            if (eventAggregate is not null)
            {
                eventAggregate.AddAttendee(invitation.AttendeeId.Value);
                await _eventRepository.UpdateAsync(eventAggregate);
            }
        }
    }

    public async Task<IEnumerable<InvitationAggregate>> GetInvitationsForAttendeeAsync(Guid attendeeId)
    {
        return await _invitationRepository.GetByAttendeeIdAsync(attendeeId);
    }

    public async Task<IEnumerable<InvitationAggregate>> GetInvitationsForEventAsync(Guid eventId)
    {
        return await _invitationRepository.GetByEventIdAsync(eventId);
    }

    public async Task<IEnumerable<InvitationAggregate>> GetConfirmedAttendeesAsync(Guid eventId)
    {
        var invitations = await _invitationRepository.GetByEventIdAsync(eventId);
        return invitations.Where(i => i.IsWillAttend);
    }

    public async Task<IEnumerable<InvitationAggregate>> GetTentativeAttendeesAsync(Guid eventId)
    {
        var invitations = await _invitationRepository.GetByEventIdAsync(eventId);
        return invitations.Where(i => i.IsMaybeWillAttend);
    }

    public async Task<IEnumerable<InvitationAggregate>> GetUnrepliedInvitationsAsync(Guid eventId)
    {
        var invitations = await _invitationRepository.GetByEventIdAsync(eventId);
        return invitations.Where(i => i.IsUnreplied);
    }

    public async Task<InvitationStatusSummary> GetInvitationStatusForEventAsync(Guid eventId)
    {
        var invitations = (await _invitationRepository.GetByEventIdAsync(eventId)).ToList();

        return new InvitationStatusSummary(
            invitations.Count,
            invitations.Count(i => i.HasResponded),
            invitations.Count(i => i.IsUnreplied),
            invitations.Where(i => i.IsWillAttend).ToList().AsReadOnly(),
            invitations.Where(i => i.IsMaybeWillAttend).ToList().AsReadOnly(),
            invitations.Where(i => i.IsCannotAttend).ToList().AsReadOnly());
    }

    private async Task<InvitationAggregate> GetInvitationOrThrowAsync(Guid invitationId)
    {
        return await _invitationRepository.GetByIdAsync(invitationId)
            ?? throw new DomainException($"Invitation with ID {invitationId} not found.");
    }
}

public record InvitationStatusSummary(
    int TotalInvitations,
    int RepliedCount,
    int UnrepliedCount,
    IReadOnlyList<InvitationAggregate> ConfirmedAttendees,
    IReadOnlyList<InvitationAggregate> TentativeAttendees,
    IReadOnlyList<InvitationAggregate> DeclinedAttendees
);
