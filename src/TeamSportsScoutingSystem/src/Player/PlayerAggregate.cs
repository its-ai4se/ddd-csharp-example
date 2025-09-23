using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Player;

public class PlayerAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public string? CurrentClub { get; private set; }
    public string? Nationality { get; private set; }
    public PlayerListType ListType { get; private set; }
    public DateTime AddedToListOn { get; private set; }
    public Guid? AddedByScoutId { get; private set; }

    private readonly List<PlayerAttribute> _attributes = new();

    public PlayerAggregate(Guid id, PersonName name, DateOnly dateOfBirth, PlayerListType listType, 
        string? currentClub = null, string? nationality = null, Guid? addedByScoutId = null) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DateOfBirth = dateOfBirth;
        CurrentClub = currentClub;
        Nationality = nationality;
        ListType = listType ?? throw new ArgumentNullException(nameof(listType));
        AddedToListOn = DateTime.UtcNow;
        AddedByScoutId = addedByScoutId;
    }

    public PlayerAggregate(PersonName name, DateOnly dateOfBirth, PlayerListType listType, 
        string? currentClub = null, string? nationality = null, Guid? addedByScoutId = null) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DateOfBirth = dateOfBirth;
        CurrentClub = currentClub;
        Nationality = nationality;
        ListType = listType ?? throw new ArgumentNullException(nameof(listType));
        AddedToListOn = DateTime.UtcNow;
        AddedByScoutId = addedByScoutId;
    }

    public IReadOnlyList<PlayerAttribute> Attributes => _attributes.AsReadOnly();

    public int Age => CalculateAge(DateOfBirth);

    public void UpdateName(PersonName newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public void UpdateCurrentClub(string? newClub)
    {
        CurrentClub = newClub;
    }

    public void UpdateNationality(string? newNationality)
    {
        Nationality = newNationality;
    }

    public void MoveToList(PlayerListType newListType)
    {
        ListType = newListType ?? throw new ArgumentNullException(nameof(newListType));
    }

    public void AddAttribute(PlayerAttribute attribute)
    {
        if (attribute == null)
        {
            throw new ArgumentNullException(nameof(attribute));
        }

        var existingAttribute = _attributes.FirstOrDefault(a => a.Name == attribute.Name);
        if (existingAttribute != null)
        {
            _attributes.Remove(existingAttribute);
        }

        _attributes.Add(attribute);
    }

    public void RemoveAttribute(string attributeName)
    {
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException("Attribute name cannot be empty or whitespace.", nameof(attributeName));
        }

        var attributeToRemove = _attributes.FirstOrDefault(a => a.Name == attributeName);
        if (attributeToRemove != null)
        {
            _attributes.Remove(attributeToRemove);
        }
    }

    public PlayerAttribute? GetAttribute(string attributeName)
    {
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException("Attribute name cannot be empty or whitespace.", nameof(attributeName));
        }

        return _attributes.FirstOrDefault(a => a.Name == attributeName);
    }

    private static int CalculateAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-age)) age--;
        return age;
    }

    public override string ToString() => $"Player: {Name} (ID: {Id})";
}
