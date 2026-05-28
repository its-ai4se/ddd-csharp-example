using DestroyBlockApplication.Domain.Game;
using DestroyBlockApplication.Domain.Game.Repositories;
using DestroyBlockApplication.Domain.GameSession;
using DestroyBlockApplication.Domain.GameSession.Repositories;
using DestroyBlockApplication.Domain.HallOfFame;
using DestroyBlockApplication.Domain.HallOfFame.Repositories;
using DestroyBlockApplication.Domain.User;
using DestroyBlockApplication.Domain.User.Repositories;

namespace DestroyBlockApplication.Domain.Tests.TestHelpers;

class FakeUserRepository : IUserRepository
{
    private readonly List<UserAggregate> _users = [];
    public Task<UserAggregate?> GetByIdAsync(Guid id) => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    public Task<UserAggregate?> GetByUsernameAsync(string username) => Task.FromResult(_users.FirstOrDefault(u => u.Username.Value == username));
    public Task AddAsync(UserAggregate user) { _users.Add(user); return Task.CompletedTask; }
    public Task UpdateAsync(UserAggregate user) => Task.CompletedTask;
}

class FakeGameRepository : IGameRepository
{
    private readonly List<GameAggregate> _games = [];
    public Task<GameAggregate?> GetByIdAsync(Guid id) => Task.FromResult(_games.FirstOrDefault(g => g.Id == id));
    public Task<GameAggregate?> GetByNameAsync(string name) => Task.FromResult(_games.FirstOrDefault(g => g.Name.Value == name));
    public Task AddAsync(GameAggregate game) { _games.Add(game); return Task.CompletedTask; }
    public Task UpdateAsync(GameAggregate game) => Task.CompletedTask;
}

class FakeHallOfFameRepository : IHallOfFameRepository
{
    private readonly List<HallOfFameAggregate> _hofs = [];
    public Task<HallOfFameAggregate?> GetByGameIdAsync(Guid gameId) => Task.FromResult(_hofs.FirstOrDefault(h => h.GameId == gameId));
    public Task AddAsync(HallOfFameAggregate hof) { _hofs.Add(hof); return Task.CompletedTask; }
    public Task UpdateAsync(HallOfFameAggregate hof) => Task.CompletedTask;
}

class FakeGameSessionRepository : IGameSessionRepository
{
    private readonly List<GameSessionAggregate> _sessions = [];
    public Task<GameSessionAggregate?> GetByIdAsync(Guid id) => Task.FromResult(_sessions.FirstOrDefault(s => s.Id == id));
    public Task<GameSessionAggregate?> GetActiveSessionForPlayerAsync(Guid playerId) =>
        Task.FromResult(_sessions.FirstOrDefault(s => s.PlayerId == playerId && (s.IsActive || s.IsPaused)));
    public Task AddAsync(GameSessionAggregate session) { _sessions.Add(session); return Task.CompletedTask; }
    public Task UpdateAsync(GameSessionAggregate session) => Task.CompletedTask;
}
