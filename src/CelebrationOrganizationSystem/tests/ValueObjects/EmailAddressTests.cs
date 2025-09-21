using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.ValueObjects;

public class EmailAddressTests
{
    [Fact]
    public void CreateEmailAddress_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var emailAddress = new EmailAddress(email);

        // Assert
        Assert.Equal("test@example.com", emailAddress.Value);
    }

    [Fact]
    public void CreateEmailAddress_WithValidEmailAndWhitespace_ShouldTrimAndLowercase()
    {
        // Arrange
        var email = "  test@example.com  ";

        // Act
        var emailAddress = new EmailAddress(email);

        // Assert
        Assert.Equal("test@example.com", emailAddress.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateEmailAddress_WithInvalidEmail_ShouldThrowException(string email)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new EmailAddress(email));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    public void CreateEmailAddress_WithMalformedEmail_ShouldThrowException(string email)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new EmailAddress(email));
    }

    [Fact]
    public void EmailAddress_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var email1 = new EmailAddress("test@example.com");
        var email2 = new EmailAddress("TEST@EXAMPLE.COM");
        var email3 = new EmailAddress("other@example.com");

        // Assert
        Assert.Equal(email1, email2);
        Assert.NotEqual(email1, email3);
        Assert.True(email1 == email2);
        Assert.False(email1 == email3);
    }
}
