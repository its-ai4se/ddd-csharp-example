using DestroyBlockApplication.Domain.Game;
using DestroyBlockApplication.Domain.Game.Repositories;
using DestroyBlockApplication.Domain.GameSession;
using DestroyBlockApplication.Domain.GameSession.Repositories;
using DestroyBlockApplication.Domain.HallOfFame;
using DestroyBlockApplication.Domain.HallOfFame.Repositories;
using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.Services;

public class GamePlayService
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IHallOfFameRepository _hallOfFameRepository;

    public GamePlayService(
        IGameRepository gameRepository,
        IGameSessionRepository gameSessionRepository,
        IHallOfFameRepository hallOfFameRepository)
    {
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _gameSessionRepository = gameSessionRepository ?? throw new ArgumentNullException(nameof(gameSessionRepository));
        _hallOfFameRepository = hallOfFameRepository ?? throw new ArgumentNullException(nameof(hallOfFameRepository));
    }

    public async Task<GameSessionAggregate> StartGameAsync(Guid playerId, Guid gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId) ?? throw new DomainException($"Game with ID {gameId} not found.");
        if (!game.IsPublished)
            throw new DomainException("Cannot start unpublished game.");

        // BR-036: only one active session at a time
        var existingSession = await _gameSessionRepository.GetActiveSessionForPlayerAsync(playerId);
        if (existingSession != null)
            throw new DomainException("Player already has an active game session.");

        // BR-005/BR-007: the game's admin cannot play their own game
        if (game.AdminId == playerId)
            throw new DomainException("Game admin cannot play their own game.");

        var session = new GameSessionAggregate(playerId, gameId);
        await _gameSessionRepository.AddAsync(session);

        return session;
    }

    public async Task PauseGameAsync(Guid sessionId, Guid playerId)
    {
        var session = await _gameSessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException($"Game session with ID {sessionId} not found.");
        if (session.PlayerId != playerId)
            throw new DomainException("Only the session owner can pause the game.");

        session.Pause();
        await _gameSessionRepository.UpdateAsync(session);
    }

    public async Task ResumeGameAsync(Guid sessionId, Guid playerId)
    {
        var session = await _gameSessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException($"Game session with ID {sessionId} not found.");
        if (session.PlayerId != playerId)
            throw new DomainException("Only the session owner can resume the game.");

        session.Resume();
        await _gameSessionRepository.UpdateAsync(session);
    }

    // BR-030: game ends, score goes to hall of fame
    public async Task CompleteGameAsync(Guid sessionId, Guid playerId)
    {
        var session = await _gameSessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException($"Game session with ID {sessionId} not found.");
        if (session.PlayerId != playerId)
            throw new DomainException("Only the session owner can complete the game.");

        session.Complete();
        await _gameSessionRepository.UpdateAsync(session);

        var hallOfFame = await _hallOfFameRepository.GetByGameIdAsync(session.GameId);
        if (hallOfFame != null)
        {
            var entry = new HighScoreEntry(session.GameId, session.PlayerId, session.Id,
                session.TotalScore, session.CompletedAt!.Value);
            hallOfFame.AddEntry(entry);
            await _hallOfFameRepository.UpdateAsync(hallOfFame);
        }
    }

    // BR-027: called when the last block of the current level is destroyed
    public async Task CompleteLevelAsync(Guid sessionId, Guid playerId)
    {
        var session = await _gameSessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException($"Game session with ID {sessionId} not found.");
        if (session.PlayerId != playerId)
            throw new DomainException("Only the session owner can complete a level.");

        var game = await _gameRepository.GetByIdAsync(session.GameId) ?? throw new DomainException($"Game with ID {session.GameId} not found.");

        // No next level means the game is over (BR-030)
        var nextLevelNumber = new LevelNumber(session.CurrentLevel.Value + 1);
        if (game.GetLevel(nextLevelNumber) == null)
        {
            await CompleteGameAsync(sessionId, playerId);
            return;
        }

        session.CompleteLevel();
        await _gameSessionRepository.UpdateAsync(session);
    }

    public async Task ConfirmNextLevelAsync(Guid sessionId, Guid playerId)
    {
        var session = await _gameSessionRepository.GetByIdAsync(sessionId) ?? throw new DomainException($"Game session with ID {sessionId} not found.");
        if (session.PlayerId != playerId)
            throw new DomainException("Only the session owner can confirm the next level.");

        session.AdvanceToNextLevel();
        await _gameSessionRepository.UpdateAsync(session);
    }
}
