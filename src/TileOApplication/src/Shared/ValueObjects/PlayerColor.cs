using TileOApplication.Domain.Shared.Common;

namespace TileOApplication.Domain.Shared.ValueObjects;

// BR-003: Each playing piece must have a unique color to distinguish players
public class PlayerColor : ValueObject
{
    public string Name { get; }

    public PlayerColor(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player color name cannot be empty.", nameof(name));
        Name = name.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }

    public override string ToString() => Name;

    public static readonly PlayerColor Red = new("Red");
    public static readonly PlayerColor Blue = new("Blue");
    public static readonly PlayerColor Green = new("Green");
    public static readonly PlayerColor Yellow = new("Yellow");
}
