using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public sealed class SessionStatus : ValueObject
{
    public static readonly SessionStatus Scheduled = new("Scheduled", skipValidation: true);
    public static readonly SessionStatus InProgress = new("InProgress", skipValidation: true);
    public static readonly SessionStatus Completed = new("Completed", skipValidation: true);
    public static readonly SessionStatus Cancelled = new("Cancelled", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Scheduled", "InProgress", "Completed", "Cancelled"];

    public string Value { get; }

    public SessionStatus(string value) : this(value, skipValidation: false) { }

    private SessionStatus(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Session status cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Session status '{value}' is not valid. Use Scheduled, InProgress, Completed, or Cancelled.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
