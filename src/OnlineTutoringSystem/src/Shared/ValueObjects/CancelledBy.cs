using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public sealed class CancelledBy : ValueObject
{
    public static readonly CancelledBy Student = new("Student", skipValidation: true);
    public static readonly CancelledBy Tutor = new("Tutor", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Student", "Tutor"];

    public string Value { get; }

    public CancelledBy(string value) : this(value, skipValidation: false) { }

    private CancelledBy(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("CancelledBy cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"CancelledBy '{value}' is not valid. Use Student or Tutor.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
