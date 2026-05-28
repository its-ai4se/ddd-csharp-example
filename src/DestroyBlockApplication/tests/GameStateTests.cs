using DestroyBlockApplication.Domain.Shared.ValueObjects;
using DestroyBlockApplication.Domain.Tests.TestHelpers;
using Xunit;

namespace DestroyBlockApplication.Domain.Tests;

public class GameStateTests
{
    [Fact]
    public async Task GT001_CompleteLevelGameIsSaved()
    {
        var (service, game, sessionRepo, _) = await Helpers.CreateGamePlaySetup();
        var playerId = Guid.NewGuid();
        var session = await service.StartGameAsync(playerId, game.Id);

        await service.CompleteLevelAsync(session.Id, playerId);

        Assert.NotNull((await sessionRepo.GetByIdAsync(session.Id))!.LastSavedAt);
    }

    [Fact]
    public async Task GT002_PauseGameGameIsSaved()
    {
        var (service, game, sessionRepo, _) = await Helpers.CreateGamePlaySetup();
        var playerId = Guid.NewGuid();
        var session = await service.StartGameAsync(playerId, game.Id);

        await service.PauseGameAsync(session.Id, playerId);

        var updated = (await sessionRepo.GetByIdAsync(session.Id))!;
        Assert.NotNull(updated.LastSavedAt);
        Assert.True(updated.IsPaused);
    }

    [Fact]
    public async Task GT003_ResumeGameGameResumes()
    {
        var (service, game, sessionRepo, _) = await Helpers.CreateGamePlaySetup();
        var playerId = Guid.NewGuid();
        var session = await service.StartGameAsync(playerId, game.Id);

        await service.PauseGameAsync(session.Id, playerId);
        await service.ResumeGameAsync(session.Id, playerId);

        Assert.True((await sessionRepo.GetByIdAsync(session.Id))!.IsActive);
    }

    [Fact]
    public async Task GT004_ResumeGameStatePreserved()
    {
        var (service, game, sessionRepo, _) = await Helpers.CreateGamePlaySetup();
        var playerId = Guid.NewGuid();
        var session = await service.StartGameAsync(playerId, game.Id);
        session.AddScore(new Score(300));
        session.LoseLife();

        await service.PauseGameAsync(session.Id, playerId);
        await service.ResumeGameAsync(session.Id, playerId);

        var updated = (await sessionRepo.GetByIdAsync(session.Id))!;
        Assert.Equal(1, updated.CurrentLevel.Value);
        Assert.Equal(300, updated.TotalScore.Value);
        Assert.Equal(2, updated.Lives.Value);
    }
}
