using TileOApplication.Domain.Game;
using TileOApplication.Domain.Board;
using TileOApplication.Domain.ActionCard;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Services;

public class GamePlayService
{
    // BR-014: Roll dice to determine movement
    public int RollDice(GameAggregate game)
    {
        var roll = new Random().Next(1, 7);
        game.RecordDiceRoll(roll);
        return roll;
    }

    // BR-015: Only move along connected tiles
    public List<Position> GetValidMoves(GameAggregate game, Guid playerId)
    {
        if (game.Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game must be in progress to get valid moves.");
        if (!game.DiceRolledThisTurn)
            throw new InvalidOperationException("Player must roll the dice before getting valid moves.");

        var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        if (player is null || player.CurrentPosition is null)
            return new List<Position>();

        var reachable = GetReachablePositions(game.Board, player.CurrentPosition, game.CurrentDiceRoll);
        reachable.Remove(player.CurrentPosition);
        return reachable;
    }

    // BR-015: BFS traversal along connected tiles
    private List<Position> GetReachablePositions(BoardAggregate board, Position start, int steps)
    {
        if (steps <= 0)
            return new List<Position> { start };

        var result = new List<Position>();
        foreach (var next in board.GetConnectedPositions(start))
            result.AddRange(GetReachablePositions(board, next, steps - 1));

        return result.Distinct().ToList();
    }

    // BR-023: Connect two adjacent tiles using a spare connection piece
    public void ExecuteConnectTilesCard(GameAggregate game, Guid playerId, Guid actionCardId, Position from, Position to)
    {
        var actionCard = ValidateActionCardTurn(game, playerId, actionCardId, ActionCardType.ConnectTiles);
        game.Board.ConnectTiles(from, to);
        actionCard.Use();
        game.AdvanceTurnAfterActionCard();
    }

    // BR-024: Remove connection piece and return to spare pile
    public void ExecuteRemoveConnectionCard(GameAggregate game, Guid playerId, Guid actionCardId, Position from, Position to)
    {
        var actionCard = ValidateActionCardTurn(game, playerId, actionCardId, ActionCardType.RemoveConnection);
        game.Board.DisconnectTiles(from, to);
        actionCard.Use();
        game.AdvanceTurnAfterActionCard();
    }

    // BR-022: Move to any tile other than current tile
    public void ExecuteTeleportCard(GameAggregate game, Guid playerId, Guid actionCardId, Position target)
    {
        var actionCard = ValidateActionCardTurn(game, playerId, actionCardId, ActionCardType.Teleport);

        var player = game.Players.FirstOrDefault(p => p.Id == playerId)
            ?? throw new ArgumentException("Player not found.");
        if (player.CurrentPosition is not null && player.CurrentPosition.Equals(target))
            throw new ArgumentException("Cannot teleport to current tile.");

        actionCard.Use();
        game.MovePlayerDirect(playerId, target);
    }

    private static ActionCardEntity ValidateActionCardTurn(GameAggregate game, Guid playerId, Guid actionCardId, ActionCardType expectedType)
    {
        if (game.Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game must be in progress to execute action cards.");
        if (game.CurrentPlayerId != playerId)
            throw new InvalidOperationException("It's not this player's turn.");

        var actionCard = game.ActionCards.FirstOrDefault(ac => ac.Id == actionCardId && !ac.IsUsed)
            ?? throw new ArgumentException("Action card not found or already used.");
        if (actionCard.Description.Type != expectedType)
            throw new ArgumentException($"Expected a {expectedType} action card.");

        return actionCard;
    }
}
