using DestroyBlockApplication.Domain.GameSession;

namespace DestroyBlockApplication.Domain.GameSession.Repositories;

public interface IGameSessionRepository
{
    Task<GameSessionAggregate?> GetByIdAsync(Guid id);
    // BR-034: returns any non-terminal (ongoing) session for the player; games cannot be played in parallel
    Task<GameSessionAggregate?> GetActiveSessionForPlayerAsync(Guid playerId);
    Task AddAsync(GameSessionAggregate session);
    Task UpdateAsync(GameSessionAggregate session);
}
