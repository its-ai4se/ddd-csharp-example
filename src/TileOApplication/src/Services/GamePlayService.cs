using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Board;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.Services;

public class GamePlayService
{
    public List<Position> GetValidMoves(GameAggregate game, Guid playerId, int diceRoll)
    {
        if (game.Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException("Game must be in progress to get valid moves.");
        }

        var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null || player.CurrentPosition == null)
        {
            return new List<Position>();
        }

        return GetValidMovesFromPosition(game.Board, player.CurrentPosition, diceRoll);
    }

    private List<Position> GetValidMovesFromPosition(BoardAggregate board, Position startPosition, int stepsRemaining)
    {
        if (stepsRemaining <= 0)
        {
            return new List<Position> { startPosition };
        }

        var validMoves = new List<Position>();
        var connectedPositions = board.GetConnectedPositions(startPosition);

        foreach (var connectedPosition in connectedPositions)
        {
            var movesFromConnected = GetValidMovesFromPosition(board, connectedPosition, stepsRemaining - 1);
            validMoves.AddRange(movesFromConnected);
        }

        return validMoves.Distinct().ToList();
    }

    public void ExecuteActionCard(GameAggregate game, Guid playerId, Guid actionCardId, Dictionary<string, object> parameters)
    {
        if (game.Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException("Game must be in progress to execute action cards.");
        }

        var actionCard = game.ActionCards.FirstOrDefault(ac => ac.Id == actionCardId);
        if (actionCard == null || actionCard.IsUsed)
        {
            throw new ArgumentException("Action card not found or already used.");
        }

        switch (actionCard.Description.Type)
        {
            case ActionCardType.ConnectTiles:
                ExecuteConnectTilesAction(game, parameters);
                break;
            case ActionCardType.RemoveConnection:
                ExecuteRemoveConnectionAction(game, parameters);
                break;
            case ActionCardType.Teleport:
                ExecuteTeleportAction(game, playerId, parameters);
                break;
            default:
                // ExtraTurn and SkipTurn are handled in GameAggregate
                break;
        }

        game.UseActionCard(playerId, actionCardId);
    }

    private void ExecuteConnectTilesAction(GameAggregate game, Dictionary<string, object> parameters)
    {
        if (!parameters.ContainsKey("fromPosition") || !parameters.ContainsKey("toPosition"))
        {
            throw new ArgumentException("ConnectTiles action requires fromPosition and toPosition parameters.");
        }

        var fromPosition = (Position)parameters["fromPosition"];
        var toPosition = (Position)parameters["toPosition"];

        game.Board.ConnectTiles(fromPosition, toPosition);
        game.Board.UseConnectionPiece();
    }

    private void ExecuteRemoveConnectionAction(GameAggregate game, Dictionary<string, object> parameters)
    {
        if (!parameters.ContainsKey("fromPosition") || !parameters.ContainsKey("toPosition"))
        {
            throw new ArgumentException("RemoveConnection action requires fromPosition and toPosition parameters.");
        }

        var fromPosition = (Position)parameters["fromPosition"];
        var toPosition = (Position)parameters["toPosition"];

        game.Board.DisconnectTiles(fromPosition, toPosition);
        game.Board.ReturnConnectionPiece();
    }

    private void ExecuteTeleportAction(GameAggregate game, Guid playerId, Dictionary<string, object> parameters)
    {
        if (!parameters.ContainsKey("targetPosition"))
        {
            throw new ArgumentException("Teleport action requires targetPosition parameter.");
        }

        var targetPosition = (Position)parameters["targetPosition"];
        var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        
        if (player == null)
        {
            throw new ArgumentException("Player not found.");
        }

        if (player.CurrentPosition == targetPosition)
        {
            throw new ArgumentException("Cannot teleport to current position.");
        }

        game.MovePlayer(playerId, targetPosition);
    }

    public int RollDie()
    {
        var random = new Random();
        return random.Next(1, 7); // Returns 1-6
    }

    public bool CanPlayerMove(GameAggregate game, Guid playerId, Position targetPosition)
    {
        if (game.Status != GameStatus.InProgress)
        {
            return false;
        }

        var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null || player.CurrentPosition == null)
        {
            return false;
        }

        var tile = game.Board.GetTileAt(targetPosition);
        if (tile == null)
        {
            return false;
        }

        // Check if position is reachable through connected tiles
        var connectedPositions = game.Board.GetConnectedPositions(player.CurrentPosition);
        return IsPositionReachable(game.Board, player.CurrentPosition, targetPosition, new HashSet<Position>());
    }

    private bool IsPositionReachable(BoardAggregate board, Position currentPosition, Position targetPosition, HashSet<Position> visited)
    {
        if (currentPosition.Equals(targetPosition))
        {
            return true;
        }

        if (visited.Contains(currentPosition))
        {
            return false;
        }

        visited.Add(currentPosition);
        var connectedPositions = board.GetConnectedPositions(currentPosition);

        foreach (var connectedPosition in connectedPositions)
        {
            if (IsPositionReachable(board, connectedPosition, targetPosition, new HashSet<Position>(visited)))
            {
                return true;
            }
        }

        return false;
    }
}
