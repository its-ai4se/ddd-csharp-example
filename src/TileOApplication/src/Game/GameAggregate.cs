using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Board;
using TileOApplication.Domain.ActionCard;

namespace TileOApplication.Domain.Game;

public enum GameStatus
{
    Designing,
    ReadyToPlay,
    InProgress,
    Completed
}

public class GameAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public GameStatus Status { get; private set; }
    public BoardAggregate Board { get; private set; }
    public Guid? CurrentPlayerId { get; private set; }
    public Guid? WinnerId { get; private set; }
    public int CurrentTurn { get; private set; }

    private readonly List<PlayerAggregate> _players;
    private readonly List<ActionCardEntity> _actionCards;
    private readonly List<ActionCardEntity> _usedActionCards;

    public GameAggregate(Guid id, string name) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Status = GameStatus.Designing;
        Board = new BoardAggregate();
        CurrentTurn = 0;
        _players = new List<PlayerAggregate>();
        _actionCards = new List<ActionCardEntity>();
        _usedActionCards = new List<ActionCardEntity>();
    }

    public GameAggregate(string name) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Game name cannot be empty or whitespace.", nameof(name));
        }
        Name = name.Trim();
        Status = GameStatus.Designing;
        Board = new BoardAggregate();
        CurrentTurn = 0;
        _players = new List<PlayerAggregate>();
        _actionCards = new List<ActionCardEntity>();
        _usedActionCards = new List<ActionCardEntity>();
    }

    public IReadOnlyList<PlayerAggregate> Players => _players.AsReadOnly();
    public IReadOnlyList<ActionCardEntity> ActionCards => _actionCards.AsReadOnly();
    public IReadOnlyList<ActionCardEntity> UsedActionCards => _usedActionCards.AsReadOnly();

    public void AddPlayer(PlayerAggregate player)
    {
        if (Status != GameStatus.Designing)
        {
            throw new InvalidOperationException("Cannot add players when game is not in designing phase.");
        }

        if (_players.Count >= 4)
        {
            throw new InvalidOperationException("Maximum of 4 players allowed.");
        }

        if (_players.Any(p => p.Color.Equals(player.Color)))
        {
            throw new ArgumentException("A player with this color already exists.");
        }

        _players.Add(player);
    }

    public void RemovePlayer(Guid playerId)
    {
        if (Status != GameStatus.Designing)
        {
            throw new InvalidOperationException("Cannot remove players when game is not in designing phase.");
        }

        var player = _players.FirstOrDefault(p => p.Id == playerId);
        if (player != null)
        {
            _players.Remove(player);
        }
    }

    public void AddActionCard(ActionCardDescription description)
    {
        if (Status != GameStatus.Designing)
        {
            throw new InvalidOperationException("Cannot add action cards when game is not in designing phase.");
        }

        if (_actionCards.Count >= 32)
        {
            throw new InvalidOperationException("Maximum of 32 action cards allowed.");
        }

        _actionCards.Add(new ActionCardEntity(description));
    }

    public void RemoveActionCard(Guid actionCardId)
    {
        if (Status != GameStatus.Designing)
        {
            throw new InvalidOperationException("Cannot remove action cards when game is not in designing phase.");
        }

        var actionCard = _actionCards.FirstOrDefault(ac => ac.Id == actionCardId);
        if (actionCard != null)
        {
            _actionCards.Remove(actionCard);
        }
    }

    public void StartGame()
    {
        if (Status != GameStatus.Designing)
        {
            throw new InvalidOperationException("Game can only be started from designing phase.");
        }

        if (_players.Count < 2)
        {
            throw new InvalidOperationException("At least 2 players are required to start the game.");
        }

        if (_actionCards.Count == 0)
        {
            throw new InvalidOperationException("At least one action card is required to start the game.");
        }

        if (Board.HiddenTilePosition == null)
        {
            throw new InvalidOperationException("Hidden tile must be set before starting the game.");
        }

        if (Board.StartingPositions.Count != _players.Count)
        {
            throw new InvalidOperationException("Starting positions must be set for all players.");
        }

        Status = GameStatus.ReadyToPlay;
        CurrentPlayerId = _players.OrderBy(p => p.TurnOrder).First().Id;
        CurrentTurn = 1;

        // Place players at their starting positions
        foreach (var player in _players)
        {
            var startingPosition = Board.StartingPositions.FirstOrDefault(sp => sp.Value == player.Id).Key;
            if (startingPosition != null)
            {
                player.PlaceAtStartingPosition(startingPosition);
            }
        }
    }

    public void BeginPlay()
    {
        if (Status != GameStatus.ReadyToPlay)
        {
            throw new InvalidOperationException("Game must be ready to play before beginning.");
        }

        Status = GameStatus.InProgress;
    }

    public void MovePlayer(Guid playerId, Position newPosition)
    {
        if (Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException("Game must be in progress to move players.");
        }

        if (CurrentPlayerId != playerId)
        {
            throw new InvalidOperationException("It's not this player's turn.");
        }

        var player = _players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
        {
            throw new ArgumentException("Player not found.");
        }

        var tile = Board.GetTileAt(newPosition);
        if (tile == null)
        {
            throw new ArgumentException("No tile exists at the specified position.");
        }

        player.MoveTo(newPosition);
        tile.MarkAsVisited();

        // Check if player landed on hidden tile
        if (tile.IsHiddenTile)
        {
            WinnerId = playerId;
            Status = GameStatus.Completed;
            return;
        }

        // Check if player landed on action tile
        if (tile.IsActionTile)
        {
            var actionCard = _actionCards.FirstOrDefault(ac => !ac.IsUsed);
            if (actionCard != null)
            {
                actionCard.Use();
                _usedActionCards.Add(actionCard);
                // Action tile becomes regular tile for specified turns
                tile.ConvertToRegular(3); // Default 3 turns
            }
        }

        // Move to next player
        MoveToNextPlayer();
    }

    public void UseActionCard(Guid playerId, Guid actionCardId)
    {
        if (Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException("Game must be in progress to use action cards.");
        }

        if (CurrentPlayerId != playerId)
        {
            throw new InvalidOperationException("It's not this player's turn.");
        }

        var actionCard = _actionCards.FirstOrDefault(ac => ac.Id == actionCardId);
        if (actionCard == null || actionCard.IsUsed)
        {
            throw new ArgumentException("Action card not found or already used.");
        }

        actionCard.Use();
        _usedActionCards.Add(actionCard);

        // Handle different action card types
        switch (actionCard.Description.Type)
        {
            case ActionCardType.ExtraTurn:
                // Player gets another turn - don't move to next player
                break;
            case ActionCardType.SkipTurn:
                var player = _players.FirstOrDefault(p => p.Id == playerId);
                player?.SkipNextTurn();
                MoveToNextPlayer();
                break;
            case ActionCardType.Teleport:
                // This would be handled by the MovePlayer method with special validation
                break;
            default:
                MoveToNextPlayer();
                break;
        }
    }

    private void MoveToNextPlayer()
    {
        var activePlayers = _players.Where(p => p.IsActive).OrderBy(p => p.TurnOrder).ToList();
        var currentIndex = activePlayers.FindIndex(p => p.Id == CurrentPlayerId);
        
        if (currentIndex == -1 || currentIndex == activePlayers.Count - 1)
        {
            CurrentPlayerId = activePlayers.First().Id;
            CurrentTurn++;
        }
        else
        {
            CurrentPlayerId = activePlayers[currentIndex + 1].Id;
        }

        // Reactivate all players for next turn
        foreach (var player in _players)
        {
            player.Activate();
        }
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Game name cannot be empty or whitespace.", nameof(newName));
        }
        Name = newName.Trim();
    }

    public override string ToString() => $"{Name} ({Status})";
}
