using TileOApplication.Domain.Game;
using TileOApplication.Domain.Game.Repositories;

namespace TileOApplication.Domain.Tests.Helpers;

public class FakeGameRepository : IGameRepository
{
    private readonly GameAggregate? _activeGame;

    public FakeGameRepository(GameAggregate? activeGame)
    {
        _activeGame = activeGame;
    }

    public Task<GameAggregate?> GetActiveGameAsync() => Task.FromResult(_activeGame);

    public Task SaveDesignAsync(GameAggregate game)
    {
        if (game.Status == GameStatus.InProgress)
            throw new InvalidOperationException("Cannot save a game that is in progress.");
        return Task.CompletedTask;
    }
}
