using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public sealed class PaymentStatus : ValueObject
{
    public static readonly PaymentStatus Pending = new("Pending", skipValidation: true);
    public static readonly PaymentStatus Completed = new("Completed", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Pending", "Completed"];

    public string Value { get; }

    public PaymentStatus(string value) : this(value, skipValidation: false) { }

    private PaymentStatus(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Payment status cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Payment status '{value}' is not valid. Use Pending or Completed.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
