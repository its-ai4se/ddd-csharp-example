namespace SmartHomeAutomationSystem.Domain.Home.Repositories;

public interface IHomeRepository
{
    Task<HomeAggregate?> GetByIdAsync(Guid id);
    Task SaveAsync(HomeAggregate home);
}
