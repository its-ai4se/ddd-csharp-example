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
    public static readonly Recommendation NotGoodSigning = new("Not a Good Signing", "Not recommended for signing");

    private static readonly Dictionary<string, Recommendation> _validRecommendations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Key Player"] = KeyPlayer,
        ["First Team Player"] = FirstTeamPlayer,
        ["Reserve Team Player"] = ReserveTeamPlayer,
        ["Prospective Player"] = ProspectivePlayer,
        ["Not a Good Signing"] = NotGoodSigning,
    };

    public static Recommendation Parse(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("rekomendasi wajib diisi");
        if (!_validRecommendations.TryGetValue(type.Trim(), out var recommendation))
            throw new DomainException("nilai rekomendasi tidak valid");
        return recommendation;
    }
}
