using CelebrationOrganizationSystem.Domain.Invitation;

namespace CelebrationOrganizationSystem.Domain.Invitation.Repositories;

public interface IInvitationRepository
{
    Task<InvitationAggregate?> GetByIdAsync(Guid id);
    Task<IEnumerable<InvitationAggregate>> GetByEventIdAsync(Guid eventId);
    Task<IEnumerable<InvitationAggregate>> GetByAttendeeIdAsync(Guid attendeeId);
    Task<InvitationAggregate?> GetByEventAndAttendeeAsync(Guid eventId, Guid attendeeId);
    System.Threading.Tasks.Task AddAsync(InvitationAggregate invitation);
    System.Threading.Tasks.Task UpdateAsync(InvitationAggregate invitation);
    Task<bool> ExistsByEventAndEmailAsync(Guid eventId, string attendeeEmail);
}
