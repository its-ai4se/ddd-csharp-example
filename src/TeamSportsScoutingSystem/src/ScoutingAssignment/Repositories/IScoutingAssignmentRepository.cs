using TeamSportsScoutingSystem.Domain.ScoutingAssignment;

namespace TeamSportsScoutingSystem.Domain.ScoutingAssignment.Repositories;

public interface IScoutingAssignmentRepository
{
    Task<ScoutingAssignmentAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ScoutingAssignmentAggregate assignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScoutingAssignmentAggregate assignment, CancellationToken cancellationToken = default);
}
