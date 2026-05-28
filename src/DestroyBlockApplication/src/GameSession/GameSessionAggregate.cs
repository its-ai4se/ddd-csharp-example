using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.GameSession;

public class GameSessionAggregate : AggregateRoot
{
    public Guid PlayerId { get; }
    public Guid GameId { get; }
    public GameSessionStatus Status { get; private set; }
    public Score TotalScore { get; private set; }
    public Lives Lives { get; private set; }
    public LevelNumber CurrentLevel { get; private set; }
    public DateTime StartedAt { get; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? LastSavedAt { get; private set; }

    private readonly List<LevelProgress> _levelProgress = new();

    public GameSessionAggregate(Guid id, Guid playerId, Guid gameId) : base(id)
    {
        PlayerId = playerId;
        GameId = gameId;
        Status = GameSessionStatus.Active;
        TotalScore = Score.Zero;
        Lives = new Lives(3);
        CurrentLevel = new LevelNumber(1);
        StartedAt = DateTime.UtcNow;
        _levelProgress.Add(new LevelProgress(CurrentLevel, StartedAt));
    }

    public GameSessionAggregate(Guid playerId, Guid gameId) : base()
    {
        PlayerId = playerId;
        GameId = gameId;
        Status = GameSessionStatus.Active;
        TotalScore = Score.Zero;
        Lives = new Lives(3);
        CurrentLevel = new LevelNumber(1);
        StartedAt = DateTime.UtcNow;
        _levelProgress.Add(new LevelProgress(CurrentLevel, StartedAt));
    }

    public IReadOnlyList<LevelProgress> LevelProgress => _levelProgress.AsReadOnly();

    public void AddScore(Score score)
    {
        if (!Status.Equals(GameSessionStatus.Active))
            throw new InvalidOperationException("Cannot add score to inactive game session.");

        TotalScore = TotalScore + score;
        GetCurrentLevelProgress()?.AddScore(score);
    }

    public void LoseLife()
    {
        if (!Status.Equals(GameSessionStatus.Active))
            throw new InvalidOperationException("Cannot lose life in inactive game session.");

        Lives = --Lives;

        if (!Lives.IsAlive)
        {
            Status = GameSessionStatus.Failed;
            CompletedAt = DateTime.UtcNow;
        }
    }

    // BR-027, BR-032: called when the last block of a level is destroyed
    // Saves the game and waits for player confirmation (BR-031)
    public void CompleteLevel()
    {
        if (!Status.Equals(GameSessionStatus.Active))
            throw new InvalidOperationException("Cannot complete level in inactive game session.");

        GetCurrentLevelProgress()?.MarkCompleted();
        Status = GameSessionStatus.LevelCompleted;
        LastSavedAt = DateTime.UtcNow;
    }

    // BR-031: only callable after player confirms (i.e., status is LevelCompleted)
    public void AdvanceToNextLevel()
    {
        if (!Status.Equals(GameSessionStatus.LevelCompleted))
            throw new InvalidOperationException("Cannot advance to next level before current level is completed and player confirms.");

        CurrentLevel = ++CurrentLevel;
        _levelProgress.Add(new LevelProgress(CurrentLevel, DateTime.UtcNow));
        Status = GameSessionStatus.Active;
    }

    public void Pause()
    {
        if (!Status.Equals(GameSessionStatus.Active) && !Status.Equals(GameSessionStatus.LevelCompleted))
            throw new InvalidOperationException("Cannot pause inactive game session.");

        Status = GameSessionStatus.Paused;
        LastSavedAt = DateTime.UtcNow;
    }

    public void Resume()
    {
        if (!Status.Equals(GameSessionStatus.Paused))
            throw new InvalidOperationException("Cannot resume non-paused game session.");

        Status = GameSessionStatus.Active;
    }

    public void Complete()
    {
        if (!Status.Equals(GameSessionStatus.Active))
            throw new InvalidOperationException("Cannot complete inactive game session.");

        Status = GameSessionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        LastSavedAt = DateTime.UtcNow;
    }

    public bool IsActive => Status.Equals(GameSessionStatus.Active);
    public bool IsPaused => Status.Equals(GameSessionStatus.Paused);
    public bool IsCompleted => Status.Equals(GameSessionStatus.Completed);
    public bool IsFailed => Status.Equals(GameSessionStatus.Failed);

    private LevelProgress? GetCurrentLevelProgress()
        => _levelProgress.FirstOrDefault(lp => lp.LevelNumber.Equals(CurrentLevel));

    public override string ToString() => $"GameSession: Player {PlayerId}, Game {GameId}, Level {CurrentLevel}, Score {TotalScore}";
}
