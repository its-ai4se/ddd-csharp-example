using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class GameObjectiveTests
{
    private static GameAggregate CreateStartedGame()
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(1, 1), 2);
        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 4; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));

        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        game.StartGame();
        return game;
    }

    [Fact]
    public void GO001_OnlyWinCondition_IsLandingOnHiddenTile()
    {
        var game = CreateStartedGame();
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);

        // Verify game is not won by visiting many tiles
        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(1, 0));
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Null(game.WinnerId);

        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(2, 0));
        Assert.Equal(GameStatus.InProgress, game.Status);

        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(2, 0));
        Assert.Equal(GameStatus.InProgress, game.Status);

        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(2, 1));
        Assert.Equal(GameStatus.InProgress, game.Status);

        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(2, 1));
        Assert.Equal(GameStatus.InProgress, game.Status);

        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(2, 2));
        Assert.Equal(GameStatus.InProgress, game.Status);

        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(2, 2));
        Assert.Equal(GameStatus.InProgress, game.Status);

        // Only landing on hidden tile (3,3) ends the game
        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(3, 2));
        Assert.Equal(GameStatus.InProgress, game.Status);

        game.RecordDiceRoll(1);
        game.MovePlayer(p1.Id, new Position(3, 2));
        Assert.Equal(GameStatus.InProgress, game.Status);

        // P2 lands on hidden tile (3,3) — game ends
        game.RecordDiceRoll(1);
        game.MovePlayer(p2.Id, new Position(3, 3));

        Assert.Equal(GameStatus.Completed, game.Status);
        Assert.Equal(p2.Id, game.WinnerId);

        // The game only ends when WinnerId is set, which only happens in MovePlayerInternal
        // when tile.IsHiddenTile is true
        Assert.NotNull(game.WinnerId);

        // Verify GameAggregate has no public methods that trigger winning other than moving to hidden tile
        var gameType = typeof(GameAggregate);
        var winMethods = gameType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(m => !m.IsSpecialName &&
                        (m.Name.Contains("Win", StringComparison.OrdinalIgnoreCase) ||
                         m.Name.Contains("Score", StringComparison.OrdinalIgnoreCase) ||
                         m.Name.Contains("Victory", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        Assert.Empty(winMethods);
    }
}
