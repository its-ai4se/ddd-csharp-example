using TileOApplication.Domain.Game;

namespace TileOApplication.Domain.Repositories;

public interface IGameRepository
{
    Task<GameAggregate?> GetByIdAsync(Guid id);
    Task<IEnumerable<GameAggregate>> GetAllAsync();
    Task SaveAsync(GameAggregate game);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
