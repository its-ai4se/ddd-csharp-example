using DestroyBlockApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace DestroyBlockApplication.Domain.Tests.ValueObjects;

public class UsernameTests
{
    [Fact]
    public void Constructor_ValidUsername_ShouldCreateInstance()
    {
        // Arrange
        var usernameValue = "player123";

        // Act
        var username = new Username(usernameValue);

        // Assert
        Assert.Equal(usernameValue, username.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_EmptyOrWhitespace_ShouldThrowArgumentException(string value)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Username(value));
    }

    [Theory]
    [InlineData("ab")] // Too short
    public void Constructor_InvalidLength_ShouldThrowArgumentException(string value)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Username(value));
    }

    [Fact]
    public void Constructor_TooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var longUsername = "a".PadRight(51, 'a'); // Too long

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Username(longUsername));
    }

    [Theory]
    [InlineData("user@name")] // Contains @
    [InlineData("user name")] // Contains space
    [InlineData("user.name")] // Contains dot
    public void Constructor_InvalidCharacters_ShouldThrowArgumentException(string value)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Username(value));
    }

    [Theory]
    [InlineData("user123")]
    [InlineData("user_name")]
    [InlineData("user-name")]
    [InlineData("User123")]
    public void Constructor_ValidCharacters_ShouldCreateInstance(string value)
    {
        // Act
        var username = new Username(value);

        // Assert
        Assert.Equal(value, username.Value);
    }

    [Fact]
    public void Equals_SameValues_ShouldReturnTrue()
    {
        // Arrange
        var username1 = new Username("player123");
        var username2 = new Username("player123");

        // Act & Assert
        Assert.Equal(username1, username2);
        Assert.True(username1 == username2);
    }

    [Fact]
    public void Equals_DifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var username1 = new Username("player123");
        var username2 = new Username("player456");

        // Act & Assert
        Assert.NotEqual(username1, username2);
        Assert.True(username1 != username2);
    }
}
