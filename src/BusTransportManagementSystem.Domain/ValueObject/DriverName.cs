namespace BusTransportManagementSystem.Domain.ValueObject;

public class DriverName
{
    public string Value { get; }

    public DriverName(string value)
    {
		if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Driver name cannot be empty or whitespace.", nameof(value));
        }
        Value = value.Trim();
    }

    public static implicit operator string(DriverName driverName) => driverName.Value;
    public static explicit operator DriverName(string value) => new(value);

    public bool Equals (DriverName other)
    {
        if (other is null) return false;
        if(ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is DriverName other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static bool operator ==(DriverName left, DriverName right) => Equals(left, right);

    public static bool operator !=(DriverName left, DriverName right) => !Equals(left, right);
}