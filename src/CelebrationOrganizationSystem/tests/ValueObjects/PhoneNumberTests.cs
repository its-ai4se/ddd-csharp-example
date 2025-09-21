using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Fact]
    public void CreatePhoneNumber_WithValidNumber_ShouldSucceed()
    {
        // Arrange
        var phoneNumber = "5551234567";

        // Act
        var phone = new PhoneNumber(phoneNumber);

        // Assert
        Assert.Equal("5551234567", phone.Value);
    }

    [Fact]
    public void CreatePhoneNumber_WithFormattedNumber_ShouldClean()
    {
        // Arrange
        var phoneNumber = "(555) 123-4567";

        // Act
        var phone = new PhoneNumber(phoneNumber);

        // Assert
        Assert.Equal("5551234567", phone.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreatePhoneNumber_WithEmptyNumber_ShouldThrowException(string phoneNumber)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PhoneNumber(phoneNumber));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789")]
    [InlineData("abc1234567")]
    public void CreatePhoneNumber_WithInvalidNumber_ShouldThrowException(string phoneNumber)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PhoneNumber(phoneNumber));
    }

    [Fact]
    public void PhoneNumber_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var phone1 = new PhoneNumber("5551234567");
        var phone2 = new PhoneNumber("(555) 123-4567");
        var phone3 = new PhoneNumber("5559876543");

        // Assert
        Assert.Equal(phone1, phone2);
        Assert.NotEqual(phone1, phone3);
    }
}
