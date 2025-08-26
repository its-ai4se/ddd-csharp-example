using BusTransportManagementSystem.Domain.ValueObject;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests.ValueObject;

public class RepairStatusTests
{
    [Theory]
    [InlineData(RepairStatusType.Operational)]
    [InlineData(RepairStatusType.UnderRepair)]
    [InlineData(RepairStatusType.OutOfService)]
    public void Constructor_WithValidEnum_ShouldCreateRepairStatus(RepairStatusType statusType)
    {
        // Act
        var status = new RepairStatus(statusType);

        // Assert
        Assert.Equal(statusType, status.Value);
    }

    [Theory]
    [InlineData("Operational", RepairStatusType.Operational)]
    [InlineData("UnderRepair", RepairStatusType.UnderRepair)]
    [InlineData("OutOfService", RepairStatusType.OutOfService)]
    [InlineData("operational", RepairStatusType.Operational)] // Case insensitive
    [InlineData("UNDERREPAIR", RepairStatusType.UnderRepair)] // Case insensitive
    [InlineData("  OutOfService  ", RepairStatusType.OutOfService)] // With whitespace
    public void Constructor_WithValidString_ShouldCreateRepairStatus(string input, RepairStatusType expected)
    {
        // Act
        var status = new RepairStatus(input);

        // Assert
        Assert.Equal(expected, status.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithNullOrWhitespace_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new RepairStatus(input));
    }

    [Theory]
    [InlineData("InvalidStatus")]
    [InlineData("Broken")]
    [InlineData("Working")]
    public void Constructor_WithInvalidString_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new RepairStatus(input));
        Assert.Contains("Invalid repair status", exception.Message);
        Assert.Contains("Operational, UnderRepair, OutOfService", exception.Message);
    }

    [Fact]
    public void StaticInstances_ShouldProvideConvenientAccess()
    {
        // Act & Assert
        Assert.Equal(RepairStatusType.Operational, RepairStatus.Operational.Value);
        Assert.Equal(RepairStatusType.UnderRepair, RepairStatus.UnderRepair.Value);
        Assert.Equal(RepairStatusType.OutOfService, RepairStatus.OutOfService.Value);
    }

    [Fact]
    public void IsOperational_WithOperationalStatus_ShouldReturnTrue()
    {
        // Arrange
        var status = new RepairStatus(RepairStatusType.Operational);

        // Act & Assert
        Assert.True(status.IsOperational());
        Assert.False(status.IsUnderRepair());
        Assert.False(status.IsOutOfService());
        Assert.True(status.IsAvailableForService());
    }

    [Fact]
    public void IsUnderRepair_WithUnderRepairStatus_ShouldReturnTrue()
    {
        // Arrange
        var status = new RepairStatus(RepairStatusType.UnderRepair);

        // Act & Assert
        Assert.False(status.IsOperational());
        Assert.True(status.IsUnderRepair());
        Assert.False(status.IsOutOfService());
        Assert.False(status.IsAvailableForService());
    }

    [Fact]
    public void IsOutOfService_WithOutOfServiceStatus_ShouldReturnTrue()
    {
        // Arrange
        var status = new RepairStatus(RepairStatusType.OutOfService);

        // Act & Assert
        Assert.False(status.IsOperational());
        Assert.False(status.IsUnderRepair());
        Assert.True(status.IsOutOfService());
        Assert.False(status.IsAvailableForService());
    }

    [Fact]
    public void IsAvailableForService_OnlyOperationalBuses_ShouldReturnTrue()
    {
        // Arrange
        var operationalStatus = new RepairStatus(RepairStatusType.Operational);
        var underRepairStatus = new RepairStatus(RepairStatusType.UnderRepair);
        var outOfServiceStatus = new RepairStatus(RepairStatusType.OutOfService);

        // Act & Assert
        Assert.True(operationalStatus.IsAvailableForService());
        Assert.False(underRepairStatus.IsAvailableForService());
        Assert.False(outOfServiceStatus.IsAvailableForService());
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnEnumValue()
    {
        // Arrange
        var status = new RepairStatus(RepairStatusType.Operational);

        // Act
        string result = status;

        // Assert
        Assert.Equal("Operational", result);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldCreateRepairStatus()
    {
        // Arrange
        var input = "UnderRepair";

        // Act
        var status = (RepairStatus)input;

        // Assert
        Assert.Equal(RepairStatusType.UnderRepair, status.Value);
    }

    [Fact]
    public void ExplicitConversion_FromEnum_ShouldCreateRepairStatus()
    {
        // Arrange
        var input = RepairStatusType.OutOfService;

        // Act
        var status = (RepairStatus)input;

        // Assert
        Assert.Equal(RepairStatusType.OutOfService, status.Value);
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var status1 = new RepairStatus(RepairStatusType.Operational);
        var status2 = new RepairStatus(RepairStatusType.Operational);

        // Act & Assert
        Assert.True(status1.Equals(status2));
        Assert.True(status1 == status2);
        Assert.False(status1 != status2);
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var status1 = new RepairStatus(RepairStatusType.Operational);
        var status2 = new RepairStatus(RepairStatusType.UnderRepair);

        // Act & Assert
        Assert.False(status1.Equals(status2));
        Assert.False(status1 == status2);
        Assert.True(status1 != status2);
    }

    [Fact]
    public void RequirementValidation_BusInRepairCannotBeAssigned()
    {
        // Based on requirement: "If that is the case, the bus cannot be assigned to a route"
        
        // Arrange
        var operationalStatus = RepairStatus.Operational;
        var underRepairStatus = RepairStatus.UnderRepair;
        var outOfServiceStatus = RepairStatus.OutOfService;

        // Act & Assert
        // Operational buses should be available for route assignment
        Assert.True(operationalStatus.IsAvailableForService());
        
        // Buses under repair should NOT be available for route assignment
        Assert.False(underRepairStatus.IsAvailableForService());
        
        // Out of service buses should NOT be available for route assignment
        Assert.False(outOfServiceStatus.IsAvailableForService());
    }
}
