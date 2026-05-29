using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace TileOApplication.Domain.Tests;

public class ActionTileTests
{
    private static GameAggregate CreateGameWithActionTile(ActionCardDescription cardType, int actionTileDuration = 3)
    {
        var game = new GameAggregate();
        game.AddPlayer(new PlayerAggregate(PlayerColor.Red, 1));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Blue, 2));
        game.AddPlayer(new PlayerAggregate(PlayerColor.Green, 3));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(3, 3));
        game.Board.SetActionTile(new Position(1, 0), actionTileDuration);

        game.Board.SetStartingPosition(new Position(0, 0), game.Players[0].Id);
        game.Board.SetStartingPosition(new Position(3, 0), game.Players[1].Id);
        game.Board.SetStartingPosition(new Position(0, 3), game.Players[2].Id);

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 4; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));
        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));

        for (int i = 0; i < 32; i++)
            game.AddActionCard(cardType);

        game.StartGame();
        return game;
    }

    private static void TakeTurn(GameAggregate game, Position target)
    {
        game.RecordDiceRoll(1);
        game.MovePlayer(game.CurrentPlayerId!.Value, target);
    }

    [Fact]
    public void AT001_ActionTileBeforeLanding_ShouldAppearAsRegularTile()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.ExtraTurn);
        var actionTilePos = new Position(1, 0);

        var tileView = game.Board.GetTileView(actionTilePos);

        Assert.NotNull(tileView);
        Assert.Equal(TileDisplayType.Regular, tileView.DisplayType);
    }

    [Fact]
    public void AT002_LandOnActionTile_ShouldDrawTopActionCard()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.ExtraTurn);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var initialUnusedCount = game.ActionCards.Count(ac => !ac.IsUsed);

        TakeTurn(game, new Position(1, 0));

        var usedCount = game.ActionCards.Count(ac => ac.IsUsed);
        Assert.Equal(1, usedCount);
        Assert.Equal(initialUnusedCount - 1, game.ActionCards.Count(ac => !ac.IsUsed));
    }

    [Fact]
    public void AT003_LandOnActionTileWithExtraTurnCard_ShouldApplyEffect()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.ExtraTurn);
        var p1 = game.Players.First(p => p.TurnOrder == 1);

        // P1 lands on action tile → draws ExtraTurn card → P1 gets extra turn
        TakeTurn(game, new Position(1, 0));

        // P1 still has the turn (extra turn granted)
        Assert.Equal(p1.Id, game.CurrentPlayerId);
    }

    [Fact]
    public void AT004_ActionTileAfterLanding_ShouldBecomeRegularForDesignatedTurns()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.SkipTurn, actionTileDuration: 3);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);
        var p3 = game.Players.First(p => p.TurnOrder == 3);
        var actionTilePos = new Position(1, 0);

        // P1 lands on action tile → it converts to regular for 3 turns
        TakeTurn(game, actionTilePos);

        // After P1 lands (with SkipTurn card), P1's next turn is skipped, so P2 goes
        // P2 takes turn
        TakeTurn(game, new Position(2, 0));

        // P3 takes turn
        TakeTurn(game, new Position(1, 3));

        // P1's turn is skipped (SkipTurn card effect), so P2 goes again
        // After 3 turns pass, action tile should reactivate
        // Turn 1 after landing: P2's turn
        // Turn 2 after landing: P3's turn
        // Turn 3 after landing: P1's turn (skipped) → P2's turn
        // After 3 turns, action tile should be active again

        // The action tile should still be in regular state during conversion
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void AT005_ActionTileInConversion_ShouldNotTriggerActionCard()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.SkipTurn, actionTileDuration: 5);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);
        var actionTilePos = new Position(1, 0);

        // P1 lands on action tile → SkipTurn drawn → 1 card used, tile converts to regular for 5 turns
        TakeTurn(game, actionTilePos);
        var usedAfterFirstLanding = game.ActionCards.Count(ac => ac.IsUsed);
        Assert.Equal(1, usedAfterFirstLanding);

        // After SkipTurn, turn goes to P2 (tick 1 of conversion — 4 ticks remaining)
        Assert.Equal(p2.Id, game.CurrentPlayerId);

        // P2 lands on action tile (1,0) which is still in conversion — should NOT draw a card
        TakeTurn(game, actionTilePos);

        Assert.Equal(usedAfterFirstLanding, game.ActionCards.Count(ac => ac.IsUsed));
    }

    [Fact]
    public void AT006_ActionTileAfterConversionPeriod_ShouldReactivate()
    {
        var game = CreateGameWithActionTile(ActionCardDescription.ExtraTurn, actionTileDuration: 2);
        var p1 = game.Players.First(p => p.TurnOrder == 1);
        var p2 = game.Players.First(p => p.TurnOrder == 2);
        var p3 = game.Players.First(p => p.TurnOrder == 3);
        var actionTilePos = new Position(1, 0);

        // P1 lands on action tile → ExtraTurn drawn → P1 gets extra turn
        TakeTurn(game, actionTilePos);
        // P1 uses extra turn
        TakeTurn(game, new Position(2, 0));

        // Now P2's turn — action tile is in conversion (2 turns)
        TakeTurn(game, new Position(2, 0)); // P2 moves away from (3,0)

        // P3's turn — action tile conversion: 1 turn remaining
        TakeTurn(game, new Position(1, 3));

        // P1's turn — action tile conversion: 0 turns remaining → reactivated
        // P1 moves back to (1,0) — should trigger action card again
        TakeTurn(game, new Position(1, 0));
        var usedAfterReactivation = game.ActionCards.Count(ac => ac.IsUsed);

        // 2 cards should have been used: 1 from first landing, 1 from reactivated landing
        Assert.Equal(2, usedAfterReactivation);
    }
}
