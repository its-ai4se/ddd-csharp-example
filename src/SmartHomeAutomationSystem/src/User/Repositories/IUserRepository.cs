using SmartHomeAutomationSystem.Domain.User;

namespace SmartHomeAutomationSystem.Domain.User.Repositories;

public interface IUserRepository
{
    Task<UserAggregate?> GetByIdAsync(Guid id);
    Task<UserAggregate?> GetByEmailAsync(string email);
    Task<List<UserAggregate>> GetAllAsync();
    Task SaveAsync(UserAggregate user);
    Task DeleteAsync(Guid id);
}
