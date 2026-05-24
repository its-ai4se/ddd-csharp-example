using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.ScoutingReport;

public class ScoutingReportAggregate : AggregateRoot
{
    public Guid PlayerId { get; private set; }
    public Guid ScoutId { get; private set; }
    public Guid ScoutingAssignmentId { get; private set; }
    public string Pros { get; private set; }
    public string Cons { get; private set; }
    public Recommendation Recommendation { get; private set; }
    public bool IsReviewed { get; private set; }

    public ScoutingReportAggregate(Guid id, Guid playerId, Guid scoutId, Guid scoutingAssignmentId,
        string pros, string cons, Recommendation recommendation) : base(id)
    {
        PlayerId = playerId;
        ScoutId = scoutId;
        ScoutingAssignmentId = scoutingAssignmentId;
        Pros = !string.IsNullOrWhiteSpace(pros) ? pros.Trim() : throw new DomainException("pros wajib diisi");
        Cons = !string.IsNullOrWhiteSpace(cons) ? cons.Trim() : throw new DomainException("cons wajib diisi");
        Recommendation = recommendation ?? throw new DomainException("rekomendasi wajib diisi");
    }

    public ScoutingReportAggregate(Guid playerId, Guid scoutId, Guid scoutingAssignmentId,
        string pros, string cons, Recommendation recommendation) : base()
    {
        PlayerId = playerId;
        ScoutId = scoutId;
        ScoutingAssignmentId = scoutingAssignmentId;
        Pros = !string.IsNullOrWhiteSpace(pros) ? pros.Trim() : throw new DomainException("pros wajib diisi");
        Cons = !string.IsNullOrWhiteSpace(cons) ? cons.Trim() : throw new DomainException("cons wajib diisi");
        Recommendation = recommendation ?? throw new DomainException("rekomendasi wajib diisi");
    }

    public void MarkAsReviewed()
    {
        IsReviewed = true;
    }

    public override string ToString() => $"Scouting Report for Player {PlayerId} (ID: {Id})";
}
