namespace TileOApplication.Domain.Game.Repositories;

public interface IGameRepository
{
    Task<GameAggregate?> GetActiveGameAsync();
    Task SaveDesignAsync(GameAggregate game);
}
