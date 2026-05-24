using TeamSportsScoutingSystem.Domain.Player;
using TeamSportsScoutingSystem.Domain.ScoutingAssignment;
using TeamSportsScoutingSystem.Domain.ScoutingReport;
using TeamSportsScoutingSystem.Domain.Services;
using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using TeamSportsScoutingSystem.Domain.Tests.Helpers;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.ScoutingReport;

public class ScoutingReportTests
{
    private (ScoutingManagementService svc, FakePersonRepository personRepo,
        FakePlayerRepository playerRepo, FakeScoutingAssignmentRepository assignRepo,
        FakeScoutingReportRepository reportRepo) Build()
    {
        var personRepo = new FakePersonRepository();
        var playerRepo = new FakePlayerRepository();
        var assignRepo = new FakeScoutingAssignmentRepository();
        var reportRepo = new FakeScoutingReportRepository();
        return (new ScoutingManagementService(assignRepo, reportRepo, playerRepo, personRepo),
            personRepo, playerRepo, assignRepo, reportRepo);
    }

    private static ScoutingAssignmentAggregate AddAssignment(
        FakeScoutingAssignmentRepository repo, Guid playerId, Guid scoutId)
    {
        var a = new ScoutingAssignmentAggregate(playerId, scoutId);
        repo.AddAsync(a).Wait();
        return a;
    }

    private static async Task<PlayerAggregate> AddPlayerAsync(FakePlayerRepository repo)
    {
        var p = new PlayerAggregate(new PersonName("Test", "Player"),
            new DateOnly(2000, 1, 1), PlayerListType.LongList);
        await repo.AddAsync(p);
        return p;
    }

    [Fact] 
    public async Task SR001_ScoutCanSubmitReport()
    {
        var (svc, personRepo, playerRepo, assignRepo, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, scout.Id);

        var report = await svc.SubmitScoutingReportAsync(
            scout.Id, player.Id, assignment.Id, "fast", "weak header", "First Team Player");

        Assert.NotNull(report);
    }

    [Fact] 
    public async Task SR002_SubmitReportWithNullProsShouldThrowsError()
    {
        var (svc, personRepo, playerRepo, assignRepo, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, scout.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.SubmitScoutingReportAsync(scout.Id, player.Id, assignment.Id,
                null!, "weak header", "Key Player"));
        Assert.Contains("pros wajib diisi", ex.Message);
    }

    [Fact] 
    public async Task SR003_SubmitReportWithNullConsShouldThrowsError()
    {
        var (svc, personRepo, playerRepo, assignRepo, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, scout.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.SubmitScoutingReportAsync(scout.Id, player.Id, assignment.Id,
                "fast", null!, "Key Player"));
        Assert.Contains("cons wajib diisi", ex.Message);
    }

    [Fact] 
    public async Task SR004_SubmitReportWithNullRecommendationShouldThrowsError()
    {
        var (svc, personRepo, playerRepo, assignRepo, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, scout.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.SubmitScoutingReportAsync(scout.Id, player.Id, assignment.Id,
                "fast", "slow", null!));
        Assert.Contains("rekomendasi wajib diisi", ex.Message);
    }

    // SR-005 to SR-009
    [Theory] 
    [InlineData("Key Player")]
    [InlineData("First Team Player")]
    [InlineData("Reserve Team Player")]
    [InlineData("Prospective Player")]
    [InlineData("Not a Good Signing")]
    public async Task SR005_SR006_SR007_SR008_SR009_SubmitReport_WithValidRecommendation_Succeeds(string rec)
    {
        var (svc, personRepo, playerRepo, assignRepo, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, scout.Id);

        var report = await svc.SubmitScoutingReportAsync(
            scout.Id, player.Id, assignment.Id, "fast", "slow", rec);

        Assert.NotNull(report);
    }

    [Fact] 
    public async Task SR010_SubmitReport_WithInvalidRecommendation_Throws()
    {
        var (svc, personRepo, playerRepo, assignRepo, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, scout.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.SubmitScoutingReportAsync(scout.Id, player.Id, assignment.Id,
                "fast", "slow", "Maybe"));
        Assert.Contains("nilai rekomendasi tidak valid", ex.Message);
    }

    [Fact] 
    public async Task SR011_SubmitReport_WithEmptyAssignmentId_Throws()
    {
        var (svc, personRepo, playerRepo, _, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.SubmitScoutingReportAsync(scout.Id, player.Id, Guid.Empty,
                "fast", "weak", "Key Player"));
        Assert.Contains("assignment ID wajib ada", ex.Message);
    }

    [Fact] 
    public async Task SR012_Director_CannotSubmitReport()
    {
        var (svc, personRepo, playerRepo, assignRepo, _) = Build();
        var director = TestFactory.Director();
        await personRepo.AddAsync(director);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, director.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.SubmitScoutingReportAsync(director.Id, player.Id, assignment.Id,
                "fast", "slow", "Key Player"));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task SR013_HeadCoach_CannotSubmitReport()
    {
        var (svc, personRepo, playerRepo, assignRepo, _) = Build();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, coach.Id);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.SubmitScoutingReportAsync(coach.Id, player.Id, assignment.Id,
                "fast", "slow", "Key Player"));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task SR014_MoveToShortList_WithOneReviewedReport_Succeeds()
    {
        var (svc, personRepo, playerRepo, assignRepo, reportRepo) = Build();
        var coach = TestFactory.HeadCoach();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(coach);
        await personRepo.AddAsync(headScout);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, headScout.Id);
        var report = new ScoutingReportAggregate(player.Id, headScout.Id, assignment.Id,
            "fast", "slow", Recommendation.KeyPlayer);
        report.MarkAsReviewed();
        await reportRepo.AddAsync(report);

        await svc.MovePlayerToShortListWithApprovalAsync(player.Id, coach.Id, headScout.Id);

        var updated = await playerRepo.GetByIdAsync(player.Id);
        Assert.Equal(PlayerListType.ShortList.Type, updated!.ListType.Type);
    }

    [Fact] 
    public async Task SR015_MoveToShortList_WithNoReport_Throws()
    {
        var (svc, personRepo, playerRepo, _, _) = Build();
        var coach = TestFactory.HeadCoach();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(coach);
        await personRepo.AddAsync(headScout);
        var player = await AddPlayerAsync(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.MovePlayerToShortListWithApprovalAsync(player.Id, coach.Id, headScout.Id));
        Assert.Contains("minimal satu scouting report", ex.Message);
    }

    [Fact] 
    public async Task SR016_MoveToShortList_WithoutHeadCoachApproval_Throws()
    {
        var (svc, personRepo, playerRepo, assignRepo, reportRepo) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);
        var player = await AddPlayerAsync(playerRepo);
        var assignment = AddAssignment(assignRepo, player.Id, headScout.Id);
        await reportRepo.AddAsync(new ScoutingReportAggregate(player.Id, headScout.Id, assignment.Id,
            "fast", "slow", Recommendation.KeyPlayer));

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.MovePlayerToShortListWithApprovalAsync(player.Id, Guid.NewGuid(), headScout.Id));
        Assert.Contains("persetujuan Head Coach diperlukan", ex.Message);
    }

    [Fact] 
    public async Task SR017_MoveToShortList_WithoutHeadScoutApproval_Throws()
    {
        var (svc, personRepo, playerRepo, assignRepo, reportRepo) = Build();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);
        var player = await AddPlayerAsync(playerRepo);
        var fakeScoutId = Guid.NewGuid();
        var assignment = AddAssignment(assignRepo, player.Id, fakeScoutId);
        await reportRepo.AddAsync(new ScoutingReportAggregate(player.Id, fakeScoutId, assignment.Id,
            "fast", "slow", Recommendation.KeyPlayer));

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.MovePlayerToShortListWithApprovalAsync(player.Id, coach.Id, Guid.NewGuid()));
        Assert.Contains("persetujuan Head Scout diperlukan", ex.Message);
    }

    [Fact] 
    public async Task SR018_RegularScout_CannotMoveToShortList()
    {
        var (svc, personRepo, playerRepo, _, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = await AddPlayerAsync(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.MovePlayerToShortListWithApprovalAsync(player.Id, scout.Id, scout.Id));
        Assert.Contains("persetujuan Head Coach diperlukan", ex.Message);
    }

    [Fact] 
    public async Task SR019_Director_CannotMoveToShortList()
    {
        var (svc, personRepo, playerRepo, _, _) = Build();
        var director = TestFactory.Director();
        await personRepo.AddAsync(director);
        var player = await AddPlayerAsync(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.MovePlayerToShortListWithApprovalAsync(player.Id, director.Id, director.Id));
        Assert.Contains("persetujuan Head Coach diperlukan", ex.Message);
    }

    [Fact] 
    public async Task SR020_ShortListedPlayer_CanReceiveNewAssignment()
    {
        var (svc, personRepo, playerRepo, _, _) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);
        var player = new PlayerAggregate(new PersonName("Short", "Listed"),
            new DateOnly(2000, 1, 1), PlayerListType.ShortList);
        await playerRepo.AddAsync(player);

        var assignment = await svc.CreateScoutingAssignmentAsync(
            headScout.Id, player.Id, headScout.Id);

        Assert.NotNull(assignment);
        Assert.Equal(player.Id, assignment.PlayerId);
    }
}
