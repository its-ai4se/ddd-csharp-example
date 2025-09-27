using DestroyBlockApplication.Domain.HallOfFame;

namespace DestroyBlockApplication.Domain.HallOfFame.Repositories;

public interface IHallOfFameRepository
{
    Task<HallOfFameAggregate?> GetByGameIdAsync(Guid gameId);
    Task<IEnumerable<HallOfFameAggregate>> GetAllAsync();
    Task AddAsync(HallOfFameAggregate hallOfFame);
    Task UpdateAsync(HallOfFameAggregate hallOfFame);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsForGameAsync(Guid gameId);
}
