using TeamSportsScoutingSystem.Domain.Player;

namespace TeamSportsScoutingSystem.Domain.Player.Repositories;

public interface IPlayerRepository
{
    Task<PlayerAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerAggregate>> GetByListTypeAsync(string listType, CancellationToken cancellationToken = default);
    Task AddAsync(PlayerAggregate player, CancellationToken cancellationToken = default);
    Task UpdateAsync(PlayerAggregate player, CancellationToken cancellationToken = default);
}
