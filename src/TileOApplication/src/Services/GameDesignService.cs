using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Services;

public class GameDesignService
{
    // BR-010: Exactly 32 action cards; BR-011: From five predefined types
    public void CreateDefaultActionCardDeck(GameAggregate game)
    {
        if (game.Status != GameStatus.Designing)
            throw new InvalidOperationException("Game must be in designing phase to create action card deck.");

        var cards = new[]
        {
            ActionCardDescription.ExtraTurn, ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn, ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn, ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn, ActionCardDescription.ExtraTurn,
            ActionCardDescription.ConnectTiles, ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles, ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles, ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles, ActionCardDescription.ConnectTiles,
            ActionCardDescription.RemoveConnection, ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection, ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection, ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection, ActionCardDescription.RemoveConnection,
            ActionCardDescription.Teleport, ActionCardDescription.Teleport,
            ActionCardDescription.Teleport, ActionCardDescription.Teleport,
            ActionCardDescription.Teleport, ActionCardDescription.Teleport,
            ActionCardDescription.SkipTurn, ActionCardDescription.SkipTurn
        };

        foreach (var card in cards)
            game.AddActionCard(card);
    }

    // BR-002: 2-4 players; BR-003: Unique colors
    public void SetupDefaultPlayers(GameAggregate game)
    {
        if (game.Status != GameStatus.Designing)
            throw new InvalidOperationException("Game must be in designing phase to setup players.");

        var colors = new[] { PlayerColor.Red, PlayerColor.Blue, PlayerColor.Green, PlayerColor.Yellow };
        for (int i = 0; i < 4; i++)
            game.AddPlayer(new PlayerAggregate(colors[i], i + 1));
    }

    // BR-004/BR-005: Board with connected tiles
    // BR-006: One hidden tile; BR-007: Starting positions
    // BR-008/BR-009: Action tiles with inactive turn durations
    public void CreateSampleBoard(GameAggregate game)
    {
        if (game.Status != GameStatus.Designing)
            throw new InvalidOperationException("Game must be in designing phase to create board.");

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                game.Board.AddTile(new TileEntity(new Position(x, y), new TileState(TileType.Regular)));

        game.Board.SetHiddenTile(new Position(2, 2));

        game.Board.SetActionTile(new Position(1, 1), inactiveTurns: 2);
        game.Board.SetActionTile(new Position(3, 2), inactiveTurns: 3);

        var players = game.Players.ToList();
        var startingPositions = new[] { new Position(0, 0), new Position(3, 0), new Position(0, 3), new Position(3, 3) };
        for (int i = 0; i < players.Count; i++)
            game.Board.SetStartingPosition(startingPositions[i], players[i].Id);

        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 4; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 3; y++)
                game.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));
    }
}
