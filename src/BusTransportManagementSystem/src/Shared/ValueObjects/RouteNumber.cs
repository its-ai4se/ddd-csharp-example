using BusTransportManagementSystem.Domain.Shared.Common;

namespace BusTransportManagementSystem.Domain.Shared.ValueObjects;

public class RouteNumber : ValueObject
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
            throw new ArgumentException("Route number must be a valid integer.", nameof(value));
        }

        if(intValue < 0)
        {
            throw new ArgumentException("Route number must be a non-negative integer.", nameof(value));
        }

        if (intValue > 9999)
        {
            throw new ArgumentException("Route number cannot exceed 9999.", nameof(value));
        }

        Value = intValue;
    }

    public static implicit operator string(RouteNumber? routeNumber) => routeNumber?.Value.ToString() ?? string.Empty;
    public static explicit operator RouteNumber(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
