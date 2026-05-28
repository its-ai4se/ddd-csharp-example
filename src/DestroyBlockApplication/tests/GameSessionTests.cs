using DestroyBlockApplication.Domain.HallOfFame;
using DestroyBlockApplication.Domain.Services;
using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Tests.TestHelpers;
using Xunit;

namespace DestroyBlockApplication.Domain.Tests;

public class GameSessionTests
{
    [Fact]
    public async Task GS001_PlayerCanPlayDifferentGames()
    {
        var (gameRepo, sessionRepo, hofRepo) = (new FakeGameRepository(), new FakeGameSessionRepository(), new FakeHallOfFameRepository());
        var adminId = Guid.NewGuid();
        var gameA = Helpers.CreateValidGame(adminId, "BlockBuster");
        var gameB = Helpers.CreateValidGame(adminId, "PaddleMaster");
        gameA.Publish(); gameB.Publish();
        await gameRepo.AddAsync(gameA); await gameRepo.AddAsync(gameB);
        await hofRepo.AddAsync(new HallOfFameAggregate(gameA.Id));
        await hofRepo.AddAsync(new HallOfFameAggregate(gameB.Id));

        var service = new GamePlayService(gameRepo, sessionRepo, hofRepo);
        var playerId = Guid.NewGuid();

        var sessionA = await service.StartGameAsync(playerId, gameA.Id);
        await service.CompleteGameAsync(sessionA.Id, playerId);

        var sessionB = await service.StartGameAsync(playerId, gameB.Id);
        Assert.Equal(gameB.Id, sessionB.GameId);
    }

    [Fact]
    public async Task GS002_PlayerCanPlaySameGameMultipleTimes()
    {
        var (service, game, _, _) = await Helpers.CreateGamePlaySetup();
        var playerId = Guid.NewGuid();

        var s1 = await service.StartGameAsync(playerId, game.Id);
        await service.CompleteGameAsync(s1.Id, playerId);

        var s2 = await service.StartGameAsync(playerId, game.Id);
        Assert.NotEqual(s1.Id, s2.Id);
    }

    [Fact]
    public async Task GS003_PlayerCannotPlayTwoGamesSimultaneously()
    {
        var (gameRepo, sessionRepo, hofRepo) = (new FakeGameRepository(), new FakeGameSessionRepository(), new FakeHallOfFameRepository());
        var adminId = Guid.NewGuid();
        var gameA = Helpers.CreateValidGame(adminId, "BlockBuster");
        var gameB = Helpers.CreateValidGame(adminId, "PaddleMaster");
        gameA.Publish(); gameB.Publish();
        await gameRepo.AddAsync(gameA); await gameRepo.AddAsync(gameB);
        await hofRepo.AddAsync(new HallOfFameAggregate(gameA.Id));
        await hofRepo.AddAsync(new HallOfFameAggregate(gameB.Id));

        var service = new GamePlayService(gameRepo, sessionRepo, hofRepo);
        var playerId = Guid.NewGuid();

        await service.StartGameAsync(playerId, gameA.Id);

        await Assert.ThrowsAsync<DomainException>(() => service.StartGameAsync(playerId, gameB.Id));
    }

    [Fact]
    public async Task GS004_PlayerMustFinishActiveGameFirst()
    {
        var (service, game, _, _) = await Helpers.CreateGamePlaySetup();
        var playerId = Guid.NewGuid();

        await service.StartGameAsync(playerId, game.Id);

        await Assert.ThrowsAsync<DomainException>(() => service.StartGameAsync(playerId, game.Id));
    }
}
