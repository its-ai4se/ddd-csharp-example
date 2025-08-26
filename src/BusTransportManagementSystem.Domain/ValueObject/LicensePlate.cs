namespace BusTransportManagementSystem.Domain.ValueObject;

public class LicensePlate
{
    public string Value { get; }

    public LicensePlate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Driver name cannot be empty or whitespace.", nameof(value));
        }

        if(value.Length > 10)
        {
            throw new ArgumentException("License plate cannot exceed 10 characters.", nameof(value));
        }

        Value = value.Trim();
    }

    public static implicit operator string(LicensePlate licensePlate) => licensePlate.Value;
    public static explicit operator LicensePlate(string value) => new(value);

    public bool Equals (LicensePlate other)
    {
        if (other is null) return false;
        if(ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is LicensePlate other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static bool operator ==(LicensePlate left, LicensePlate right) => Equals(left, right);

    public static bool operator !=(LicensePlate left, LicensePlate right) => !Equals(left, right);
}