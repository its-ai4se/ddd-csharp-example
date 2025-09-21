using CelebrationOrganizationSystem.Domain.Invitation;
using CelebrationOrganizationSystem.Domain.Invitation.Repositories;
using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Person.Repositories;
using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.Services;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.Services;

public class InvitationService : DomainServiceBase
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IPersonRepository _personRepository;

    public InvitationService(
        IClock clock,
        IInvitationRepository invitationRepository,
        IPersonRepository personRepository) : base(clock)
    {
        _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async System.Threading.Tasks.Task RespondToInvitationAsync(Guid invitationId, InvitationStatus status)
    {
        var invitation = await _invitationRepository.GetByIdAsync(invitationId);
        if (invitation == null)
        {
            throw new DomainException($"Invitation with ID {invitationId} not found.");
        }

        invitation.RespondToInvitation(status);
        await _invitationRepository.UpdateAsync(invitation);
    }

    public async System.Threading.Tasks.Task UpdateInvitationResponseAsync(Guid invitationId, InvitationStatus newStatus)
    {
        var invitation = await _invitationRepository.GetByIdAsync(invitationId);
        if (invitation == null)
        {
            throw new DomainException($"Invitation with ID {invitationId} not found.");
        }

        invitation.UpdateResponse(newStatus);
        await _invitationRepository.UpdateAsync(invitation);
    }

    public async System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetInvitationsForAttendeeAsync(Guid attendeeId)
    {
        return await _invitationRepository.GetByAttendeeIdAsync(attendeeId);
    }

    public async System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetInvitationsForEventAsync(Guid eventId)
    {
        return await _invitationRepository.GetByEventIdAsync(eventId);
    }

    public async System.Threading.Tasks.Task<InvitationStatistics> GetInvitationStatisticsAsync(Guid eventId)
    {
        var invitations = await _invitationRepository.GetByEventIdAsync(eventId);
        
        var totalInvitations = invitations.Count();
        var acceptedCount = invitations.Count(i => i.IsAccepted);
        var maybeCount = invitations.Count(i => i.IsMaybe);
        var declinedCount = invitations.Count(i => i.IsDeclined);
        var pendingCount = invitations.Count(i => i.IsPending);

        return new InvitationStatistics(
            totalInvitations,
            acceptedCount,
            maybeCount,
            declinedCount,
            pendingCount
        );
    }
}

public record InvitationStatistics(
    int TotalInvitations,
    int AcceptedCount,
    int MaybeCount,
    int DeclinedCount,
    int PendingCount
);
