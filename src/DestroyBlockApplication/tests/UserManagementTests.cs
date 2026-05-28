using DestroyBlockApplication.Domain.Services;
using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;
using DestroyBlockApplication.Domain.Tests.TestHelpers;
using DestroyBlockApplication.Domain.User;
using Xunit;

namespace DestroyBlockApplication.Domain.Tests;

public class UserManagementTests
{
    [Fact]
    public async Task UM001_RegisterUniqueUsername_Succeeds()
    {
        var (svc, _) = Helpers.CreateUserService();
        var user = await svc.RegisterUserAsync(new Username("alice"), new Password("pass123"));
        Assert.NotNull(user);
        Assert.Equal("alice", user.Username.Value);
    }

    [Fact]
    public async Task UM002_RegisterDuplicateUsername_ThrowsDomainException()
    {
        var (svc, _) = Helpers.CreateUserService();
        await svc.RegisterUserAsync(new Username("alice"), new Password("pass123"));
        await Assert.ThrowsAsync<DomainException>(() =>
            svc.RegisterUserAsync(new Username("alice"), new Password("pass123")));
    }

    [Fact]
    public void UM003_RegisterEmptyUsername_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Username(""));
    }

    [Fact]
    public async Task UM004_RegisterNewUser_HasPlayerRole()
    {
        var (svc, _) = Helpers.CreateUserService();
        var user = await svc.RegisterUserAsync(new Username("bob"), new Password("pass123"));
        Assert.False(user.IsAdmin);
    }

    [Fact]
    public void UM005_UserAssignAdmin_HasBothRoles()
    {
        var user = new UserAggregate(new Username("bob"), new Password("pass123"), isAdmin: true);
        Assert.True(user.IsAdmin);
    }

    [Fact]
    public async Task UM006_LoginSamePasswordForBothModes_Succeeds()
    {
        var (_, repo) = Helpers.CreateUserService();
        await repo.AddAsync(new UserAggregate(new Username("carol"), new Password("secret"), isAdmin: true));
        var svc = new UserManagementService(repo);

        var playerSession = await svc.LoginAsync(new Username("carol"), new Password("secret"), LoginMode.Player);
        var adminSession = await svc.LoginAsync(new Username("carol"), new Password("secret"), LoginMode.Admin);

        Assert.Equal(LoginMode.Player, playerSession.Mode);
        Assert.Equal(LoginMode.Admin, adminSession.Mode);
    }

    [Fact]
    public async Task UM007_LoginPlayerMode_Succeeds()
    {
        var (svc, _) = Helpers.CreateUserService();
        await svc.RegisterUserAsync(new Username("carol"), new Password("secret"));
        var session = await svc.LoginAsync(new Username("carol"), new Password("secret"), LoginMode.Player);
        Assert.Equal(LoginMode.Player, session.Mode);
    }

    [Fact]
    public async Task UM008_LoginAdminMode_Succeeds()
    {
        var (_, repo) = Helpers.CreateUserService();
        await repo.AddAsync(new UserAggregate(new Username("carol"), new Password("secret"), isAdmin: true));
        var svc = new UserManagementService(repo);
        var session = await svc.LoginAsync(new Username("carol"), new Password("secret"), LoginMode.Admin);
        Assert.Equal(LoginMode.Admin, session.Mode);
    }

    [Fact]
    public async Task UM009_LoginWrongPassword_ThrowsDomainException()
    {
        var (svc, _) = Helpers.CreateUserService();
        await svc.RegisterUserAsync(new Username("carol"), new Password("secret"));
        await Assert.ThrowsAsync<DomainException>(() =>
            svc.LoginAsync(new Username("carol"), new Password("wrong"), LoginMode.Player));
    }

    [Fact]
    public async Task UM010_LoginUnknownUsername_ThrowsDomainException()
    {
        var (svc, _) = Helpers.CreateUserService();
        await Assert.ThrowsAsync<DomainException>(() =>
            svc.LoginAsync(new Username("unknown"), new Password("pass"), LoginMode.Player));
    }

    [Fact]
    public async Task UM011_LoginNonAdminInAdminMode_ThrowsDomainException()
    {
        var (svc, _) = Helpers.CreateUserService();
        await svc.RegisterUserAsync(new Username("dave"), new Password("pass123"));
        await Assert.ThrowsAsync<DomainException>(() =>
            svc.LoginAsync(new Username("dave"), new Password("pass123"), LoginMode.Admin));
    }

    [Fact]
    public async Task UM012_UserDifferentRolesInDifferentGames_Allowed()
    {
        var (_, userRepo, gameRepo, hofRepo) = Helpers.CreateGameManagementService();
        var gameSvc = new GameManagementService(gameRepo, userRepo, hofRepo);

        var eve = new UserAggregate(new Username("eve"), new Password("pass"), isAdmin: true);
        await userRepo.AddAsync(eve);
        var gameA = await gameSvc.CreateGameAsync(new GameName("GameA"), eve.Id,
            new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10);

        var otherAdmin = new UserAggregate(new Username("admin2"), new Password("pass"), isAdmin: true);
        await userRepo.AddAsync(otherAdmin);
        var gameB = await gameSvc.CreateGameAsync(new GameName("GameB"), otherAdmin.Id,
            new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10);

        Assert.NotEqual(gameA.AdminId, gameB.AdminId);
    }

    [Fact]
    public async Task UM013_UserPlayerAndAdminSameGame_NotAllowed()
    {
        var (service, game, _, _) = await Helpers.CreateGamePlaySetup();
        await Assert.ThrowsAsync<DomainException>(() =>
            service.StartGameAsync(game.AdminId, game.Id));
    }
}
