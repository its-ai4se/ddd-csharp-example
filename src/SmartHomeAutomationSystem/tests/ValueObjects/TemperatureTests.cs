using Xunit;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Tests.ValueObjects;

public class TemperatureTests
{
    [Fact]
    public void Temperature_WithValidCelsiusValue_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var temperature = new Temperature(22.5, TemperatureUnit.Celsius);

        // Assert
        Assert.Equal(22.5, temperature.Value);
        Assert.Equal(TemperatureUnit.Celsius, temperature.Unit);
    }

    [Fact]
    public void Temperature_WithValidFahrenheitValue_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var temperature = new Temperature(72.5, TemperatureUnit.Fahrenheit);

        // Assert
        Assert.Equal(72.5, temperature.Value);
        Assert.Equal(TemperatureUnit.Fahrenheit, temperature.Unit);
    }

    [Fact]
    public void Temperature_WithTooLowCelsiusValue_ShouldThrowDomainException()
    {
        // Arrange, Act & Assert
        Assert.Throws<DomainException>(() => new Temperature(-51, TemperatureUnit.Celsius));
    }

    [Fact]
    public void Temperature_WithTooHighCelsiusValue_ShouldThrowDomainException()
    {
        // Arrange, Act & Assert
        Assert.Throws<DomainException>(() => new Temperature(61, TemperatureUnit.Celsius));
    }

    [Fact]
    public void Temperature_ToCelsius_ShouldConvertCorrectly()
    {
        // Arrange
        var fahrenheitTemp = new Temperature(32, TemperatureUnit.Fahrenheit);

        // Act
        var celsiusTemp = fahrenheitTemp.ToCelsius();

        // Assert
        Assert.Equal(0, celsiusTemp.Value, 1);
        Assert.Equal(TemperatureUnit.Celsius, celsiusTemp.Unit);
    }

    [Fact]
    public void Temperature_ToFahrenheit_ShouldConvertCorrectly()
    {
        // Arrange
        var celsiusTemp = new Temperature(0, TemperatureUnit.Celsius);

        // Act
        var fahrenheitTemp = celsiusTemp.ToFahrenheit();

        // Assert
        Assert.Equal(32, fahrenheitTemp.Value, 1);
        Assert.Equal(TemperatureUnit.Fahrenheit, fahrenheitTemp.Unit);
    }
}
