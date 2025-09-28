using TileOApplication.Domain.Player;

namespace TileOApplication.Domain.Repositories;

public interface IPlayerRepository
{
    Task<PlayerAggregate?> GetByIdAsync(Guid id);
    Task<IEnumerable<PlayerAggregate>> GetAllAsync();
    Task<IEnumerable<PlayerAggregate>> GetByGameIdAsync(Guid gameId);
    Task SaveAsync(PlayerAggregate player);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
