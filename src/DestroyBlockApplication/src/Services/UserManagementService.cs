using DestroyBlockApplication.Domain.Game;
using DestroyBlockApplication.Domain.Game.Repositories;
using DestroyBlockApplication.Domain.GameSession;
using DestroyBlockApplication.Domain.GameSession.Repositories;
using DestroyBlockApplication.Domain.Shared.Common;
using DestroyBlockApplication.Domain.Shared.Services;
using DestroyBlockApplication.Domain.Shared.ValueObjects;
using DestroyBlockApplication.Domain.User;
using DestroyBlockApplication.Domain.User.Repositories;

namespace DestroyBlockApplication.Domain.Services;

public class UserManagementService : DomainServiceBase
{
    private readonly IUserRepository _userRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IGameSessionRepository _gameSessionRepository;

    public UserManagementService(
        IUserRepository userRepository,
        IGameRepository gameRepository,
        IGameSessionRepository gameSessionRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _gameSessionRepository = gameSessionRepository ?? throw new ArgumentNullException(nameof(gameSessionRepository));
    }

    public async Task<UserAggregate> RegisterUserAsync(Username username, Password password)
    {
        // Check if username already exists
        var existingUser = await _userRepository.GetByUsernameAsync(username.Value);
        if (existingUser != null)
        {
            throw new DomainException($"Username '{username}' is already taken.");
        }

        var user = new UserAggregate(username, password);
        await _userRepository.AddAsync(user);

        return user;
    }

    public async Task<UserAggregate> LoginAsync(Username username, Password password)
    {
        var user = await _userRepository.GetByUsernameAsync(username.Value);
        if (user == null)
        {
            throw new DomainException("Invalid username or password.");
        }

        if (!user.VerifyPassword(password))
        {
            throw new DomainException("Invalid username or password.");
        }

        return user;
    }

    public async Task PromoteToAdminAsync(Guid userId, Guid adminId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new DomainException($"User with ID {userId} not found.");
        }

        var admin = await _userRepository.GetByIdAsync(adminId);
        if (admin == null || !admin.IsAdmin)
        {
            throw new DomainException("Only admins can promote users to admin.");
        }

        user.PromoteToAdmin();
        await _userRepository.UpdateAsync(user);
    }

    public async Task AssignGameRoleAsync(Guid userId, Guid gameId, RoleType roleType)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new DomainException($"User with ID {userId} not found.");
        }

        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null)
        {
            throw new DomainException($"Game with ID {gameId} not found.");
        }

        // Check if user already has a role for this game
        if (user.HasRoleForGame(gameId, roleType))
        {
            throw new DomainException($"User already has {roleType} role for this game.");
        }

        // If assigning admin role, check if there's already an admin for this game
        if (roleType == RoleType.Admin)
        {
            if (game.AdminId != userId)
            {
                throw new DomainException("Only the game creator can be the admin.");
            }
        }

        // Check if user has an active game session for this game
        var activeSession = await _gameSessionRepository.GetActiveSessionForPlayerAsync(userId);
        if (activeSession != null && activeSession.GameId == gameId)
        {
            throw new DomainException("Cannot assign role while user has an active game session.");
        }

        var gameRole = new GameRole(userId, gameId, roleType);
        user.AddGameRole(gameRole);
        await _userRepository.UpdateAsync(user);
    }
}
