namespace DestroyBlockApplication.Domain.HallOfFame.Repositories;

public interface IHallOfFameRepository
{
    Task<HallOfFameAggregate?> GetByGameIdAsync(Guid gameId);
    Task AddAsync(HallOfFameAggregate hallOfFame);
    Task UpdateAsync(HallOfFameAggregate hallOfFame);
}
