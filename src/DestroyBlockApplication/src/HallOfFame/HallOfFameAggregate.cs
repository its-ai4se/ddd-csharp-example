using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.HallOfFame;

public class HallOfFameAggregate : AggregateRoot
{
    public Guid GameId { get; }
    private readonly List<HighScoreEntry> _entries = new();

    public HallOfFameAggregate(Guid id, Guid gameId) : base(id)
    {
        GameId = gameId;
    }

    public HallOfFameAggregate(Guid gameId) : base()
    {
        GameId = gameId;
    }

    // BR-034: players compete for high score; ordered by score descending
    public IReadOnlyList<HighScoreEntry> Entries => _entries
        .OrderByDescending(e => e.Score.Value)
        .ThenBy(e => e.CompletedAt)
        .ToList()
        .AsReadOnly();

    // BR-030: total score displayed in hall of fame when game ends
    public void AddEntry(HighScoreEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (entry.GameId != GameId)
            throw new ArgumentException("Entry must belong to this game.", nameof(entry));
        if (_entries.Any(e => e.PlayerId == entry.PlayerId && e.SessionId == entry.SessionId))
            throw new InvalidOperationException("Entry for this session already exists.");

        _entries.Add(entry);
    }

    public override string ToString() => $"Hall of Fame for Game {GameId} ({_entries.Count} entries)";
}
