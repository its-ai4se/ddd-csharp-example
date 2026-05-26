using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public sealed class ExpertiseLevel : ValueObject
{
    public static readonly ExpertiseLevel Beginner = new("Beginner", skipValidation: true);
    public static readonly ExpertiseLevel Intermediate = new("Intermediate", skipValidation: true);
    public static readonly ExpertiseLevel Advanced = new("Advanced", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Beginner", "Intermediate", "Advanced"];

    public string Value { get; }

    public ExpertiseLevel(string value) : this(value, skipValidation: false) { }

    private ExpertiseLevel(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Expertise level cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Expertise level '{value}' is not valid. Use Beginner, Intermediate, or Advanced.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
