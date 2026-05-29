using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Board;
using TileOApplication.Domain.ActionCard;

namespace TileOApplication.Domain.Game;

public enum GameStatus
{
    Designing,
    InProgress,
    Completed
}

public class GameAggregate : AggregateRoot
{
    public GameStatus Status { get; private set; }
    public Guid? CurrentPlayerId { get; private set; }
    public Guid? WinnerId { get; private set; }
    internal int CurrentTurn { get; private set; }
    internal BoardAggregate Board { get; private set; }

    private readonly List<PlayerAggregate> _players;
    private readonly List<ActionCardEntity> _actionCards;
    private Guid? _skippedPlayerId;

    private bool _diceRolledThisTurn;
    private int _currentDiceRoll;

    public GameAggregate() : base()
    {
        Status = GameStatus.Designing;
        Board = new BoardAggregate();
        _players = [];
        _actionCards = [];
    }

    public IReadOnlyList<PlayerAggregate> Players => _players.AsReadOnly();
    internal IReadOnlyList<ActionCardEntity> ActionCards => _actionCards.AsReadOnly();
    public int CurrentDiceRoll => _currentDiceRoll;
    public bool DiceRolledThisTurn => _diceRolledThisTurn;

    public void AddPlayer(PlayerAggregate player)
    {
        if (Status != GameStatus.Designing)
            throw new InvalidOperationException("Cannot add players when game is not in designing phase.");
        if (_players.Count >= 4)
            throw new InvalidOperationException("Maximum of 4 players allowed.");
        if (_players.Any(p => p.Color.Equals(player.Color)))
            throw new ArgumentException("A player with this color already exists.");
        _players.Add(player);
    }

    public void AddActionCard(ActionCardDescription description)
    {
        if (Status != GameStatus.Designing)
            throw new InvalidOperationException("Cannot add action cards when game is not in designing phase.");
        if (_actionCards.Count >= 32)
            throw new InvalidOperationException("Maximum of 32 action cards allowed.");
        _actionCards.Add(new ActionCardEntity(description));
    }

    public void StartGame()
    {
        if (Status != GameStatus.Designing)
            throw new InvalidOperationException("Game can only be started from designing phase.");
        if (_players.Count < 2)
            throw new InvalidOperationException("At least 2 players are required to start the game.");
        if (_actionCards.Count != 32)
            throw new InvalidOperationException("The action card deck must contain exactly 32 cards.");
        if (Board.HiddenTilePosition == null)
            throw new InvalidOperationException("Hidden tile must be set before starting the game.");
        if (!Board.Tiles.Values.Any(t => t.IsActionTile))
            throw new InvalidOperationException("At least one action tile must be placed on the board.");
        if (Board.StartingPositions.Count != _players.Count)
            throw new InvalidOperationException("Starting positions must be set for all players.");

        Status = GameStatus.InProgress;
        CurrentPlayerId = _players.OrderBy(p => p.TurnOrder).First().Id;
        CurrentTurn = 1;

        foreach (var player in _players)
        {
            var startingPosition = Board.StartingPositions.FirstOrDefault(sp => sp.Value == player.Id).Key;
            if (startingPosition != null)
            {
                player.PlaceAtStartingPosition(startingPosition);
                Board.GetTileAt(startingPosition)?.MarkAsVisited();
            }
        }
    }

    public void MovePlayer(Guid playerId, Position newPosition)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game must be in progress to move players.");
        if (CurrentPlayerId != playerId)
            throw new InvalidOperationException("It's not this player's turn.");
        if (!_diceRolledThisTurn)
            throw new InvalidOperationException("Player must roll the dice before moving.");

        MovePlayerInternal(playerId, newPosition);
    }

    internal void MovePlayerDirect(Guid playerId, Position newPosition)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game must be in progress to move players.");
        if (CurrentPlayerId != playerId)
            throw new InvalidOperationException("It's not this player's turn.");

        MovePlayerInternal(playerId, newPosition);
    }

    private void MovePlayerInternal(Guid playerId, Position newPosition)
    {
        var player = _players.FirstOrDefault(p => p.Id == playerId)
            ?? throw new ArgumentException("Player not found.");
        var tile = Board.GetTileAt(newPosition)
            ?? throw new ArgumentException("No tile exists at the specified position.");

        player.MoveTo(newPosition);
        tile.MarkAsVisited();

        if (tile.IsHiddenTile)
        {
            WinnerId = playerId;
            Status = GameStatus.Completed;
            return;
        }

        if (tile.IsActionTile)
        {
            var actionCard = _actionCards.FirstOrDefault(ac => !ac.IsUsed)
                ?? throw new InvalidOperationException("No action cards remaining in the deck.");
            actionCard.Use();
            tile.ConvertToRegular(tile.State.ActionTileTurnsRemaining);
            ApplyActionCard(playerId, actionCard);
            return;
        }

        AdvanceTurn();
    }

    public void RecordDiceRoll(int roll)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Game must be in progress to roll dice.");
        if (_diceRolledThisTurn)
            throw new InvalidOperationException("Dice has already been rolled this turn.");
        if (roll < 1 || roll > 6)
            throw new ArgumentOutOfRangeException(nameof(roll), "Dice roll must be between 1 and 6.");
        _diceRolledThisTurn = true;
        _currentDiceRoll = roll;
    }

    internal void AdvanceTurnAfterActionCard()
    {
        AdvanceTurn();
    }

    private void ApplyActionCard(Guid playerId, ActionCardEntity actionCard)
    {
        switch (actionCard.Description.Type)
        {
            case ActionCardType.ExtraTurn:
                ResetDiceRoll();
                break;
            case ActionCardType.SkipTurn:
                _skippedPlayerId = playerId;
                AdvanceTurn();
                break;
            default:
                AdvanceTurn();
                break;
        }
    }

    private void AdvanceTurn()
    {
        Board.TickActionTiles();

        var orderedPlayers = _players.OrderBy(p => p.TurnOrder).ToList();
        var currentIndex = orderedPlayers.FindIndex(p => p.Id == CurrentPlayerId);
        var nextIndex = (currentIndex + 1) % orderedPlayers.Count;

        if (nextIndex <= currentIndex)
            CurrentTurn++;

        if (orderedPlayers[nextIndex].Id == _skippedPlayerId)
        {
            _skippedPlayerId = null;
            var skippedIndex = nextIndex;
            nextIndex = (skippedIndex + 1) % orderedPlayers.Count;
            if (nextIndex <= skippedIndex)
                CurrentTurn++;
        }

        CurrentPlayerId = orderedPlayers[nextIndex].Id;
        ResetDiceRoll();
    }

    private void ResetDiceRoll()
    {
        _diceRolledThisTurn = false;
        _currentDiceRoll = 0;
    }

    public override string ToString() => $"Game ({Status})";
}
