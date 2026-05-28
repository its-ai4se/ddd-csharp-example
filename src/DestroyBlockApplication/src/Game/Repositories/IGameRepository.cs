namespace DestroyBlockApplication.Domain.Game.Repositories;

public interface IGameRepository
{
    Task<GameAggregate?> GetByIdAsync(Guid id);
    Task<GameAggregate?> GetByNameAsync(string name);
    Task AddAsync(GameAggregate game);
    Task UpdateAsync(GameAggregate game);
}
