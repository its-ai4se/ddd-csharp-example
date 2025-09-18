using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests.ValueObject;

public class LicensePlateTests
{
    [Theory]
    [InlineData("ABC123")]
    [InlineData("A")]
    [InlineData("1234567890")] // Exactly 10 characters - should be valid per requirements
    public void Constructor_WithValidLicensePlate_ShouldCreateLicensePlate(string input)
    {
        // Act
        var licensePlate = new LicensePlate(input);

        // Assert
        Assert.Equal(input.Trim(), licensePlate.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithNullOrWhitespace_ShouldThrowArgumentException(string? input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new LicensePlate(input!));
    }

    [Fact]
    public void Constructor_WithLicensePlateExceeding10Characters_ShouldThrowArgumentException()
    {
        // According to requirements: "licence plate number may be up to 10 characters long, inclusive"
        
        // Arrange
        var invalidLicensePlate = "12345678901"; // 11 characters

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new LicensePlate(invalidLicensePlate));
    }

    [Fact]
    public void Constructor_WithWhitespace_ShouldTrimInput()
    {
        // Arrange
        var input = "  ABC123  ";

        // Act
        var licensePlate = new LicensePlate(input);

        // Assert
        Assert.Equal("ABC123", licensePlate.Value);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var licensePlate = new LicensePlate("ABC123");

        // Act
        string result = licensePlate;

        // Assert
        Assert.Equal("ABC123", result);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldCreateLicensePlate()
    {
        // Arrange
        var input = "ABC123";

        // Act
        var licensePlate = (LicensePlate)input;

        // Assert
        Assert.Equal("ABC123", licensePlate.Value);
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var licensePlate1 = new LicensePlate("ABC123");
        var licensePlate2 = new LicensePlate("ABC123");

        // Act & Assert
        Assert.True(licensePlate1.Equals(licensePlate2));
        Assert.True(licensePlate1 == licensePlate2);
        Assert.False(licensePlate1 != licensePlate2);
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var licensePlate1 = new LicensePlate("ABC123");
        var licensePlate2 = new LicensePlate("XYZ789");

        // Act & Assert
        Assert.False(licensePlate1.Equals(licensePlate2));
        Assert.False(licensePlate1 == licensePlate2);
        Assert.True(licensePlate1 != licensePlate2);
    }

    [Fact]
    public void GetHashCode_WithSameValue_ShouldBeSame()
    {
        // Arrange
        var licensePlate1 = new LicensePlate("ABC123");
        var licensePlate2 = new LicensePlate("ABC123");

        // Act & Assert
        Assert.Equal(licensePlate1.GetHashCode(), licensePlate2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var licensePlate = new LicensePlate("ABC123");

        // Act
        var result = licensePlate.ToString();

        // Assert
        Assert.Equal("ABC123", result);
    }
}
