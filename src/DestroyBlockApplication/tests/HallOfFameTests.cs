using DestroyBlockApplication.Domain.HallOfFame;
using DestroyBlockApplication.Domain.Shared.ValueObjects;
using DestroyBlockApplication.Domain.Tests.TestHelpers;
using Xunit;

namespace DestroyBlockApplication.Domain.Tests;

public class HallOfFameTests
{
    [Fact]
    public async Task HF001_ScoreAppearsInHallOfFame()
    {
        var (service, game, _, hofRepo) = await Helpers.CreateGamePlaySetup();
        var playerId = Guid.NewGuid();
        var session = await service.StartGameAsync(playerId, game.Id);
        session.AddScore(new Score(1200));
        await service.CompleteGameAsync(session.Id, playerId);

        var hof = await hofRepo.GetByGameIdAsync(game.Id);
        Assert.Equal(1200, hof!.Entries[0].Score.Value);
    }

    [Fact]
    public void HF002_HallOfFameOrderedByScoreDescending()
    {
        var hof = new HallOfFameAggregate(Guid.NewGuid());
        hof.AddEntry(new HighScoreEntry(hof.GameId, Guid.NewGuid(), Guid.NewGuid(), new Score(1200), DateTime.UtcNow));
        hof.AddEntry(new HighScoreEntry(hof.GameId, Guid.NewGuid(), Guid.NewGuid(), new Score(900), DateTime.UtcNow));
        hof.AddEntry(new HighScoreEntry(hof.GameId, Guid.NewGuid(), Guid.NewGuid(), new Score(1500), DateTime.UtcNow));

        Assert.Equal(1500, hof.Entries[0].Score.Value);
        Assert.Equal(1200, hof.Entries[1].Score.Value);
        Assert.Equal(900, hof.Entries[2].Score.Value);
    }

    [Fact]
    public void HF003_HallOfFameSeparatePerGame()
    {
        var hofA = new HallOfFameAggregate(Guid.NewGuid());
        var hofB = new HallOfFameAggregate(Guid.NewGuid());
        hofA.AddEntry(new HighScoreEntry(hofA.GameId, Guid.NewGuid(), Guid.NewGuid(), new Score(1200), DateTime.UtcNow));

        Assert.Single(hofA.Entries);
        Assert.Empty(hofB.Entries);
    }

    [Fact]
    public async Task HF004_MultiplePlaysSameGameBothScoresInHallOfFame()
    {
        var (service, game, _, hofRepo) = await Helpers.CreateGamePlaySetup();
        var playerId = Guid.NewGuid();

        var s1 = await service.StartGameAsync(playerId, game.Id);
        s1.AddScore(new Score(800));
        await service.CompleteGameAsync(s1.Id, playerId);

        var s2 = await service.StartGameAsync(playerId, game.Id);
        s2.AddScore(new Score(1200));
        await service.CompleteGameAsync(s2.Id, playerId);

        Assert.Equal(2, (await hofRepo.GetByGameIdAsync(game.Id))!.Entries.Count);
    }
}
