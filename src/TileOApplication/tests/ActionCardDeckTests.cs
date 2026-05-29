using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class ActionCardDeckTests
{
    private static GameAggregate CreateGameWithBoardAndPlayers()
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

        return game;
    }

    private static GameAggregate CreateFullGame()
    {
        var game = CreateGameWithBoardAndPlayers();
        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);
        return game;
    }

    [Fact]
    public void AD001_ActionCardDeckWith32Cards_ShouldAllowGameStart()
    {
        var game = CreateGameWithBoardAndPlayers();
        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        game.StartGame();

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(32, game.ActionCards.Count);
    }

    [Fact]
    public void AD002_StartGameWith31Cards_ShouldThrow()
    {
        var game = CreateGameWithBoardAndPlayers();
        for (int i = 0; i < 31; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        var ex = Assert.Throws<InvalidOperationException>(() => game.StartGame());
        Assert.Contains("32", ex.Message);
    }

    [Fact]
    public void AD003_AddActionCardWhenDeckAlreadyHas32_ShouldThrow()
    {
        var game = new GameAggregate();
        for (int i = 0; i < 32; i++)
            game.AddActionCard(ActionCardDescription.ExtraTurn);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            game.AddActionCard(ActionCardDescription.ExtraTurn));
        Assert.Contains("32", ex.Message);
    }

    [Fact]
    public void AD004_AddActionCardExtraTurnType_ShouldSucceed()
    {
        var game = new GameAggregate();

        game.AddActionCard(ActionCardDescription.ExtraTurn);

        Assert.Single(game.ActionCards);
        Assert.Equal(ActionCardType.ExtraTurn, game.ActionCards[0].Description.Type);
    }

    [Fact]
    public void AD005_AddActionCardConnectTilesType_ShouldSucceed()
    {
        var game = new GameAggregate();

        game.AddActionCard(ActionCardDescription.ConnectTiles);

        Assert.Single(game.ActionCards);
        Assert.Equal(ActionCardType.ConnectTiles, game.ActionCards[0].Description.Type);
    }

    [Fact]
    public void AD006_AddActionCardRemoveConnectionType_ShouldSucceed()
    {
        var game = new GameAggregate();

        game.AddActionCard(ActionCardDescription.RemoveConnection);

        Assert.Single(game.ActionCards);
        Assert.Equal(ActionCardType.RemoveConnection, game.ActionCards[0].Description.Type);
    }

    [Fact]
    public void AD007_AddActionCardTeleportType_ShouldSucceed()
    {
        var game = new GameAggregate();

        game.AddActionCard(ActionCardDescription.Teleport);

        Assert.Single(game.ActionCards);
        Assert.Equal(ActionCardType.Teleport, game.ActionCards[0].Description.Type);
    }

    [Fact]
    public void AD008_AddActionCardSkipTurnType_ShouldSucceed()
    {
        var game = new GameAggregate();

        game.AddActionCard(ActionCardDescription.SkipTurn);

        Assert.Single(game.ActionCards);
        Assert.Equal(ActionCardType.SkipTurn, game.ActionCards[0].Description.Type);
    }

    [Fact]
    public void AD009_ActionCardDescriptionOnlyFiveValidTypes_ShouldExist()
    {
        var validTypes = Enum.GetValues<ActionCardType>();

        Assert.Equal(5, validTypes.Length);
        Assert.Contains(ActionCardType.ExtraTurn, validTypes);
        Assert.Contains(ActionCardType.ConnectTiles, validTypes);
        Assert.Contains(ActionCardType.RemoveConnection, validTypes);
        Assert.Contains(ActionCardType.Teleport, validTypes);
        Assert.Contains(ActionCardType.SkipTurn, validTypes);
    }

    [Fact]
    public void AD010_SpareConnectionPiecesAtGameStart_ShouldBe32()
    {
        var game = CreateFullGame();
        game.StartGame();

        Assert.Equal(GameStatus.InProgress, game.Status);

        // The initial value before any connections is 32
        var freshBoard = new GameAggregate();
        Assert.Equal(32, freshBoard.Board.SpareConnectionPieces);
    }

    [Fact]
    public void AD011_ConnectTiles_ShouldDecrementSpareConnectionPieces()
    {
        var game = CreateFullGame();
        game.StartGame();

        var player1 = game.Players[0];
        game.RecordDiceRoll(1);

        var freshGame = new GameAggregate();
        freshGame.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        freshGame.Board.AddTile(new TileEntity(new Position(1, 0), new TileState(TileType.Regular)));

        var initialSpare = freshGame.Board.SpareConnectionPieces;
        freshGame.Board.ConnectTiles(new Position(0, 0), new Position(1, 0));

        Assert.Equal(initialSpare - 1, freshGame.Board.SpareConnectionPieces);
    }

    [Fact]
    public void AD012_DisconnectTilesShouldIncrementSpareConnectionPieces()
    {
        var game = new GameAggregate();
        game.Board.AddTile(new TileEntity(new Position(0, 0), new TileState(TileType.Regular)));
        game.Board.AddTile(new TileEntity(new Position(1, 0), new TileState(TileType.Regular)));

        game.Board.ConnectTiles(new Position(0, 0), new Position(1, 0));
        var spareAfterConnect = game.Board.SpareConnectionPieces;

        game.Board.DisconnectTiles(new Position(0, 0), new Position(1, 0));

        Assert.Equal(spareAfterConnect + 1, game.Board.SpareConnectionPieces);
    }
}
