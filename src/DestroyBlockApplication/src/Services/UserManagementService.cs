using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.ValueObjects;
using DestroyBlockApplication.Domain.User;
using DestroyBlockApplication.Domain.User.Repositories;

namespace DestroyBlockApplication.Domain.Services;

public class UserManagementService
{
    private readonly IUserRepository _userRepository;

    public UserManagementService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<UserAggregate> RegisterUserAsync(Username username, Password password)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username.Value);
        if (existingUser != null)
            throw new DomainException($"Username '{username}' is already taken.");

        var user = new UserAggregate(username, password);
        await _userRepository.AddAsync(user);

        return user;
    }

    // BR-004: user must choose a mode; admin mode requires admin privileges
    public async Task<UserSession> LoginAsync(Username username, Password password, LoginMode mode)
    {
        var user = await _userRepository.GetByUsernameAsync(username.Value) ?? throw new DomainException("Invalid username or password.");
        if (!user.VerifyPassword(password))
            throw new DomainException("Invalid username or password.");

        if (mode.Equals(LoginMode.Admin) && !user.IsAdmin)
            throw new DomainException("User does not have admin privileges.");

        return new UserSession(user.Id, user.Username, mode);
    }
}
