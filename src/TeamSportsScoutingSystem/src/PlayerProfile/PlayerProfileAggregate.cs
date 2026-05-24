using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.PlayerProfile;

public class PlayerProfileAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public Guid CreatedByHeadCoachId { get; private set; }

    private readonly List<Position> _targetPositions = [];
    private readonly List<PlayerAttribute> _requiredAttributes = [];

    public PlayerProfileAggregate(Guid id, string name, Guid createdByHeadCoachId) : base(id)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentException("Profile name cannot be empty or whitespace.", nameof(name));
        CreatedByHeadCoachId = createdByHeadCoachId;
    }

    public PlayerProfileAggregate(string name, Guid createdByHeadCoachId) : base()
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentException("Profile name cannot be empty or whitespace.", nameof(name));
        CreatedByHeadCoachId = createdByHeadCoachId;
    }

    public IReadOnlyList<Position> TargetPositions => _targetPositions.AsReadOnly();
    public IReadOnlyList<PlayerAttribute> RequiredAttributes => _requiredAttributes.AsReadOnly();

    public void AddTargetPosition(Position position)
    {
        ArgumentNullException.ThrowIfNull(position);
        if (!_targetPositions.Any(p => p.Code == position.Code))
            _targetPositions.Add(position);
    }

    public void AddRequiredAttribute(PlayerAttribute attribute)
    {
        ArgumentNullException.ThrowIfNull(attribute);
        var existing = _requiredAttributes.FirstOrDefault(a => a.Name == attribute.Name);
        if (existing != null) _requiredAttributes.Remove(existing);
        _requiredAttributes.Add(attribute);
    }

    public bool MatchesPlayer(Player.PlayerAggregate player)
    {
        if (player == null) return false;

        if (_targetPositions.Any())
        {
            var playerPositions = player.Attributes
                .Where(a => a.Name.ToLowerInvariant() == "position")
                .Select(a => a.Value);
            if (!_targetPositions.Any(tp =>
                    playerPositions.Any(pp => pp.Equals(tp.Code, StringComparison.OrdinalIgnoreCase))))
                return false;
        }

        return _requiredAttributes.All(ra =>
            player.Attributes.Any(pa =>
                pa.Name.Equals(ra.Name, StringComparison.OrdinalIgnoreCase) &&
                pa.Value.Equals(ra.Value, StringComparison.OrdinalIgnoreCase)));
    }

    public override string ToString() => $"Player Profile: {Name} (ID: {Id})";
}
