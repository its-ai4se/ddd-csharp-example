using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.GameSession;

public class LevelProgress : Entity
{
    public LevelNumber LevelNumber { get; }
    public DateTime StartedAt { get; }
    public DateTime? CompletedAt { get; private set; }
    public Score Score { get; private set; }

    public LevelProgress(Guid id, LevelNumber levelNumber, DateTime startedAt) : base(id)
    {
        LevelNumber = levelNumber ?? throw new ArgumentNullException(nameof(levelNumber));
        StartedAt = startedAt;
        Score = Score.Zero;
    }

    public LevelProgress(LevelNumber levelNumber, DateTime startedAt) : base()
    {
        LevelNumber = levelNumber ?? throw new ArgumentNullException(nameof(levelNumber));
        StartedAt = startedAt;
        Score = Score.Zero;
    }

    public void AddScore(Score score)
    {
        Score = Score + score;
    }

    public void MarkCompleted()
    {
        CompletedAt = DateTime.UtcNow;
    }

    public bool IsCompleted => CompletedAt.HasValue;

    public override string ToString() => $"Level {LevelNumber}: Score {Score} (Completed: {IsCompleted})";
}
