using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Services;
using TileOApplication.Domain.Shared.ValueObjects;
using TileOApplication.Domain.Tests.Helpers;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class GameSessionTests
{
    private static GameAggregate CreateFullyConfiguredGame()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(2, 2));
        game.Board.SetActionTile(new Position(1, 1), 2);
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(2, 0), game.Players[1].Id);

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        return game;
    }

    [Fact]
    public async Task GS001_StartGameWhenAnotherGameIsActive_ShouldThrow()
    {
        var activeGame = CreateFullyConfiguredGame();
        activeGame.StartGame();

        var repository = new FakeGameRepository(activeGame);
        var service = new GameSessionService(repository);

        var newGame = CreateFullyConfiguredGame();

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.StartGameAsync(newGame));
    }

    [Fact]
    public void GS002_PauseGameWhenGameIsInProgress_ShouldNotBePossible()
    {
        var game = CreateFullyConfiguredGame();
        game.StartGame();

        var hasPauseMethod = typeof(GameAggregate).GetMethod("Pause") != null;
        Assert.False(hasPauseMethod);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public async Task GS003_SaveGameWhenGameIsInProgress_ShouldThrow()
    {
        var game = CreateFullyConfiguredGame();
        game.StartGame();

        var repository = new FakeGameRepository(null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.SaveDesignAsync(game));
    }

}
