using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public sealed class PaymentMethod : ValueObject
{
    public static readonly PaymentMethod CreditCard = new("CreditCard", skipValidation: true);
    public static readonly PaymentMethod BankTransfer = new("BankTransfer", skipValidation: true);

    private static readonly HashSet<string> _valid = ["CreditCard", "BankTransfer"];

    public string Value { get; }

    public PaymentMethod(string value) : this(value, skipValidation: false) { }

    private PaymentMethod(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Payment method cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Payment method '{value}' is not supported. Use CreditCard or BankTransfer.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
