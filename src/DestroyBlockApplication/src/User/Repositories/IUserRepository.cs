using DestroyBlockApplication.Domain.User;

namespace DestroyBlockApplication.Domain.User.Repositories;

public interface IUserRepository
{
    Task<UserAggregate?> GetByIdAsync(Guid id);
    Task<UserAggregate?> GetByUsernameAsync(string username);
    Task AddAsync(UserAggregate user);
    Task UpdateAsync(UserAggregate user);
}
