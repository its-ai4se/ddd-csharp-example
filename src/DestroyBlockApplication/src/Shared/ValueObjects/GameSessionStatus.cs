using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public sealed class GameSessionStatus : ValueObject
{
    public static readonly GameSessionStatus Active = new("Active", skipValidation: true);
    public static readonly GameSessionStatus LevelCompleted = new("LevelCompleted", skipValidation: true); // BR-027: intermediate state after last block destroyed, before advancing
    public static readonly GameSessionStatus Paused = new("Paused", skipValidation: true);
    public static readonly GameSessionStatus Completed = new("Completed", skipValidation: true);
    public static readonly GameSessionStatus Failed = new("Failed", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Active", "LevelCompleted", "Paused", "Completed", "Failed"];

    public string Value { get; }

    public GameSessionStatus(string value) : this(value, skipValidation: false) { }

    private GameSessionStatus(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Game session status cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Game session status '{value}' is not valid.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
