using DestroyBlockApplication.Domain.Game;
using DestroyBlockApplication.Domain.HallOfFame;
using DestroyBlockApplication.Domain.Services;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.Tests.TestHelpers;

static class Helpers
{
    public static (UserManagementService, FakeUserRepository) CreateUserService()
    {
        var repo = new FakeUserRepository();
        return (new UserManagementService(repo), repo);
    }

    public static (GameManagementService, FakeUserRepository, FakeGameRepository, FakeHallOfFameRepository) CreateGameManagementService()
    {
        var users = new FakeUserRepository();
        var games = new FakeGameRepository();
        var hofs = new FakeHallOfFameRepository();
        return (new GameManagementService(games, users, hofs), users, games, hofs);
    }

    public static GameAggregate CreateValidGame(Guid adminId, string name = "TestGame", int numLevels = 10)
    {
        var game = new GameAggregate(
            new GameName(name), adminId,
            new Speed(2), 0.1,
            new PaddleLength(200), new PaddleLength(50), 10);

        var blockType = new BlockType(new Color("red"), new Score(100));
        game.AddBlockType(blockType);

        for (int i = 1; i <= numLevels; i++)
        {
            var level = new Level(new LevelNumber(i));
            for (int b = 1; b <= game.BlocksPerLevel; b++)
                level.AddBlockPlacement(new BlockPlacement(new GridPosition(b, 1), blockType.Id));
            game.AddLevel(level);
        }

        return game;
    }

    public static async Task<(GamePlayService, GameAggregate, FakeGameSessionRepository, FakeHallOfFameRepository)>
        CreateGamePlaySetup(int numLevels = 10)
    {
        var gameRepo = new FakeGameRepository();
        var sessionRepo = new FakeGameSessionRepository();
        var hofRepo = new FakeHallOfFameRepository();

        var game = CreateValidGame(Guid.NewGuid(), numLevels: numLevels);
        game.Publish();
        await gameRepo.AddAsync(game);
        await hofRepo.AddAsync(new HallOfFameAggregate(game.Id));

        var service = new GamePlayService(gameRepo, sessionRepo, hofRepo);
        return (service, game, sessionRepo, hofRepo);
    }
}
