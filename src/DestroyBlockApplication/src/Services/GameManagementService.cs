using DestroyBlockApplication.Domain.Game;
using DestroyBlockApplication.Domain.Game.Repositories;
using DestroyBlockApplication.Domain.HallOfFame;
using DestroyBlockApplication.Domain.HallOfFame.Repositories;
using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;
using DestroyBlockApplication.Domain.User.Repositories;

namespace DestroyBlockApplication.Domain.Services;

public class GameManagementService
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHallOfFameRepository _hallOfFameRepository;

    public GameManagementService(
        IGameRepository gameRepository,
        IUserRepository userRepository,
        IHallOfFameRepository hallOfFameRepository)
    {
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _hallOfFameRepository = hallOfFameRepository ?? throw new ArgumentNullException(nameof(hallOfFameRepository));
    }

    public async Task<GameAggregate> CreateGameAsync(
        GameName name,
        Guid adminId,
        Speed minimumSpeed,
        double speedIncreaseFactor,
        PaddleLength maximumPaddleLength,
        PaddleLength minimumPaddleLength,
        int blocksPerLevel)
    {
        // Verify admin exists and has admin privileges
        var admin = await _userRepository.GetByIdAsync(adminId) ?? throw new DomainException($"User with ID {adminId} not found.");
        if (!admin.IsAdmin)
        {
            throw new DomainException($"User {admin.Username} is not an admin.");
        }

        // Check if game name already exists
        var existingGame = await _gameRepository.GetByNameAsync(name.Value);
        if (existingGame != null)
        {
            throw new DomainException($"Game with name '{name}' already exists.");
        }

        var game = new GameAggregate(name, adminId, minimumSpeed, speedIncreaseFactor, 
            maximumPaddleLength, minimumPaddleLength, blocksPerLevel);

        await _gameRepository.AddAsync(game);

        var hallOfFame = new HallOfFameAggregate(game.Id);
        await _hallOfFameRepository.AddAsync(hallOfFame);

        return game;
    }

    public async Task PublishGameAsync(Guid gameId, Guid adminId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId) ?? throw new DomainException($"Game with ID {gameId} not found.");
        if (game.AdminId != adminId)
            throw new DomainException("Only the game admin can publish the game.");

        game.Publish();
        await _gameRepository.UpdateAsync(game);
    }
}
