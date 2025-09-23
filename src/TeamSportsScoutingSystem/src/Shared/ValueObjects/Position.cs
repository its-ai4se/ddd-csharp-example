using TeamSportsScoutingSystem.Domain.Shared.Common;

namespace TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

public class Position : ValueObject
{
    public string Code { get; }
    public string Description { get; }

    public Position(string code, string description)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Position code cannot be empty or whitespace.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Position description cannot be empty or whitespace.", nameof(description));
        }

        Code = code.Trim().ToUpper();
        Description = description.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => $"{Code} - {Description}";

    // Common football positions
    public static readonly Position Goalkeeper = new("GK", "Goalkeeper");
    public static readonly Position LeftBack = new("LB", "Left Back");
    public static readonly Position RightBack = new("RB", "Right Back");
    public static readonly Position CenterBack = new("CB", "Center Back");
    public static readonly Position LeftWingBack = new("LWB", "Left Wing Back");
    public static readonly Position RightWingBack = new("RWB", "Right Wing Back");
    public static readonly Position DefensiveMidfielder = new("CDM", "Defensive Midfielder");
    public static readonly Position CentralMidfielder = new("CM", "Central Midfielder");
    public static readonly Position AttackingMidfielder = new("CAM", "Attacking Midfielder");
    public static readonly Position LeftMidfielder = new("LM", "Left Midfielder");
    public static readonly Position RightMidfielder = new("RM", "Right Midfielder");
    public static readonly Position LeftWinger = new("LW", "Left Winger");
    public static readonly Position RightWinger = new("RW", "Right Winger");
    public static readonly Position Striker = new("ST", "Striker");
    public static readonly Position CenterForward = new("CF", "Center Forward");
}
