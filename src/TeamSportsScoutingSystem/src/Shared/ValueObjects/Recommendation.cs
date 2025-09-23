using TeamSportsScoutingSystem.Domain.Shared.Common;

namespace TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

public class Recommendation : ValueObject
{
    public string Type { get; }
    public string Description { get; }

    public Recommendation(string type, string description)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Recommendation type cannot be empty or whitespace.", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Recommendation description cannot be empty or whitespace.", nameof(description));
        }

        Type = type.Trim();
        Description = description.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
    }

    public override string ToString() => $"{Type} - {Description}";

    // Common recommendation types
    public static readonly Recommendation KeyPlayer = new("Key Player", "Essential player for the team");
    public static readonly Recommendation FirstTeamPlayer = new("First Team Player", "Regular starter in the first team");
    public static readonly Recommendation ReserveTeamPlayer = new("Reserve Team Player", "Suitable for reserve team");
    public static readonly Recommendation ProspectivePlayer = new("Prospective Player", "Young player with potential");
    public static readonly Recommendation NotGoodSigning = new("Not Good Signing", "Not recommended for signing");
}
