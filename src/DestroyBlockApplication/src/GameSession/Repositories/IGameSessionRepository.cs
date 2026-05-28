using DestroyBlockApplication.Domain.GameSession;

namespace DestroyBlockApplication.Domain.GameSession.Repositories;

public interface IGameSessionRepository
{
    Task<GameSessionAggregate?> GetByIdAsync(Guid id);
    Task<GameSessionAggregate?> GetActiveSessionForPlayerAsync(Guid playerId); // BR-036
    Task AddAsync(GameSessionAggregate session);
    Task UpdateAsync(GameSessionAggregate session);
}
