using DestroyBlockApplication.Domain.Services;
using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;
using DestroyBlockApplication.Domain.Tests.TestHelpers;
using DestroyBlockApplication.Domain.User;
using Xunit;

namespace DestroyBlockApplication.Domain.Tests;

public class GameAdministrationTests
{
    [Fact]
    public async Task GA001_CreateGameByAdmin_Succeeds()
    {
        var (_, userRepo, gameRepo, hofRepo) = Helpers.CreateGameManagementService();
        var admin = new UserAggregate(new Username("admin1"), new Password("pass"), isAdmin: true);
        await userRepo.AddAsync(admin);
        var svc = new GameManagementService(gameRepo, userRepo, hofRepo);

        var game = await svc.CreateGameAsync(new GameName("BlockBuster"), admin.Id,
            new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10);

        Assert.NotNull(game);
        Assert.Equal("BlockBuster", game.Name.Value);
    }

    [Fact]
    public async Task GA002_CreateGameByPlayer_ThrowsDomainException()
    {
        var (_, userRepo, gameRepo, hofRepo) = Helpers.CreateGameManagementService();
        var player = new UserAggregate(new Username("dave"), new Password("pass"), isAdmin: false);
        await userRepo.AddAsync(player);
        var svc = new GameManagementService(gameRepo, userRepo, hofRepo);

        await Assert.ThrowsAsync<DomainException>(() =>
            svc.CreateGameAsync(new GameName("TestGame"), player.Id,
                new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10));
    }

    [Fact]
    public async Task GA003_CreateGameDuplicateName_ThrowsDomainException()
    {
        var (_, userRepo, gameRepo, hofRepo) = Helpers.CreateGameManagementService();
        var admin = new UserAggregate(new Username("admin1"), new Password("pass"), isAdmin: true);
        await userRepo.AddAsync(admin);
        var svc = new GameManagementService(gameRepo, userRepo, hofRepo);

        await svc.CreateGameAsync(new GameName("BlockBuster"), admin.Id,
            new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10);

        await Assert.ThrowsAsync<DomainException>(() =>
            svc.CreateGameAsync(new GameName("BlockBuster"), admin.Id,
                new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10));
    }

    [Fact]
    public void GA004_CreateGameEmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new GameName(""));
    }

    [Fact]
    public async Task GA005_EachGameHasSeparateHallOfFame()
    {
        var (_, userRepo, gameRepo, hofRepo) = Helpers.CreateGameManagementService();
        var admin = new UserAggregate(new Username("admin1"), new Password("pass"), isAdmin: true);
        await userRepo.AddAsync(admin);
        var svc = new GameManagementService(gameRepo, userRepo, hofRepo);

        var gameA = await svc.CreateGameAsync(new GameName("BlockBuster"), admin.Id,
            new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10);
        var gameB = await svc.CreateGameAsync(new GameName("PaddleMaster"), admin.Id,
            new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10);

        var hofA = await hofRepo.GetByGameIdAsync(gameA.Id);
        var hofB = await hofRepo.GetByGameIdAsync(gameB.Id);

        Assert.NotNull(hofA);
        Assert.NotNull(hofB);
        Assert.NotEqual(hofA.GameId, hofB.GameId);
    }

    [Fact]
    public async Task GA006_GameOnlyOneAdmin()
    {
        var (_, userRepo, gameRepo, hofRepo) = Helpers.CreateGameManagementService();
        var admin1 = new UserAggregate(new Username("alice"), new Password("pass"), isAdmin: true);
        var admin2 = new UserAggregate(new Username("bob"), new Password("pass"), isAdmin: true);
        await userRepo.AddAsync(admin1);
        await userRepo.AddAsync(admin2);
        var svc = new GameManagementService(gameRepo, userRepo, hofRepo);

        var game = await svc.CreateGameAsync(new GameName("BlockBuster"), admin1.Id,
            new Speed(2), 0.1, new PaddleLength(200), new PaddleLength(50), 10);

        Assert.Equal(admin1.Id, game.AdminId);
        Assert.NotEqual(admin2.Id, game.AdminId);
    }

    [Fact]
    public async Task GA007_UnpublishedGameCannotBeStarted()
    {
        var (gameRepo, sessionRepo, hofRepo) = (new FakeGameRepository(), new FakeGameSessionRepository(), new FakeHallOfFameRepository());
        var game = Helpers.CreateValidGame(Guid.NewGuid());
        await gameRepo.AddAsync(game);
        var svc = new GamePlayService(gameRepo, sessionRepo, hofRepo);

        await Assert.ThrowsAsync<DomainException>(() => svc.StartGameAsync(Guid.NewGuid(), game.Id));
    }

    [Fact]
    public async Task GA008_PublishedGameCanBeStarted()
    {
        var (service, game, _, _) = await Helpers.CreateGamePlaySetup();
        var playerId = Guid.NewGuid();

        var session = await service.StartGameAsync(playerId, game.Id);

        Assert.NotNull(session);
        Assert.Equal(playerId, session.PlayerId);
    }
}
