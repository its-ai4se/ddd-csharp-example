using BusTransportManagementSystem.Domain.Entity;
using BusTransportManagementSystem.Domain.ValueObject;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests.Entity;

public class BusTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateBus()
    {
        // Arrange
        var id = Guid.NewGuid();
        var licensePlate = new LicensePlate("ABC123");
        var repairStatus = RepairStatus.Operational;

        // Act
        var bus = new Bus(id, licensePlate, repairStatus);

        // Assert
        Assert.Equal(id, bus.Id);
        Assert.Equal(licensePlate, bus.LicensePlate);
        Assert.Equal(repairStatus, bus.RepairStatus);
    }

    [Fact]
    public void Constructor_WithoutRepairStatus_ShouldDefaultToOperational()
    {
        // Arrange
        var id = Guid.NewGuid();
        var licensePlate = new LicensePlate("ABC123");

        // Act
        var bus = new Bus(id, licensePlate);

        // Assert
        Assert.Equal(RepairStatus.Operational, bus.RepairStatus);
        Assert.True(bus.IsOperational());
        Assert.True(bus.IsAvailableForService());
    }

    [Fact]
    public void Constructor_WithLicensePlateOnly_ShouldGenerateIdAndDefaultToOperational()
    {
        // Arrange
        var licensePlate = new LicensePlate("ABC123");

        // Act
        var bus = new Bus(licensePlate);

        // Assert
        Assert.NotEqual(Guid.Empty, bus.Id);
        Assert.Equal(licensePlate, bus.LicensePlate);
        Assert.Equal(RepairStatus.Operational, bus.RepairStatus);
        Assert.True(bus.IsAvailableForService());
    }

    [Fact]
    public void Constructor_WithEmptyId_ShouldThrowArgumentException()
    {
        // Arrange
        var emptyId = Guid.Empty;
        var licensePlate = new LicensePlate("ABC123");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Bus(emptyId, licensePlate));
    }

    [Fact]
    public void Constructor_WithNullLicensePlate_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        LicensePlate nullLicensePlate = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Bus(id, nullLicensePlate));
    }

    [Fact]
    public void UpdateLicensePlate_WithValidLicensePlate_ShouldUpdateLicensePlate()
    {
        // Arrange
        var bus = new Bus(new LicensePlate("ABC123"));
        var newLicensePlate = new LicensePlate("XYZ789");

        // Act
        bus.UpdateLicensePlate(newLicensePlate);

        // Assert
        Assert.Equal(newLicensePlate, bus.LicensePlate);
    }

    [Fact]
    public void UpdateLicensePlate_WithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var bus = new Bus(new LicensePlate("ABC123"));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bus.UpdateLicensePlate(null!));
    }

    [Fact]
    public void SetUnderRepair_ShouldChangeStatusToUnderRepair()
    {
        // Arrange
        var bus = new Bus(new LicensePlate("ABC123"));
        Assert.True(bus.IsOperational()); // Initially operational

        // Act
        bus.SetUnderRepair();

        // Assert
        Assert.Equal(RepairStatus.UnderRepair, bus.RepairStatus);
        Assert.False(bus.IsOperational());
        Assert.True(bus.IsUnderRepair());
        Assert.False(bus.IsAvailableForService());
    }

    [Fact]
    public void SetOutOfService_ShouldChangeStatusToOutOfService()
    {
        // Arrange
        var bus = new Bus(new LicensePlate("ABC123"));

        // Act
        bus.SetOutOfService();

        // Assert
        Assert.Equal(RepairStatus.OutOfService, bus.RepairStatus);
        Assert.False(bus.IsOperational());
        Assert.True(bus.IsOutOfService());
        Assert.False(bus.IsAvailableForService());
    }

    [Fact]
    public void SetOperational_ShouldChangeStatusToOperational()
    {
        // Arrange
        var bus = new Bus(new LicensePlate("ABC123"), RepairStatus.UnderRepair);
        Assert.False(bus.IsOperational()); // Initially not operational

        // Act
        bus.SetOperational();

        // Assert
        Assert.Equal(RepairStatus.Operational, bus.RepairStatus);
        Assert.True(bus.IsOperational());
        Assert.False(bus.IsUnderRepair());
        Assert.False(bus.IsOutOfService());
        Assert.True(bus.IsAvailableForService());
    }

    [Fact]
    public void IsOperational_WithOperationalStatus_ShouldReturnTrue()
    {
        // Arrange
        var bus = new Bus(new LicensePlate("ABC123"), RepairStatus.Operational);

        // Act & Assert
        Assert.True(bus.IsOperational());
        Assert.False(bus.IsUnderRepair());
        Assert.False(bus.IsOutOfService());
    }

    [Fact]
    public void IsUnderRepair_WithUnderRepairStatus_ShouldReturnTrue()
    {
        // Arrange
        var bus = new Bus(new LicensePlate("ABC123"), RepairStatus.UnderRepair);

        // Act & Assert
        Assert.False(bus.IsOperational());
        Assert.True(bus.IsUnderRepair());
        Assert.False(bus.IsOutOfService());
    }

    [Fact]
    public void IsOutOfService_WithOutOfServiceStatus_ShouldReturnTrue()
    {
        // Arrange
        var bus = new Bus(new LicensePlate("ABC123"), RepairStatus.OutOfService);

        // Act & Assert
        Assert.False(bus.IsOperational());
        Assert.False(bus.IsUnderRepair());
        Assert.True(bus.IsOutOfService());
    }

    [Fact]
    public void IsAvailableForService_OnlyOperationalBuses_ShouldReturnTrue()
    {
        // Arrange
        var operationalBus = new Bus(new LicensePlate("ABC123"), RepairStatus.Operational);
        var repairBus = new Bus(new LicensePlate("DEF456"), RepairStatus.UnderRepair);
        var outOfServiceBus = new Bus(new LicensePlate("GHI789"), RepairStatus.OutOfService);

        // Act & Assert
        Assert.True(operationalBus.IsAvailableForService());
        Assert.False(repairBus.IsAvailableForService());
        Assert.False(outOfServiceBus.IsAvailableForService());
    }

    [Fact]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var bus1 = new Bus(id, new LicensePlate("ABC123"));
        var bus2 = new Bus(id, new LicensePlate("XYZ789")); // Different license plate, same ID

        // Act & Assert
        Assert.True(bus1.Equals(bus2));
        Assert.True(bus1 == bus2);
        Assert.False(bus1 != bus2);
    }

    [Fact]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // Arrange
        var bus1 = new Bus(new LicensePlate("ABC123"));
        var bus2 = new Bus(new LicensePlate("ABC123")); // Same license plate, different ID

        // Act & Assert
        Assert.False(bus1.Equals(bus2));
        Assert.False(bus1 == bus2);
        Assert.True(bus1 != bus2);
    }

    [Fact]
    public void GetHashCode_WithSameId_ShouldBeSame()
    {
        // Arrange
        var id = Guid.NewGuid();
        var bus1 = new Bus(id, new LicensePlate("ABC123"));
        var bus2 = new Bus(id, new LicensePlate("XYZ789"));

        // Act & Assert
        Assert.Equal(bus1.GetHashCode(), bus2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldIncludeBusInformation()
    {
        // Arrange
        var bus = new Bus(new LicensePlate("ABC123"), RepairStatus.UnderRepair);

        // Act
        var result = bus.ToString();

        // Assert
        Assert.Contains("ABC123", result);
        Assert.Contains(bus.Id.ToString(), result);
        Assert.Contains("UnderRepair", result);
    }

    [Fact]
    public void RequirementValidation_BusIdentifiedByLicensePlate()
    {
        // Based on requirement: "a bus is identified by its unique licence plate"
        
        // Arrange
        var licensePlate = new LicensePlate("ABC123");

        // Act
        var bus = new Bus(licensePlate);

        // Assert
        Assert.Equal(licensePlate, bus.LicensePlate);
        Assert.NotEqual(Guid.Empty, bus.Id);
    }

    [Fact]
    public void RequirementValidation_BusInRepairCannotBeAssigned()
    {
        // Based on requirement: "If a bus is in the repair shop, the bus cannot be assigned to a route"
        
        // Arrange
        var operationalBus = new Bus(new LicensePlate("OPER001"));
        var repairBus = new Bus(new LicensePlate("REPAIR001"));
        var outOfServiceBus = new Bus(new LicensePlate("OUT001"));
        
        repairBus.SetUnderRepair();
        outOfServiceBus.SetOutOfService();

        // Act & Assert
        // Operational bus should be available for route assignment
        Assert.True(operationalBus.IsAvailableForService());
        
        // Bus under repair should NOT be available for route assignment
        Assert.False(repairBus.IsAvailableForService());
        
        // Out of service bus should NOT be available for route assignment
        Assert.False(outOfServiceBus.IsAvailableForService());
    }

    [Fact]
    public void RequirementValidation_LicensePlateLength()
    {
        // Based on requirement: "licence plate number may be up to 10 characters long, inclusive"
        
        // Arrange & Act
        var shortPlate = new Bus(new LicensePlate("A"));
        var maxLengthPlate = new Bus(new LicensePlate("1234567890")); // 10 characters

        // Assert
        Assert.Equal("A", shortPlate.LicensePlate.Value);
        Assert.Equal("1234567890", maxLengthPlate.LicensePlate.Value);

        // This should throw for license plates longer than 10 characters
        Assert.Throws<ArgumentException>(() => new LicensePlate("12345678901")); // 11 characters
    }
}
