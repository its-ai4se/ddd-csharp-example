using TeamSportsScoutingSystem.Domain.PlayerProfile;

namespace TeamSportsScoutingSystem.Domain.PlayerProfile.Repositories;

public interface IPlayerProfileRepository
{
    Task<PlayerProfileAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PlayerProfileAggregate profile, CancellationToken cancellationToken = default);
}
