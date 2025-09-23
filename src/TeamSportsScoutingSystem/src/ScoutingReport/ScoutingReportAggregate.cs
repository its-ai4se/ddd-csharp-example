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
    public DateTime SubmittedOn { get; private set; }
    public string? AdditionalNotes { get; private set; }

    private readonly List<PlayerAttribute> _observedAttributes = new();

    public ScoutingReportAggregate(Guid id, Guid playerId, Guid scoutId, Guid scoutingAssignmentId, 
        string pros, string cons, Recommendation recommendation, string? additionalNotes = null) : base(id)
    {
        PlayerId = playerId;
        ScoutId = scoutId;
        ScoutingAssignmentId = scoutingAssignmentId;
        Pros = !string.IsNullOrWhiteSpace(pros) ? pros.Trim() : throw new ArgumentException("Pros cannot be empty or whitespace.", nameof(pros));
        Cons = !string.IsNullOrWhiteSpace(cons) ? cons.Trim() : throw new ArgumentException("Cons cannot be empty or whitespace.", nameof(cons));
        Recommendation = recommendation ?? throw new ArgumentNullException(nameof(recommendation));
        AdditionalNotes = additionalNotes;
        SubmittedOn = DateTime.UtcNow;
    }

    public ScoutingReportAggregate(Guid playerId, Guid scoutId, Guid scoutingAssignmentId, 
        string pros, string cons, Recommendation recommendation, string? additionalNotes = null) : base()
    {
        PlayerId = playerId;
        ScoutId = scoutId;
        ScoutingAssignmentId = scoutingAssignmentId;
        Pros = !string.IsNullOrWhiteSpace(pros) ? pros.Trim() : throw new ArgumentException("Pros cannot be empty or whitespace.", nameof(pros));
        Cons = !string.IsNullOrWhiteSpace(cons) ? cons.Trim() : throw new ArgumentException("Cons cannot be empty or whitespace.", nameof(cons));
        Recommendation = recommendation ?? throw new ArgumentNullException(nameof(recommendation));
        AdditionalNotes = additionalNotes;
        SubmittedOn = DateTime.UtcNow;
    }

    public IReadOnlyList<PlayerAttribute> ObservedAttributes => _observedAttributes.AsReadOnly();

    public void UpdatePros(string newPros)
    {
        Pros = !string.IsNullOrWhiteSpace(newPros) ? newPros.Trim() : throw new ArgumentException("Pros cannot be empty or whitespace.", nameof(newPros));
    }

    public void UpdateCons(string newCons)
    {
        Cons = !string.IsNullOrWhiteSpace(newCons) ? newCons.Trim() : throw new ArgumentException("Cons cannot be empty or whitespace.", nameof(newCons));
    }

    public void UpdateRecommendation(Recommendation newRecommendation)
    {
        Recommendation = newRecommendation ?? throw new ArgumentNullException(nameof(newRecommendation));
    }

    public void UpdateAdditionalNotes(string? newNotes)
    {
        AdditionalNotes = newNotes;
    }

    public void AddObservedAttribute(PlayerAttribute attribute)
    {
        if (attribute == null)
        {
            throw new ArgumentNullException(nameof(attribute));
        }

        var existingAttribute = _observedAttributes.FirstOrDefault(a => a.Name == attribute.Name);
        if (existingAttribute != null)
        {
            _observedAttributes.Remove(existingAttribute);
        }

        _observedAttributes.Add(attribute);
    }

    public void RemoveObservedAttribute(string attributeName)
    {
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException("Attribute name cannot be empty or whitespace.", nameof(attributeName));
        }

        var attributeToRemove = _observedAttributes.FirstOrDefault(a => a.Name == attributeName);
        if (attributeToRemove != null)
        {
            _observedAttributes.Remove(attributeToRemove);
        }
    }

    public PlayerAttribute? GetObservedAttribute(string attributeName)
    {
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException("Attribute name cannot be empty or whitespace.", nameof(attributeName));
        }

        return _observedAttributes.FirstOrDefault(a => a.Name == attributeName);
    }

    public bool IsPositiveRecommendation => Recommendation.Type != Recommendation.NotGoodSigning.Type;

    public override string ToString() => $"Scouting Report for Player {PlayerId} (ID: {Id})";
}
