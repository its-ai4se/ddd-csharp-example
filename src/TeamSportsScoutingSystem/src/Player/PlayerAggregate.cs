using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Player;

public class PlayerAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public PlayerListType ListType { get; private set; }
    public bool HasFinalRecommendation { get; private set; }

    private readonly List<PlayerAttribute> _attributes = [];

    public PlayerAggregate(Guid id, PersonName name, DateOnly dateOfBirth, PlayerListType listType) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DateOfBirth = dateOfBirth;
        ListType = listType ?? throw new ArgumentNullException(nameof(listType));
    }

    public PlayerAggregate(PersonName name, DateOnly dateOfBirth, PlayerListType listType) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DateOfBirth = dateOfBirth;
        ListType = listType ?? throw new ArgumentNullException(nameof(listType));
    }

    public IReadOnlyList<PlayerAttribute> Attributes => _attributes.AsReadOnly();

    public void MoveToList(PlayerListType newListType)
    {
        ListType = newListType ?? throw new ArgumentNullException(nameof(newListType));
    }

    public void AddAttribute(PlayerAttribute attribute)
    {
        ArgumentNullException.ThrowIfNull(attribute);
        var existing = _attributes.FirstOrDefault(a => a.Name == attribute.Name);
        if (existing != null) _attributes.Remove(existing);
        _attributes.Add(attribute);
    }

    public void MarkFinalRecommendationIssued()
    {
        HasFinalRecommendation = true;
    }

    public override string ToString() => $"Player: {Name} (ID: {Id})";
}
