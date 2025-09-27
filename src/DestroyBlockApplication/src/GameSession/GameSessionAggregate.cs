using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.GameSession;

public enum GameSessionStatus
{
    Active,
    Paused,
    Completed,
    Failed
}

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
        TotalScore = new Score(0);
        Lives = new Lives(3);
        CurrentLevel = new LevelNumber(1);
        StartedAt = DateTime.UtcNow;
    }

    public GameSessionAggregate(Guid playerId, Guid gameId) : base()
    {
        PlayerId = playerId;
        GameId = gameId;
        Status = GameSessionStatus.Active;
        TotalScore = new Score(0);
        Lives = new Lives(3);
        CurrentLevel = new LevelNumber(1);
        StartedAt = DateTime.UtcNow;
    }

    public IReadOnlyList<LevelProgress> LevelProgress => _levelProgress.AsReadOnly();

    public void AddScore(Score score)
    {
        if (Status != GameSessionStatus.Active)
        {
            throw new InvalidOperationException("Cannot add score to inactive game session.");
        }

        TotalScore = TotalScore + score;
    }

    public void LoseLife()
    {
        if (Status != GameSessionStatus.Active)
        {
            throw new InvalidOperationException("Cannot lose life in inactive game session.");
        }

        Lives = --Lives;

        if (!Lives.IsAlive)
        {
            Status = GameSessionStatus.Failed;
            CompletedAt = DateTime.UtcNow;
        }
    }

    public void AdvanceToNextLevel()
    {
        if (Status != GameSessionStatus.Active)
        {
            throw new InvalidOperationException("Cannot advance level in inactive game session.");
        }

        var currentLevelProgress = GetCurrentLevelProgress();
        if (currentLevelProgress != null)
        {
            currentLevelProgress.MarkCompleted();
        }

        CurrentLevel = ++CurrentLevel;
        _levelProgress.Add(new LevelProgress(CurrentLevel, DateTime.UtcNow));
    }

    public void Pause()
    {
        if (Status != GameSessionStatus.Active)
        {
            throw new InvalidOperationException("Cannot pause inactive game session.");
        }

        Status = GameSessionStatus.Paused;
        LastSavedAt = DateTime.UtcNow;
    }

    public void Resume()
    {
        if (Status != GameSessionStatus.Paused)
        {
            throw new InvalidOperationException("Cannot resume non-paused game session.");
        }

        Status = GameSessionStatus.Active;
    }

    public void Complete()
    {
        if (Status != GameSessionStatus.Active)
        {
            throw new InvalidOperationException("Cannot complete inactive game session.");
        }

        Status = GameSessionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        LastSavedAt = DateTime.UtcNow;
    }

    public void Save()
    {
        LastSavedAt = DateTime.UtcNow;
    }

    public LevelProgress? GetCurrentLevelProgress()
    {
        return _levelProgress.FirstOrDefault(lp => lp.LevelNumber.Equals(CurrentLevel));
    }

    public LevelProgress? GetLevelProgress(LevelNumber levelNumber)
    {
        return _levelProgress.FirstOrDefault(lp => lp.LevelNumber.Equals(levelNumber));
    }

    public bool IsActive => Status == GameSessionStatus.Active;
    public bool IsPaused => Status == GameSessionStatus.Paused;
    public bool IsCompleted => Status == GameSessionStatus.Completed;
    public bool IsFailed => Status == GameSessionStatus.Failed;

    public override string ToString() => $"GameSession: Player {PlayerId}, Game {GameId}, Level {CurrentLevel}, Score {TotalScore}";
}
