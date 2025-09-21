using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.ValueObjects;

public class AddressTests
{
    [Fact]
    public void CreateAddress_WithValidData_ShouldSucceed()
    {
        // Arrange
        var street = "123 Main St";
        var city = "Anytown";
        var state = "CA";
        var postalCode = "12345";
        var country = "USA";

        // Act
        var address = new Address(street, city, state, postalCode, country);

        // Assert
        Assert.Equal("123 Main St", address.Street);
        Assert.Equal("Anytown", address.City);
        Assert.Equal("CA", address.State);
        Assert.Equal("12345", address.PostalCode);
        Assert.Equal("USA", address.Country);
        Assert.Equal("123 Main St, Anytown, CA 12345, USA", address.FullAddress);
    }

    [Theory]
    [InlineData("", "City", "State", "12345", "Country")]
    [InlineData("Street", "", "State", "12345", "Country")]
    [InlineData("Street", "City", "", "12345", "Country")]
    [InlineData("Street", "City", "State", "", "Country")]
    [InlineData("Street", "City", "State", "12345", "")]
    public void CreateAddress_WithEmptyFields_ShouldThrowException(string street, string city, string state, string postalCode, string country)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Address(street, city, state, postalCode, country));
    }

    [Fact]
    public void Address_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var address1 = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
        var address2 = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
        var address3 = new Address("456 Oak Ave", "Anytown", "CA", "12345", "USA");

        // Assert
        Assert.Equal(address1, address2);
        Assert.NotEqual(address1, address3);
    }
}
