using TileOApplication.Domain.Board;

namespace TileOApplication.Domain.Repositories;

public interface IBoardRepository
{
    Task<BoardAggregate?> GetByIdAsync(Guid id);
    Task<BoardAggregate?> GetByGameIdAsync(Guid gameId);
    Task SaveAsync(BoardAggregate board);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
