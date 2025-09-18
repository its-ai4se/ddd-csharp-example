using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests.ValueObject;

public class RouteNumberTests
{
    [Fact]
    public void Constructor_WithValidString_ShouldCreateRouteNumber()
    {
        // Arrange
        var validRouteNumber = "123";

        // Act
        var routeNumber = new RouteNumber(validRouteNumber);

        // Assert
        Assert.Equal(123, routeNumber.Value);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("9999")]
    [InlineData("123")]
    public void Constructor_WithValidRouteNumbers_ShouldCreateRouteNumber(string input)
    {
        // Act
        var routeNumber = new RouteNumber(input);

        // Assert
        Assert.Equal(int.Parse(input), routeNumber.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithNullOrWhitespace_ShouldThrowArgumentException(string? input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new RouteNumber(input!));
    }

    [Theory]
    [InlineData("12345678901")] // 11 characters - exceeds limit
    [InlineData("abcd")]
    [InlineData("123abc")]
    public void Constructor_WithInvalidInput_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new RouteNumber(input));
    }

    [Fact]
    public void Constructor_WithRouteNumberExceeding9999_ShouldThrowArgumentException()
    {
        // According to requirements: "The highest possible number for a bus route is 9999"
        // Note: Current implementation doesn't enforce this, but it should based on requirements
        
        // Arrange
        var routeNumber = "10000";

        // Act & Assert
        // This test will currently pass, but it shows a requirement gap
        Assert.Throws<ArgumentException>(() => new RouteNumber(routeNumber));
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValueAsString()
    {
        // Arrange
        var routeNumber = new RouteNumber("123");

        // Act
        string result = routeNumber;

        // Assert
        Assert.Equal("123", result);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldCreateRouteNumber()
    {
        // Arrange
        var input = "123";

        // Act
        var routeNumber = (RouteNumber)input;

        // Assert
        Assert.Equal(123, routeNumber.Value);
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var routeNumber1 = new RouteNumber("123");
        var routeNumber2 = new RouteNumber("123");

        // Act & Assert
        Assert.True(routeNumber1.Equals(routeNumber2));
        Assert.True(routeNumber1 == routeNumber2);
        Assert.False(routeNumber1 != routeNumber2);
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var routeNumber1 = new RouteNumber("123");
        var routeNumber2 = new RouteNumber("456");

        // Act & Assert
        Assert.False(routeNumber1.Equals(routeNumber2));
        Assert.False(routeNumber1 == routeNumber2);
        Assert.True(routeNumber1 != routeNumber2);
    }

    [Fact]
    public void GetHashCode_WithSameValue_ShouldBeSame()
    {
        // Arrange
        var routeNumber1 = new RouteNumber("123");
        var routeNumber2 = new RouteNumber("123");

        // Act & Assert
        Assert.Equal(routeNumber1.GetHashCode(), routeNumber2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValueAsString()
    {
        // Arrange
        var routeNumber = new RouteNumber("123");

        // Act
        var result = routeNumber.ToString();

        // Assert
        Assert.Equal("123", result);
    }
}
