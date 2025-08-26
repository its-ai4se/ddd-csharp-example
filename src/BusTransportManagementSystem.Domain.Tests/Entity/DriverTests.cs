using BusTransportManagementSystem.Domain.Entity;
using BusTransportManagementSystem.Domain.ValueObject;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests.Entity;

public class DriverTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateDriver()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = new DriverName("John Doe");
        var status = SickLeaveStatus.Active;

        // Act
        var driver = new Driver(id, name, status);

        // Assert
        Assert.Equal(id, driver.Id);
        Assert.Equal(name, driver.Name);
        Assert.Equal(status, driver.SickLeaveStatus);
    }

    [Fact]
    public void Constructor_WithoutStatus_ShouldDefaultToActive()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = new DriverName("John Doe");

        // Act
        var driver = new Driver(id, name);

        // Assert
        Assert.Equal(SickLeaveStatus.Active, driver.SickLeaveStatus);
        Assert.True(driver.IsAvailable());
    }

    [Fact]
    public void Constructor_WithNameOnly_ShouldGenerateIdAndDefaultToActive()
    {
        // Arrange
        var name = new DriverName("John Doe");

        // Act
        var driver = new Driver(name);

        // Assert
        Assert.NotEqual(Guid.Empty, driver.Id);
        Assert.Equal(name, driver.Name);
        Assert.Equal(SickLeaveStatus.Active, driver.SickLeaveStatus);
        Assert.True(driver.IsAvailable());
    }

    [Fact]
    public void Constructor_WithEmptyId_ShouldThrowArgumentException()
    {
        // Arrange
        var emptyId = Guid.Empty;
        var name = new DriverName("John Doe");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Driver(emptyId, name));
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        DriverName nullName = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Driver(id, nullName));
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var driver = new Driver(new DriverName("John Doe"));
        var newName = new DriverName("Jane Smith");

        // Act
        driver.UpdateName(newName);

        // Assert
        Assert.Equal(newName, driver.Name);
    }

    [Fact]
    public void UpdateName_WithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var driver = new Driver(new DriverName("John Doe"));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => driver.UpdateName(null!));
    }

    [Fact]
    public void SetSickLeave_ShouldChangeStatusToOnSickLeave()
    {
        // Arrange
        var driver = new Driver(new DriverName("John Doe"));
        Assert.True(driver.IsAvailable()); // Initially available

        // Act
        driver.SetSickLeave();

        // Assert
        Assert.Equal(SickLeaveStatus.OnSickLeave, driver.SickLeaveStatus);
        Assert.False(driver.IsAvailable());
        Assert.True(driver.IsOnSickLeave());
    }

    [Fact]
    public void ClearSickLeave_ShouldChangeStatusToActive()
    {
        // Arrange
        var driver = new Driver(new DriverName("John Doe"), SickLeaveStatus.OnSickLeave);
        Assert.False(driver.IsAvailable()); // Initially not available

        // Act
        driver.ClearSickLeave();

        // Assert
        Assert.Equal(SickLeaveStatus.Active, driver.SickLeaveStatus);
        Assert.True(driver.IsAvailable());
        Assert.False(driver.IsOnSickLeave());
    }

    [Fact]
    public void IsAvailable_WithActiveStatus_ShouldReturnTrue()
    {
        // Arrange
        var driver = new Driver(new DriverName("John Doe"), SickLeaveStatus.Active);

        // Act & Assert
        Assert.True(driver.IsAvailable());
        Assert.False(driver.IsOnSickLeave());
    }

    [Fact]
    public void IsOnSickLeave_WithSickLeaveStatus_ShouldReturnTrue()
    {
        // Arrange
        var driver = new Driver(new DriverName("John Doe"), SickLeaveStatus.OnSickLeave);

        // Act & Assert
        Assert.True(driver.IsOnSickLeave());
        Assert.False(driver.IsAvailable());
    }

    [Fact]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var driver1 = new Driver(id, new DriverName("John Doe"));
        var driver2 = new Driver(id, new DriverName("Jane Smith")); // Different name, same ID

        // Act & Assert
        Assert.True(driver1.Equals(driver2));
        Assert.True(driver1 == driver2);
        Assert.False(driver1 != driver2);
    }

    [Fact]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // Arrange
        var driver1 = new Driver(new DriverName("John Doe"));
        var driver2 = new Driver(new DriverName("John Doe")); // Same name, different ID

        // Act & Assert
        Assert.False(driver1.Equals(driver2));
        Assert.False(driver1 == driver2);
        Assert.True(driver1 != driver2);
    }

    [Fact]
    public void GetHashCode_WithSameId_ShouldBeSame()
    {
        // Arrange
        var id = Guid.NewGuid();
        var driver1 = new Driver(id, new DriverName("John Doe"));
        var driver2 = new Driver(id, new DriverName("Jane Smith"));

        // Act & Assert
        Assert.Equal(driver1.GetHashCode(), driver2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldIncludeDriverInformation()
    {
        // Arrange
        var driver = new Driver(new DriverName("John Doe"), SickLeaveStatus.OnSickLeave);

        // Act
        var result = driver.ToString();

        // Assert
        Assert.Contains("John Doe", result);
        Assert.Contains(driver.Id.ToString(), result);
        Assert.Contains("OnSickLeave", result);
    }

    [Fact]
    public void RequirementValidation_DriverGetsUniqueId()
    {
        // Based on requirement: "automatically assigns a unique ID to each driver"
        
        // Arrange & Act
        var driver1 = new Driver(new DriverName("John Doe"));
        var driver2 = new Driver(new DriverName("Jane Smith"));

        // Assert
        Assert.NotEqual(Guid.Empty, driver1.Id);
        Assert.NotEqual(Guid.Empty, driver2.Id);
        Assert.NotEqual(driver1.Id, driver2.Id);
    }

    [Fact]
    public void RequirementValidation_SickDriverCannotBeScheduled()
    {
        // Based on requirement: "If a driver is currently sick, the driver cannot be scheduled"
        
        // Arrange
        var activeDriver = new Driver(new DriverName("Active Driver"));
        var sickDriver = new Driver(new DriverName("Sick Driver"));
        sickDriver.SetSickLeave();

        // Act & Assert
        // Active driver should be available for scheduling
        Assert.True(activeDriver.IsAvailable());
        Assert.False(activeDriver.IsOnSickLeave());
        
        // Sick driver should NOT be available for scheduling
        Assert.False(sickDriver.IsAvailable());
        Assert.True(sickDriver.IsOnSickLeave());
    }
}
