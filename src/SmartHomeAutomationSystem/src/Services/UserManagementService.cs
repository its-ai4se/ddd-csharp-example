using SmartHomeAutomationSystem.Domain.Shared.Services;
using SmartHomeAutomationSystem.Domain.User;
using SmartHomeAutomationSystem.Domain.User.Repositories;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Services;

public class UserManagementService : DomainServiceBase
{
    private readonly IUserRepository _userRepository;

    public UserManagementService(
        IClock clock,
        IUserRepository userRepository) : base(clock)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<UserAggregate> CreateUserAsync(UserName name, EmailAddress email)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email.Value);
        if (existingUser != null)
            throw new DomainException("User with this email already exists.");

        var user = new UserAggregate(name, email);
        await _userRepository.SaveAsync(user);
        return user;
    }

    public async Task AddAdminRoleAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new DomainException("User not found.");

        user.AddRole(new AdminRole(userId));
        await _userRepository.SaveAsync(user);
    }

    public async Task AddResidentRoleAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new DomainException("User not found.");

        user.AddRole(new ResidentRole(userId));
        await _userRepository.SaveAsync(user);
    }

    public async Task AddGuestRoleAsync(Guid userId, DateTime expiresAt)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new DomainException("User not found.");

        user.AddRole(new GuestRole(userId, expiresAt));
        await _userRepository.SaveAsync(user);
    }

    public async Task RemoveRoleAsync(Guid userId, string roleType)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new DomainException("User not found.");

        user.RemoveRole(roleType);
        await _userRepository.SaveAsync(user);
    }

    public async Task<bool> HasRoleAsync(Guid userId, string roleType)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new DomainException("User not found.");

        return user.HasRole(roleType);
    }

    public async Task UpdateUserLastLoginAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new DomainException("User not found.");

        user.UpdateLastLogin();
        await _userRepository.SaveAsync(user);
    }

    public async Task<UserAggregate?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }
}
