using DestroyBlockApplication.Domain.Game;
using DestroyBlockApplication.Domain.Game.Repositories;
using DestroyBlockApplication.Domain.GameSession;
using DestroyBlockApplication.Domain.GameSession.Repositories;
using DestroyBlockApplication.Domain.HallOfFame;
using DestroyBlockApplication.Domain.HallOfFame.Repositories;
using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.Services;
using DestroyBlockApplication.Domain.Shared.ValueObjects;
using DestroyBlockApplication.Domain.User;
using DestroyBlockApplication.Domain.User.Repositories;

namespace DestroyBlockApplication.Domain.Services;

public class GamePlayService : DomainServiceBase
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IHallOfFameRepository _hallOfFameRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClock _clock;

    public GamePlayService(
        IGameRepository gameRepository,
        IGameSessionRepository gameSessionRepository,
        IHallOfFameRepository hallOfFameRepository,
        IUserRepository userRepository,
        IClock clock)
    {
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _gameSessionRepository = gameSessionRepository ?? throw new ArgumentNullException(nameof(gameSessionRepository));
        _hallOfFameRepository = hallOfFameRepository ?? throw new ArgumentNullException(nameof(hallOfFameRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task<GameSessionAggregate> StartGameAsync(Guid playerId, Guid gameId)
    {
        // Verify player exists
        var player = await _userRepository.GetByIdAsync(playerId);
        if (player == null)
        {
            throw new DomainException($"Player with ID {playerId} not found.");
        }

        // Verify game exists and is published
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null)
        {
            throw new DomainException($"Game with ID {gameId} not found.");
        }

        if (!game.IsPublished)
        {
            throw new DomainException("Cannot start unpublished game.");
        }

        // Check if player already has an active session
        var existingSession = await _gameSessionRepository.GetActiveSessionForPlayerAsync(playerId);
        if (existingSession != null)
        {
            throw new DomainException("Player already has an active game session.");
        }

        // Check if player is admin for this game (not allowed)
        if (player.HasRoleForGame(gameId, RoleType.Admin))
        {
            throw new DomainException("Game admin cannot play their own game.");
        }

        // Create new game session
        var session = new GameSessionAggregate(playerId, gameId);
        await _gameSessionRepository.AddAsync(session);

        return session;
    }

    public async Task PauseGameAsync(Guid sessionId, Guid playerId)
    {
        var session = await _gameSessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new DomainException($"Game session with ID {sessionId} not found.");
        }

        if (session.PlayerId != playerId)
        {
            throw new DomainException("Only the session owner can pause the game.");
        }

        session.Pause();
        await _gameSessionRepository.UpdateAsync(session);
    }

    public async Task ResumeGameAsync(Guid sessionId, Guid playerId)
    {
        var session = await _gameSessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new DomainException($"Game session with ID {sessionId} not found.");
        }

        if (session.PlayerId != playerId)
        {
            throw new DomainException("Only the session owner can resume the game.");
        }

        session.Resume();
        await _gameSessionRepository.UpdateAsync(session);
    }

    public async Task CompleteGameAsync(Guid sessionId, Guid playerId)
    {
        var session = await _gameSessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new DomainException($"Game session with ID {sessionId} not found.");
        }

        if (session.PlayerId != playerId)
        {
            throw new DomainException("Only the session owner can complete the game.");
        }

        session.Complete();
        await _gameSessionRepository.UpdateAsync(session);

        // Add to hall of fame
        var hallOfFame = await _hallOfFameRepository.GetByGameIdAsync(session.GameId);
        if (hallOfFame != null)
        {
            var entry = new HighScoreEntry(session.GameId, session.PlayerId, session.Id, 
                session.TotalScore, _clock.Now);
            hallOfFame.AddEntry(entry);
            await _hallOfFameRepository.UpdateAsync(hallOfFame);
        }
    }

    public async Task AdvanceToNextLevelAsync(Guid sessionId, Guid playerId)
    {
        var session = await _gameSessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new DomainException($"Game session with ID {sessionId} not found.");
        }

        if (session.PlayerId != playerId)
        {
            throw new DomainException("Only the session owner can advance levels.");
        }

        var game = await _gameRepository.GetByIdAsync(session.GameId);
        if (game == null)
        {
            throw new DomainException($"Game with ID {session.GameId} not found.");
        }

        // Check if there's a next level
        var nextLevelNumber = new LevelNumber(session.CurrentLevel.Value + 1);
        var nextLevel = game.GetLevel(nextLevelNumber);
        
        if (nextLevel == null)
        {
            // No more levels, complete the game
            await CompleteGameAsync(sessionId, playerId);
            return;
        }

        session.AdvanceToNextLevel();
        await _gameSessionRepository.UpdateAsync(session);
    }
}
