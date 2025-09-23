using TeamSportsScoutingSystem.Domain.ScoutingAssignment;

namespace TeamSportsScoutingSystem.Domain.ScoutingAssignment.Repositories;

public interface IScoutingAssignmentRepository
{
    Task<ScoutingAssignmentAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingAssignmentAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingAssignmentAggregate>> GetByPlayerAsync(Guid playerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingAssignmentAggregate>> GetByScoutAsync(Guid scoutId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingAssignmentAggregate>> GetByStatusAsync(ScoutingAssignmentStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScoutingAssignmentAggregate>> GetByHeadScoutAsync(Guid headScoutId, CancellationToken cancellationToken = default);
    Task AddAsync(ScoutingAssignmentAggregate assignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScoutingAssignmentAggregate assignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
