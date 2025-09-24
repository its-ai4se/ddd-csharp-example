using SmartHomeAutomationSystem.Domain.Home;

namespace SmartHomeAutomationSystem.Domain.Home.Repositories;

public interface IHomeRepository
{
    Task<HomeAggregate?> GetByIdAsync(Guid id);
    Task<List<HomeAggregate>> GetAllAsync();
    Task SaveAsync(HomeAggregate home);
    Task DeleteAsync(Guid id);
}
