using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void CreatePassword_WithValidPassword_ShouldSucceed()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var passwordObj = new Password(password);

        // Assert
        Assert.Equal("SecurePassword123!", passwordObj.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreatePassword_WithEmptyPassword_ShouldThrowException(string password)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Password(password));
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("short")]
    public void CreatePassword_WithShortPassword_ShouldThrowException(string password)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Password(password));
    }

    [Fact]
    public void Password_ToString_ShouldMaskValue()
    {
        // Arrange
        var password = new Password("SecurePassword123!");

        // Act
        var result = password.ToString();

        // Assert
        Assert.Equal("***", result);
    }
}
