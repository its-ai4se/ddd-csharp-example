using TeamSportsScoutingSystem.Domain.Person;
using TeamSportsScoutingSystem.Domain.Person.Repositories;
using TeamSportsScoutingSystem.Domain.Player;
using TeamSportsScoutingSystem.Domain.Player.Repositories;
using TeamSportsScoutingSystem.Domain.PlayerProfile;
using TeamSportsScoutingSystem.Domain.PlayerProfile.Repositories;
using TeamSportsScoutingSystem.Domain.ScoutingAssignment;
using TeamSportsScoutingSystem.Domain.ScoutingAssignment.Repositories;
using TeamSportsScoutingSystem.Domain.ScoutingReport;
using TeamSportsScoutingSystem.Domain.ScoutingReport.Repositories;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Tests.Helpers;

public class FakePersonRepository : IPersonRepository
{
    private readonly Dictionary<Guid, PersonAggregate> _store = [];
    public Task<PersonAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var p) ? p : null);
    public Task AddAsync(PersonAggregate person, CancellationToken ct = default)
    { _store[person.Id] = person; return Task.CompletedTask; }
    public Task UpdateAsync(PersonAggregate person, CancellationToken ct = default)
    { _store[person.Id] = person; return Task.CompletedTask; }
}

public class FakePlayerRepository : IPlayerRepository
{
    private readonly Dictionary<Guid, PlayerAggregate> _store = [];
    public Task<PlayerAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var p) ? p : null);
    public Task<IEnumerable<PlayerAggregate>> GetByListTypeAsync(string listType, CancellationToken ct = default)
        => Task.FromResult(_store.Values.Where(p => p.ListType.Type == listType));
    public Task AddAsync(PlayerAggregate player, CancellationToken ct = default)
    { _store[player.Id] = player; return Task.CompletedTask; }
    public Task UpdateAsync(PlayerAggregate player, CancellationToken ct = default)
    { _store[player.Id] = player; return Task.CompletedTask; }
}

public class FakePlayerProfileRepository : IPlayerProfileRepository
{
    private readonly Dictionary<Guid, PlayerProfileAggregate> _store = [];
    public Task<PlayerProfileAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var p) ? p : null);
    public Task AddAsync(PlayerProfileAggregate profile, CancellationToken ct = default)
    { _store[profile.Id] = profile; return Task.CompletedTask; }
}

public class FakeScoutingAssignmentRepository : IScoutingAssignmentRepository
{
    private readonly Dictionary<Guid, ScoutingAssignmentAggregate> _store = [];
    public Task<ScoutingAssignmentAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var a) ? a : null);
    public Task AddAsync(ScoutingAssignmentAggregate assignment, CancellationToken ct = default)
    { _store[assignment.Id] = assignment; return Task.CompletedTask; }
    public Task UpdateAsync(ScoutingAssignmentAggregate assignment, CancellationToken ct = default)
    { _store[assignment.Id] = assignment; return Task.CompletedTask; }
}

public class FakeScoutingReportRepository : IScoutingReportRepository
{
    private readonly List<ScoutingReportAggregate> _store = [];
    public Task<ScoutingReportAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.FirstOrDefault(r => r.Id == id));
    public Task<IEnumerable<ScoutingReportAggregate>> GetByPlayerAsync(Guid playerId, CancellationToken ct = default)
        => Task.FromResult(_store.Where(r => r.PlayerId == playerId));
    public Task<IEnumerable<ScoutingReportAggregate>> GetByScoutingAssignmentAsync(Guid assignmentId, CancellationToken ct = default)
        => Task.FromResult(_store.Where(r => r.ScoutingAssignmentId == assignmentId));
    public Task AddAsync(ScoutingReportAggregate report, CancellationToken ct = default)
    { _store.Add(report); return Task.CompletedTask; }
    public Task UpdateAsync(ScoutingReportAggregate report, CancellationToken ct = default)
        => Task.CompletedTask;
}

public static class TestFactory
{
    public static PersonAggregate HeadCoach()
    {
        var p = new PersonAggregate(new PersonName("Head", "Coach"));
        p.AddRole(new HeadCoachRole(p.Id));
        return p;
    }
    public static PersonAggregate Director()
    {
        var p = new PersonAggregate(new PersonName("The", "Director"));
        p.AddRole(new DirectorRole(p.Id));
        return p;
    }
    public static PersonAggregate Scout(bool isHeadScout = false)
    {
        var p = new PersonAggregate(new PersonName("Scout", "Person"));
        p.AddRole(new ScoutRole(p.Id, isHeadScout));
        return p;
    }
    public static PersonAggregate HeadScout() => Scout(isHeadScout: true);
    public static PersonAggregate NoRole() => new PersonAggregate(new PersonName("No", "Role"));
}
