using TeamSportsScoutingSystem.Domain.Shared.Common;

namespace TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

public class PlayerListType : ValueObject
{
    public string Type { get; }

    public PlayerListType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Player list type cannot be empty or whitespace.", nameof(type));
        }

        Type = type.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
    }

    public override string ToString() => Type;

    public static readonly PlayerListType LongList = new("Long List");
    public static readonly PlayerListType ShortList = new("Short List");
}
