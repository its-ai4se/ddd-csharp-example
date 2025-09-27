using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.HallOfFame;

public class HighScoreEntry : Entity
{
    public Guid GameId { get; }
    public Guid PlayerId { get; }
    public Guid SessionId { get; }
    public Score Score { get; }
    public DateTime CompletedAt { get; }

    public HighScoreEntry(Guid id, Guid gameId, Guid playerId, Guid sessionId, Score score, DateTime completedAt) : base(id)
    {
        GameId = gameId;
        PlayerId = playerId;
        SessionId = sessionId;
        Score = score ?? throw new ArgumentNullException(nameof(score));
        CompletedAt = completedAt;
    }

    public HighScoreEntry(Guid gameId, Guid playerId, Guid sessionId, Score score, DateTime completedAt) : base()
    {
        GameId = gameId;
        PlayerId = playerId;
        SessionId = sessionId;
        Score = score ?? throw new ArgumentNullException(nameof(score));
        CompletedAt = completedAt;
    }

    public override string ToString() => $"High Score: Player {PlayerId}, Score {Score}, {CompletedAt:yyyy-MM-dd HH:mm}";
}
