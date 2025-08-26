using BusTransportManagementSystem.Domain.ValueObject;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests.ValueObject;

public class ShiftPeriodTests
{
    [Theory]
    [InlineData(ShiftPeriodType.Morning)]
    [InlineData(ShiftPeriodType.Afternoon)]
    [InlineData(ShiftPeriodType.Night)]
    public void Constructor_WithValidEnum_ShouldCreateShiftPeriod(ShiftPeriodType shiftType)
    {
        // Act
        var shiftPeriod = new ShiftPeriod(shiftType);

        // Assert
        Assert.Equal(shiftType, shiftPeriod.Value);
    }

    [Theory]
    [InlineData("Morning", ShiftPeriodType.Morning)]
    [InlineData("Afternoon", ShiftPeriodType.Afternoon)]
    [InlineData("Night", ShiftPeriodType.Night)]
    [InlineData("morning", ShiftPeriodType.Morning)] // Case insensitive
    [InlineData("AFTERNOON", ShiftPeriodType.Afternoon)] // Case insensitive
    [InlineData("  Night  ", ShiftPeriodType.Night)] // With whitespace
    public void Constructor_WithValidString_ShouldCreateShiftPeriod(string input, ShiftPeriodType expected)
    {
        // Act
        var shiftPeriod = new ShiftPeriod(input);

        // Assert
        Assert.Equal(expected, shiftPeriod.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithNullOrWhitespace_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ShiftPeriod(input));
    }

    [Theory]
    [InlineData("InvalidShift")]
    [InlineData("Evening")]
    public void Constructor_WithInvalidString_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new ShiftPeriod(input));
        Assert.Contains("Invalid shift period", exception.Message);
        Assert.Contains("Morning, Afternoon, Night", exception.Message);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnEnumValue()
    {
        // Arrange
        var shiftPeriod = new ShiftPeriod(ShiftPeriodType.Morning);

        // Act
        string result = shiftPeriod;

        // Assert
        Assert.Equal("Morning", result);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldCreateShiftPeriod()
    {
        // Arrange
        var input = "Afternoon";

        // Act
        var shiftPeriod = (ShiftPeriod)input;

        // Assert
        Assert.Equal(ShiftPeriodType.Afternoon, shiftPeriod.Value);
    }

    [Fact]
    public void ExplicitConversion_FromEnum_ShouldCreateShiftPeriod()
    {
        // Arrange
        var input = ShiftPeriodType.Night;

        // Act
        var shiftPeriod = (ShiftPeriod)input;

        // Assert
        Assert.Equal(ShiftPeriodType.Night, shiftPeriod.Value);
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var shiftPeriod1 = new ShiftPeriod(ShiftPeriodType.Morning);
        var shiftPeriod2 = new ShiftPeriod(ShiftPeriodType.Morning);

        // Act & Assert
        Assert.True(shiftPeriod1.Equals(shiftPeriod2));
        Assert.True(shiftPeriod1 == shiftPeriod2);
        Assert.False(shiftPeriod1 != shiftPeriod2);
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var shiftPeriod1 = new ShiftPeriod(ShiftPeriodType.Morning);
        var shiftPeriod2 = new ShiftPeriod(ShiftPeriodType.Afternoon);

        // Act & Assert
        Assert.False(shiftPeriod1.Equals(shiftPeriod2));
        Assert.False(shiftPeriod1 == shiftPeriod2);
        Assert.True(shiftPeriod1 != shiftPeriod2);
    }

    [Fact]
    public void GetHashCode_WithSameValue_ShouldBeSame()
    {
        // Arrange
        var shiftPeriod1 = new ShiftPeriod(ShiftPeriodType.Morning);
        var shiftPeriod2 = new ShiftPeriod(ShiftPeriodType.Morning);

        // Act & Assert
        Assert.Equal(shiftPeriod1.GetHashCode(), shiftPeriod2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnEnumValue()
    {
        // Arrange
        var shiftPeriod = new ShiftPeriod(ShiftPeriodType.Morning);

        // Act
        var result = shiftPeriod.ToString();

        // Assert
        Assert.Equal("Morning", result);
    }

    [Fact]
    public void RequirementValidation_ShouldSupportThreeShiftsPerRoute()
    {
        // Based on requirement: "For each route, there is a morning shift, an afternoon shift, and a night shift"
        
        // Arrange & Act
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        var nightShift = new ShiftPeriod(ShiftPeriodType.Night);

        // Assert
        Assert.Equal(ShiftPeriodType.Morning, morningShift.Value);
        Assert.Equal(ShiftPeriodType.Afternoon, afternoonShift.Value);
        Assert.Equal(ShiftPeriodType.Night, nightShift.Value);
        
        // All three shifts should be different
        Assert.NotEqual(morningShift, afternoonShift);
        Assert.NotEqual(afternoonShift, nightShift);
        Assert.NotEqual(morningShift, nightShift);
    }
}
