using CelebrationOrganizationSystem.Domain.Invitation;

namespace CelebrationOrganizationSystem.Domain.Invitation.Repositories;

public interface IInvitationRepository
{
    System.Threading.Tasks.Task<InvitationAggregate?> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetAllAsync();
    System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetByEventIdAsync(Guid eventId);
    System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetByAttendeeIdAsync(Guid attendeeId);
    System.Threading.Tasks.Task<InvitationAggregate?> GetByEventAndAttendeeAsync(Guid eventId, Guid attendeeId);
    System.Threading.Tasks.Task<IEnumerable<InvitationAggregate>> GetByStatusAsync(Shared.ValueObjects.InvitationStatus status);
    System.Threading.Tasks.Task AddAsync(InvitationAggregate invitation);
    System.Threading.Tasks.Task UpdateAsync(InvitationAggregate invitation);
    System.Threading.Tasks.Task DeleteAsync(Guid id);
    System.Threading.Tasks.Task<bool> ExistsAsync(Guid id);
    System.Threading.Tasks.Task<bool> ExistsByEventAndAttendeeAsync(Guid eventId, Guid attendeeId);
}
