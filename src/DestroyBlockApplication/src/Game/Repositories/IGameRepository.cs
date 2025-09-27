using DestroyBlockApplication.Domain.Game;

namespace DestroyBlockApplication.Domain.Game.Repositories;

public interface IGameRepository
{
    Task<GameAggregate?> GetByIdAsync(Guid id);
    Task<GameAggregate?> GetByNameAsync(string name);
    Task<IEnumerable<GameAggregate>> GetAllAsync();
    Task<IEnumerable<GameAggregate>> GetPublishedGamesAsync();
    Task<IEnumerable<GameAggregate>> GetGamesByAdminAsync(Guid adminId);
    Task AddAsync(GameAggregate game);
    Task UpdateAsync(GameAggregate game);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name);
}
