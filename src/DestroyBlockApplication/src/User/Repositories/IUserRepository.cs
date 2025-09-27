using DestroyBlockApplication.Domain.User;

namespace DestroyBlockApplication.Domain.User.Repositories;

public interface IUserRepository
{
    Task<UserAggregate?> GetByIdAsync(Guid id);
    Task<UserAggregate?> GetByUsernameAsync(string username);
    Task<IEnumerable<UserAggregate>> GetAllAsync();
    Task AddAsync(UserAggregate user);
    Task UpdateAsync(UserAggregate user);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> UsernameExistsAsync(string username);
}
