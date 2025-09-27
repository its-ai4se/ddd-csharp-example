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

    public IReadOnlyList<HighScoreEntry> Entries => _entries
        .OrderByDescending(e => e.Score.Value)
        .ThenBy(e => e.CompletedAt)
        .ToList()
        .AsReadOnly();

    public void AddEntry(HighScoreEntry entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        if (entry.GameId != GameId)
        {
            throw new ArgumentException("Entry must belong to this game.", nameof(entry));
        }

        if (_entries.Any(e => e.PlayerId == entry.PlayerId && e.SessionId == entry.SessionId))
        {
            throw new InvalidOperationException("Entry for this session already exists.");
        }

        _entries.Add(entry);
    }

    public void RemoveEntry(Guid sessionId)
    {
        var entryToRemove = _entries.FirstOrDefault(e => e.SessionId == sessionId);
        if (entryToRemove != null)
        {
            _entries.Remove(entryToRemove);
        }
    }

    public IEnumerable<HighScoreEntry> GetTopScores(int count = 10)
    {
        return _entries
            .OrderByDescending(e => e.Score.Value)
            .ThenBy(e => e.CompletedAt)
            .Take(count);
    }

    public HighScoreEntry? GetPlayerBestScore(Guid playerId)
    {
        return _entries
            .Where(e => e.PlayerId == playerId)
            .OrderByDescending(e => e.Score.Value)
            .FirstOrDefault();
    }

    public int GetPlayerRank(Guid playerId)
    {
        var sortedEntries = _entries
            .OrderByDescending(e => e.Score.Value)
            .ThenBy(e => e.CompletedAt)
            .ToList();

        for (int i = 0; i < sortedEntries.Count; i++)
        {
            if (sortedEntries[i].PlayerId == playerId)
            {
                return i + 1;
            }
        }

        return -1; // Player not found
    }

    public override string ToString() => $"Hall of Fame for Game {GameId} ({_entries.Count} entries)";
}
