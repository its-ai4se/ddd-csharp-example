using DestroyBlockApplication.Domain.User;
using DestroyBlockApplication.Domain.Shared.ValueObjects;
using Xunit;

namespace DestroyBlockApplication.Domain.Tests.Aggregates;

public class UserAggregateTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var username = new Username("player123");
        var password = new Password("secret123");

        // Act
        var user = new UserAggregate(username, password);

        // Assert
        Assert.Equal(username, user.Username);
        Assert.Equal(password, user.Password);
        Assert.True(user.IsPlayer);
        Assert.False(user.IsAdmin);
        Assert.Empty(user.GameRoles);
    }

    [Fact]
    public void Constructor_NullUsername_ShouldThrowArgumentNullException()
    {
        // Arrange
        var password = new Password("secret123");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserAggregate(null!, password));
    }

    [Fact]
    public void Constructor_NullPassword_ShouldThrowArgumentNullException()
    {
        // Arrange
        var username = new Username("player123");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserAggregate(username, null!));
    }

    [Fact]
    public void PromoteToAdmin_ShouldSetIsAdminToTrue()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        user.PromoteToAdmin();

        // Assert
        Assert.True(user.IsAdmin);
    }

    [Fact]
    public void DemoteFromAdmin_ShouldSetIsAdminToFalse()
    {
        // Arrange
        var user = CreateValidUser();
        user.PromoteToAdmin();

        // Act
        user.DemoteFromAdmin();

        // Assert
        Assert.False(user.IsAdmin);
    }

    [Fact]
    public void AddGameRole_ValidRole_ShouldAddRole()
    {
        // Arrange
        var user = CreateValidUser();
        var gameId = Guid.NewGuid();
        var role = new GameRole(user.Id, gameId, RoleType.Player);

        // Act
        user.AddGameRole(role);

        // Assert
        Assert.Single(user.GameRoles);
        Assert.Equal(role, user.GameRoles.First());
    }

    [Fact]
    public void AddGameRole_NullRole_ShouldThrowArgumentNullException()
    {
        // Arrange
        var user = CreateValidUser();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => user.AddGameRole(null!));
    }

    [Fact]
    public void AddGameRole_RoleForDifferentUser_ShouldThrowArgumentException()
    {
        // Arrange
        var user = CreateValidUser();
        var gameId = Guid.NewGuid();
        var role = new GameRole(Guid.NewGuid(), gameId, RoleType.Player);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.AddGameRole(role));
    }

    [Fact]
    public void AddGameRole_DuplicateGameRole_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = CreateValidUser();
        var gameId = Guid.NewGuid();
        var role1 = new GameRole(user.Id, gameId, RoleType.Player);
        var role2 = new GameRole(user.Id, gameId, RoleType.Admin);

        // Act
        user.AddGameRole(role1);

        // Assert
        Assert.Throws<InvalidOperationException>(() => user.AddGameRole(role2));
    }

    [Fact]
    public void HasRoleForGame_ExistingRole_ShouldReturnTrue()
    {
        // Arrange
        var user = CreateValidUser();
        var gameId = Guid.NewGuid();
        var role = new GameRole(user.Id, gameId, RoleType.Player);
        user.AddGameRole(role);

        // Act
        var hasRole = user.HasRoleForGame(gameId, RoleType.Player);

        // Assert
        Assert.True(hasRole);
    }

    [Fact]
    public void HasRoleForGame_NonExistingRole_ShouldReturnFalse()
    {
        // Arrange
        var user = CreateValidUser();
        var gameId = Guid.NewGuid();

        // Act
        var hasRole = user.HasRoleForGame(gameId, RoleType.Player);

        // Assert
        Assert.False(hasRole);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = new Password("secret123");
        var user = new UserAggregate(new Username("player123"), password);

        // Act
        var isValid = user.VerifyPassword(password);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = new Password("secret123");
        var wrongPassword = new Password("wrong123");
        var user = new UserAggregate(new Username("player123"), password);

        // Act
        var isValid = user.VerifyPassword(wrongPassword);

        // Assert
        Assert.False(isValid);
    }

    private static UserAggregate CreateValidUser()
    {
        var username = new Username("player123");
        var password = new Password("secret123");
        return new UserAggregate(username, password);
    }
}
