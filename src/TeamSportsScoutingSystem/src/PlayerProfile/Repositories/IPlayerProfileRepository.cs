using TeamSportsScoutingSystem.Domain.PlayerProfile;

namespace TeamSportsScoutingSystem.Domain.PlayerProfile.Repositories;

public interface IPlayerProfileRepository
{
    Task<PlayerProfileAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerProfileAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerProfileAggregate>> GetByHeadCoachAsync(Guid headCoachId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerProfileAggregate>> GetActiveProfilesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(PlayerProfileAggregate profile, CancellationToken cancellationToken = default);
    Task UpdateAsync(PlayerProfileAggregate profile, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
