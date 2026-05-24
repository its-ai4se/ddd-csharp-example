using TeamSportsScoutingSystem.Domain.Player;
using TeamSportsScoutingSystem.Domain.Services;
using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using TeamSportsScoutingSystem.Domain.Tests.Helpers;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.ScoutingAssignment;

public class ScoutingAssignmentTests
{
    private (ScoutingManagementService svc, FakePersonRepository personRepo,
        FakePlayerRepository playerRepo) Build()
    {
        var personRepo = new FakePersonRepository();
        var playerRepo = new FakePlayerRepository();
        var svc = new ScoutingManagementService(
            new FakeScoutingAssignmentRepository(),
            new FakeScoutingReportRepository(),
            playerRepo, personRepo);
        return (svc, personRepo, playerRepo);
    }

    private static async Task<PlayerAggregate> AddPlayerAsync(FakePlayerRepository repo, PlayerListType? listType = null)
    {
        var player = new PlayerAggregate(
            new PersonName("Test", "Player"), new DateOnly(2000, 1, 1),
            listType ?? PlayerListType.LongList);
        await repo.AddAsync(player);
        return player;
    }

    private static PlayerAggregate AddPlayer(FakePlayerRepository repo, PlayerListType? listType = null)
    {
        var player = new PlayerAggregate(
            new PersonName("Test", "Player"), new DateOnly(2000, 1, 1),
            listType ?? PlayerListType.LongList);
        repo.AddAsync(player).Wait();
        return player;
    }

    [Fact] 
    public async Task SA001_HeadScoutCanCreateAssignmentForPlayer()
    {
        var (svc, personRepo, playerRepo) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);
        var player = await AddPlayerAsync(playerRepo);

        var assignment = await svc.CreateScoutingAssignmentAsync(headScout.Id, player.Id, headScout.Id);

        Assert.NotNull(assignment);
        Assert.Equal(player.Id, assignment.PlayerId);
    }

    [Fact] 
    public async Task SA002_CreateAssignmentWithEmptyPlayerIdShouldThrows()
    {
        var (svc, personRepo, _) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.CreateScoutingAssignmentAsync(headScout.Id, Guid.Empty, headScout.Id));
        Assert.Contains("pemain target wajib ditentukan", ex.Message);
    }

    [Fact] 
    public async Task SA003_CreateAssignmentWithMultiplePlayerIdsShouldThrows()
    {
        var (svc, personRepo, playerRepo) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);
        var p1 = await AddPlayerAsync(playerRepo);
        var p2 = await AddPlayerAsync(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.CreateScoutingAssignmentAsync(headScout.Id, Guid.Empty, headScout.Id,
                [p1.Id, p2.Id]));
        Assert.Contains("hanya satu pemain per assignment", ex.Message);
    }

    [Fact] 
    public async Task SA004_RegularScoutCannotCreateAssignment()
    {
        var (svc, personRepo, playerRepo) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.CreateScoutingAssignmentAsync(scout.Id, player.Id, scout.Id));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task SA005_HeadCoachCannotCreateAssignment()
    {
        var (svc, personRepo, playerRepo) = Build();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);
        var player = await AddPlayerAsync(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.CreateScoutingAssignmentAsync(coach.Id, player.Id, coach.Id));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task SA006_DirectorCannotCreateAssignment()
    {
        var (svc, personRepo, playerRepo) = Build();
        var director = TestFactory.Director();
        await personRepo.AddAsync(director);
        var player = await AddPlayerAsync(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.CreateScoutingAssignmentAsync(director.Id, player.Id, director.Id));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task SA007_HeadScout_CanSelfAssign()
    {
        var (svc, personRepo, playerRepo) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);
        var player = await AddPlayerAsync(playerRepo);

        var assignment = await svc.CreateScoutingAssignmentAsync(headScout.Id, player.Id, headScout.Id);

        Assert.Equal(headScout.Id, assignment.AssignedScoutId);
    }

    [Fact] 
    public async Task SA008_HeadScout_CanDelegateToOtherScout()
    {
        var (svc, personRepo, playerRepo) = Build();
        var headScout = TestFactory.HeadScout();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(headScout);
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);

        var assignment = await svc.CreateScoutingAssignmentAsync(headScout.Id, player.Id, scout.Id);

        Assert.Equal(scout.Id, assignment.AssignedScoutId);
    }

    [Fact] 
    public async Task SA009_ShortListedPlayer_CanHaveMultipleAssignments()
    {
        var (svc, personRepo, playerRepo) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);
        var player = await AddPlayerAsync(playerRepo, PlayerListType.ShortList);

        var a1 = await svc.CreateScoutingAssignmentAsync(headScout.Id, player.Id, headScout.Id);
        var a2 = await svc.CreateScoutingAssignmentAsync(headScout.Id, player.Id, headScout.Id);

        Assert.NotEqual(a1.Id, a2.Id);
        Assert.Equal(player.Id, a2.PlayerId);
    }
}
