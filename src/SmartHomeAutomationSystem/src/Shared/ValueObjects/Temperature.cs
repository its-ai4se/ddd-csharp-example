using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

public class Temperature : ValueObject
{
    public double Value { get; }
    public TemperatureUnit Unit { get; }

    public Temperature(double value, TemperatureUnit unit = TemperatureUnit.Celsius)
    {
        if (unit == TemperatureUnit.Celsius && (value < -50 || value > 60))
            throw new DomainException("Temperature must be between -50°C and 60°C.");
        
        if (unit == TemperatureUnit.Fahrenheit && (value < -58 || value > 140))
            throw new DomainException("Temperature must be between -58°F and 140°F.");
        
        Value = value;
        Unit = unit;
    }

    public Temperature ToCelsius()
    {
        if (Unit == TemperatureUnit.Celsius)
            return this;
        
        return new Temperature((Value - 32) * 5 / 9, TemperatureUnit.Celsius);
    }

    public Temperature ToFahrenheit()
    {
        if (Unit == TemperatureUnit.Fahrenheit)
            return this;
        
        return new Temperature(Value * 9 / 5 + 32, TemperatureUnit.Fahrenheit);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }

    public override string ToString() => $"{Value:F1}°{Unit}";
}

public enum TemperatureUnit
{
    Celsius,
    Fahrenheit
}
