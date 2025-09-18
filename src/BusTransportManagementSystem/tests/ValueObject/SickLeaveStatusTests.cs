using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests.ValueObject;

public class SickLeaveStatusTests
{
    [Theory]
    [InlineData(SickLeaveStatusType.Active)]
    [InlineData(SickLeaveStatusType.OnSickLeave)]
    public void Constructor_WithValidEnum_ShouldCreateSickLeaveStatus(SickLeaveStatusType statusType)
    {
        // Act
        var status = new SickLeaveStatus(statusType);

        // Assert
        Assert.Equal(statusType, status.Value);
    }

    [Theory]
    [InlineData("Active", SickLeaveStatusType.Active)]
    [InlineData("OnSickLeave", SickLeaveStatusType.OnSickLeave)]
    [InlineData("active", SickLeaveStatusType.Active)] // Case insensitive
    [InlineData("ONSICKLEAVE", SickLeaveStatusType.OnSickLeave)] // Case insensitive
    [InlineData("  Active  ", SickLeaveStatusType.Active)] // With whitespace
    public void Constructor_WithValidString_ShouldCreateSickLeaveStatus(string input, SickLeaveStatusType expected)
    {
        // Act
        var status = new SickLeaveStatus(input);

        // Assert
        Assert.Equal(expected, status.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithNullOrWhitespace_ShouldThrowArgumentException(string? input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SickLeaveStatus(input!));
    }

    [Theory]
    [InlineData("InvalidStatus")]
    [InlineData("Sick")]
    [InlineData("Available")]
    public void Constructor_WithInvalidString_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new SickLeaveStatus(input));
        Assert.Contains("Invalid sick leave status", exception.Message);
        Assert.Contains("Active, OnSickLeave", exception.Message);
    }

    [Fact]
    public void StaticInstances_ShouldProvideConvenientAccess()
    {
        // Act & Assert
        Assert.Equal(SickLeaveStatusType.Active, SickLeaveStatus.Active.Value);
        Assert.Equal(SickLeaveStatusType.OnSickLeave, SickLeaveStatus.OnSickLeave.Value);
    }

    [Fact]
    public void IsActive_WithActiveStatus_ShouldReturnTrue()
    {
        // Arrange
        var status = new SickLeaveStatus(SickLeaveStatusType.Active);

        // Act & Assert
        Assert.True(status.IsActive());
        Assert.False(status.IsOnSickLeave());
    }

    [Fact]
    public void IsOnSickLeave_WithOnSickLeaveStatus_ShouldReturnTrue()
    {
        // Arrange
        var status = new SickLeaveStatus(SickLeaveStatusType.OnSickLeave);

        // Act & Assert
        Assert.True(status.IsOnSickLeave());
        Assert.False(status.IsActive());
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnEnumValue()
    {
        // Arrange
        var status = new SickLeaveStatus(SickLeaveStatusType.Active);

        // Act
        string result = status;

        // Assert
        Assert.Equal("Active", result);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldCreateSickLeaveStatus()
    {
        // Arrange
        var input = "OnSickLeave";

        // Act
        var status = (SickLeaveStatus)input;

        // Assert
        Assert.Equal(SickLeaveStatusType.OnSickLeave, status.Value);
    }

    [Fact]
    public void ExplicitConversion_FromEnum_ShouldCreateSickLeaveStatus()
    {
        // Arrange
        var input = SickLeaveStatusType.Active;

        // Act
        var status = (SickLeaveStatus)input;

        // Assert
        Assert.Equal(SickLeaveStatusType.Active, status.Value);
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var status1 = new SickLeaveStatus(SickLeaveStatusType.Active);
        var status2 = new SickLeaveStatus(SickLeaveStatusType.Active);

        // Act & Assert
        Assert.True(status1.Equals(status2));
        Assert.True(status1 == status2);
        Assert.False(status1 != status2);
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var status1 = new SickLeaveStatus(SickLeaveStatusType.Active);
        var status2 = new SickLeaveStatus(SickLeaveStatusType.OnSickLeave);

        // Act & Assert
        Assert.False(status1.Equals(status2));
        Assert.False(status1 == status2);
        Assert.True(status1 != status2);
    }

    [Fact]
    public void RequirementValidation_SickDriverCannotBeScheduled()
    {
        // Based on requirement: "If that is the case, the driver cannot be scheduled"
        
        // Arrange
        var activeStatus = SickLeaveStatus.Active;
        var sickStatus = SickLeaveStatus.OnSickLeave;

        // Act & Assert
        // Active drivers should be available for scheduling
        Assert.True(activeStatus.IsActive());
        Assert.False(activeStatus.IsOnSickLeave());
        
        // Sick drivers should NOT be available for scheduling
        Assert.False(sickStatus.IsActive());
        Assert.True(sickStatus.IsOnSickLeave());
    }
}
