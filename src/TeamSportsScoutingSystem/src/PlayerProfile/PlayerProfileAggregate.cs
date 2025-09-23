using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.PlayerProfile;

public class PlayerProfileAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid CreatedByHeadCoachId { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Position> _targetPositions = new();
    private readonly List<PlayerAttribute> _requiredAttributes = new();

    public PlayerProfileAggregate(Guid id, string name, string description, Guid createdByHeadCoachId) : base(id)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentException("Profile name cannot be empty or whitespace.", nameof(name));
        Description = !string.IsNullOrWhiteSpace(description) ? description.Trim() : throw new ArgumentException("Profile description cannot be empty or whitespace.", nameof(description));
        CreatedByHeadCoachId = createdByHeadCoachId;
        CreatedOn = DateTime.UtcNow;
        IsActive = true;
    }

    public PlayerProfileAggregate(string name, string description, Guid createdByHeadCoachId) : base()
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentException("Profile name cannot be empty or whitespace.", nameof(name));
        Description = !string.IsNullOrWhiteSpace(description) ? description.Trim() : throw new ArgumentException("Profile description cannot be empty or whitespace.", nameof(description));
        CreatedByHeadCoachId = createdByHeadCoachId;
        CreatedOn = DateTime.UtcNow;
        IsActive = true;
    }

    public IReadOnlyList<Position> TargetPositions => _targetPositions.AsReadOnly();
    public IReadOnlyList<PlayerAttribute> RequiredAttributes => _requiredAttributes.AsReadOnly();

    public void UpdateName(string newName)
    {
        Name = !string.IsNullOrWhiteSpace(newName) ? newName.Trim() : throw new ArgumentException("Profile name cannot be empty or whitespace.", nameof(newName));
    }

    public void UpdateDescription(string newDescription)
    {
        Description = !string.IsNullOrWhiteSpace(newDescription) ? newDescription.Trim() : throw new ArgumentException("Profile description cannot be empty or whitespace.", nameof(newDescription));
    }

    public void AddTargetPosition(Position position)
    {
        if (position == null)
        {
            throw new ArgumentNullException(nameof(position));
        }

        if (!_targetPositions.Any(p => p.Code == position.Code))
        {
            _targetPositions.Add(position);
        }
    }

    public void RemoveTargetPosition(string positionCode)
    {
        if (string.IsNullOrWhiteSpace(positionCode))
        {
            throw new ArgumentException("Position code cannot be empty or whitespace.", nameof(positionCode));
        }

        var positionToRemove = _targetPositions.FirstOrDefault(p => p.Code == positionCode);
        if (positionToRemove != null)
        {
            _targetPositions.Remove(positionToRemove);
        }
    }

    public void AddRequiredAttribute(PlayerAttribute attribute)
    {
        if (attribute == null)
        {
            throw new ArgumentNullException(nameof(attribute));
        }

        var existingAttribute = _requiredAttributes.FirstOrDefault(a => a.Name == attribute.Name);
        if (existingAttribute != null)
        {
            _requiredAttributes.Remove(existingAttribute);
        }

        _requiredAttributes.Add(attribute);
    }

    public void RemoveRequiredAttribute(string attributeName)
    {
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException("Attribute name cannot be empty or whitespace.", nameof(attributeName));
        }

        var attributeToRemove = _requiredAttributes.FirstOrDefault(a => a.Name == attributeName);
        if (attributeToRemove != null)
        {
            _requiredAttributes.Remove(attributeToRemove);
        }
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public bool MatchesPlayer(Player.PlayerAggregate player)
    {
        if (player == null) return false;

        // Check if player has any of the target positions
        var playerPositions = player.Attributes.Where(a => a.Name.ToLowerInvariant() == "position").Select(a => a.Value);
        var hasMatchingPosition = _targetPositions.Any(tp => playerPositions.Any(pp => pp.Equals(tp.Code, StringComparison.OrdinalIgnoreCase)));

        // Check if player has all required attributes
        var hasAllRequiredAttributes = _requiredAttributes.All(ra => 
            player.Attributes.Any(pa => pa.Name.Equals(ra.Name, StringComparison.OrdinalIgnoreCase) && 
                                      pa.Value.Equals(ra.Value, StringComparison.OrdinalIgnoreCase)));

        return hasMatchingPosition && hasAllRequiredAttributes;
    }

    public override string ToString() => $"Player Profile: {Name} (ID: {Id})";
}
