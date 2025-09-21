using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.ValueObjects;

public class LocationTests
{
    [Fact]
    public void CreateLocation_WithValidData_ShouldSucceed()
    {
        // Arrange
        var name = "Community Center";
        var address = new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA");

        // Act
        var location = new Location(name, address);

        // Assert
        Assert.Equal("Community Center", location.Name);
        Assert.Equal(address, location.Address);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateLocation_WithInvalidName_ShouldThrowException(string name)
    {
        // Arrange
        var address = new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Location(name, address));
    }

    [Fact]
    public void CreateLocation_WithNullAddress_ShouldThrowException()
    {
        // Arrange
        var name = "Community Center";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Location(name, null!));
    }

    [Fact]
    public void Location_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var address = new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA");
        var location1 = new Location("Community Center", address);
        var location2 = new Location("Community Center", address);
        var location3 = new Location("Library", address);

        // Assert
        Assert.Equal(location1, location2);
        Assert.NotEqual(location1, location3);
    }
}
