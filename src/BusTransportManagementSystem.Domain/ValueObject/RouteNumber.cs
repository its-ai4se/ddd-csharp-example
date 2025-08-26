namespace BusTransportManagementSystem.Domain.ValueObject;

public class RouteNumber : IEquatable<RouteNumber>
{
    public int Value { get; }

    public RouteNumber(string value)    
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Route number cannot be empty or whitespace.", nameof(value));
        }

        if (!int.TryParse(value.Trim(), out var intValue))
        {
            throw new ArgumentException("Route number cannot exceed 10 characters.", nameof(value));
        }

        if (intValue > 9999)
        {
            throw new ArgumentException("Route number must be a valid integer.", nameof(value));
        }

        Value = intValue;
    }

    public static implicit operator string(RouteNumber? routeNumber) => routeNumber?.Value.ToString() ?? string.Empty;
    public static explicit operator RouteNumber(string value) => new(value);

    public bool Equals(RouteNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is RouteNumber other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool operator ==(RouteNumber left, RouteNumber right) => Equals(left, right);

    public static bool operator !=(RouteNumber left, RouteNumber right) => !Equals(left, right);
}
