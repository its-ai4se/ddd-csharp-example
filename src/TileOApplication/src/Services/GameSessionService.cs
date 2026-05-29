using TileOApplication.Domain.Game;
using TileOApplication.Domain.Game.Repositories;

namespace TileOApplication.Domain.Services;

public class GameSessionService
{
    private readonly IGameRepository _gameRepository;

    public GameSessionService(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task StartGameAsync(GameAggregate game)
    {
        var activeGame = await _gameRepository.GetActiveGameAsync();
        if (activeGame is not null)
            throw new InvalidOperationException("A game is already in progress. Only one game can be active at a time.");

        game.StartGame();
    }
}
