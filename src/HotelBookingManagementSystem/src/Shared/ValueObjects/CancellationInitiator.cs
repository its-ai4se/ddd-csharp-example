using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public sealed class CancellationInitiator : ValueObject
{
    public static readonly CancellationInitiator Hotel = new("Hotel", skipValidation: true);
    public static readonly CancellationInitiator Traveller = new("Traveller", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Hotel", "Traveller"];

    public string Value { get; }

    public CancellationInitiator(string value) : this(value, skipValidation: false) { }

    private CancellationInitiator(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Cancellation initiator cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Cancellation initiator '{value}' is not valid.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
