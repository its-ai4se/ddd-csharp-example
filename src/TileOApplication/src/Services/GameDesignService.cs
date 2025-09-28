using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Board;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Services;

public class GameDesignService
{
    public void CreateDefaultActionCardDeck(GameAggregate game)
    {
        if (game.Status != GameStatus.Designing)
        {
            throw new InvalidOperationException("Game must be in designing phase to create action card deck.");
        }

        // Create a balanced deck of 32 action cards
        var actionCards = new List<ActionCardDescription>
        {
            // Extra turn cards (8 cards)
            ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn,
            ActionCardDescription.ExtraTurn,

            // Connect tiles cards (8 cards)
            ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles,
            ActionCardDescription.ConnectTiles,

            // Remove connection cards (8 cards)
            ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection,
            ActionCardDescription.RemoveConnection,

            // Teleport cards (6 cards)
            ActionCardDescription.Teleport,
            ActionCardDescription.Teleport,
            ActionCardDescription.Teleport,
            ActionCardDescription.Teleport,
            ActionCardDescription.Teleport,
            ActionCardDescription.Teleport,

            // Skip turn cards (2 cards)
            ActionCardDescription.SkipTurn,
            ActionCardDescription.SkipTurn
        };

        foreach (var actionCard in actionCards)
        {
            game.AddActionCard(actionCard);
        }
    }

    public void SetupDefaultPlayers(GameAggregate game)
    {
        if (game.Status != GameStatus.Designing)
        {
            throw new InvalidOperationException("Game must be in designing phase to setup players.");
        }

        var colors = new[] { PlayerColor.Red, PlayerColor.Blue, PlayerColor.Green, PlayerColor.Yellow };
        var playerNames = new[] { "Player 1", "Player 2", "Player 3", "Player 4" };

        for (int i = 0; i < 4; i++)
        {
            var player = new PlayerAggregate(playerNames[i], colors[i], i + 1);
            game.AddPlayer(player);
        }
    }

    public void CreateSampleBoard(GameAggregate game)
    {
        if (game.Status != GameStatus.Designing)
        {
            throw new InvalidOperationException("Game must be in designing phase to create board.");
        }

        // Create a simple 4x4 board
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                var position = new Position(x, y);
                var tileState = new TileState(TileType.Regular);
                var tile = new TileEntity(position, tileState);
                game.Board.AddTile(tile);
            }
        }

        // Set hidden tile
        game.Board.SetHiddenTile(new Position(2, 2));

        // Set starting positions for players
        var startingPositions = new[]
        {
            new Position(0, 0), // Player 1
            new Position(3, 0), // Player 2
            new Position(0, 3), // Player 3
            new Position(3, 3)  // Player 4
        };

        var players = game.Players.ToList();
        for (int i = 0; i < Math.Min(players.Count, startingPositions.Length); i++)
        {
            game.Board.SetStartingPosition(startingPositions[i], players[i].Id);
        }

        // Connect tiles in a simple pattern
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                game.Board.ConnectTiles(new Position(x, y), new Position(x + 1, y));
            }
        }

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                game.Board.ConnectTiles(new Position(x, y), new Position(x, y + 1));
            }
        }
    }
}
