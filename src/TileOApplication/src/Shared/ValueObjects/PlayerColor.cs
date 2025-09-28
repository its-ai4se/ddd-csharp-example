using TileOApplication.Domain.Shared.Common;

namespace TileOApplication.Domain.Shared.ValueObjects;

public class PlayerColor : ValueObject
{
    public string Name { get; }
    public string HexCode { get; }

    public PlayerColor(string name, string hexCode)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Player color name cannot be empty or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(hexCode) || !IsValidHexCode(hexCode))
        {
            throw new ArgumentException("Invalid hex code format.", nameof(hexCode));
        }

        Name = name.Trim();
        HexCode = hexCode.Trim().ToUpper();
    }

    private static bool IsValidHexCode(string hexCode)
    {
        return hexCode.Length == 7 && hexCode.StartsWith('#') && 
               hexCode.Substring(1).All(c => "0123456789ABCDEFabcdef".Contains(c));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return HexCode;
    }

    public override string ToString() => $"{Name} ({HexCode})";

    public static readonly PlayerColor Red = new("Red", "#FF0000");
    public static readonly PlayerColor Blue = new("Blue", "#0000FF");
    public static readonly PlayerColor Green = new("Green", "#00FF00");
    public static readonly PlayerColor Yellow = new("Yellow", "#FFFF00");
}
