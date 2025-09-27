using DestroyBlockApplication.Domain.GameSession;

namespace DestroyBlockApplication.Domain.GameSession.Repositories;

public interface IGameSessionRepository
{
    Task<GameSessionAggregate?> GetByIdAsync(Guid id);
    Task<GameSessionAggregate?> GetActiveSessionForPlayerAsync(Guid playerId);
    Task<IEnumerable<GameSessionAggregate>> GetSessionsByPlayerAsync(Guid playerId);
    Task<IEnumerable<GameSessionAggregate>> GetSessionsByGameAsync(Guid gameId);
    Task<IEnumerable<GameSessionAggregate>> GetCompletedSessionsByGameAsync(Guid gameId);
    Task AddAsync(GameSessionAggregate session);
    Task UpdateAsync(GameSessionAggregate session);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> HasActiveSessionAsync(Guid playerId);
}
