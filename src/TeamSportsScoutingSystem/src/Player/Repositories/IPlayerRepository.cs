using TeamSportsScoutingSystem.Domain.Player;

namespace TeamSportsScoutingSystem.Domain.Player.Repositories;

public interface IPlayerRepository
{
    Task<PlayerAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerAggregate>> GetByListTypeAsync(string listType, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerAggregate>> GetByScoutAsync(Guid scoutId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerAggregate>> GetByCurrentClubAsync(string club, CancellationToken cancellationToken = default);
    Task<IEnumerable<PlayerAggregate>> GetByNationalityAsync(string nationality, CancellationToken cancellationToken = default);
    Task AddAsync(PlayerAggregate player, CancellationToken cancellationToken = default);
    Task UpdateAsync(PlayerAggregate player, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
