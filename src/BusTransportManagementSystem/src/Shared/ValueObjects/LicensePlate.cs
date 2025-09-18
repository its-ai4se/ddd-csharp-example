using BusTransportManagementSystem.Domain.Shared.Common;

namespace BusTransportManagementSystem.Domain.Shared.ValueObjects;

public class LicensePlate : ValueObject
{
    public string Value { get; }

    public LicensePlate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("License plate cannot be empty or whitespace.", nameof(value));
        }

        if (value.Length > 10)
        {
            throw new ArgumentException("License plate cannot exceed 10 characters.", nameof(value));
        }

        Value = value.Trim();
    }

    public static implicit operator string(LicensePlate licensePlate) => licensePlate.Value;
    public static explicit operator LicensePlate(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
