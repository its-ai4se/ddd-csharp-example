using TeamSportsScoutingSystem.Domain.Player;
using TeamSportsScoutingSystem.Domain.Services;
using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using TeamSportsScoutingSystem.Domain.Tests.Helpers;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.SigningProcess;

public class SigningProcessTests
{
    private static (OfferManagementService offerSvc, ScoutingManagementService scoutSvc,
        FakePersonRepository personRepo, FakePlayerRepository playerRepo) Build()
    {
        var personRepo = new FakePersonRepository();
        var playerRepo = new FakePlayerRepository();
        var offerSvc = new OfferManagementService(personRepo, playerRepo);
        var scoutSvc = new ScoutingManagementService(
            new FakeScoutingAssignmentRepository(),
            new FakeScoutingReportRepository(),
            playerRepo, personRepo);
        return (offerSvc, scoutSvc, personRepo, playerRepo);
    }

    private static PlayerAggregate AddShortListedPlayer(FakePlayerRepository repo, bool hasFinalRec = false)
    {
        var p = new PlayerAggregate(new PersonName("Short", "Listed"),
            new DateOnly(2000, 1, 1), PlayerListType.ShortList);
        if (hasFinalRec) p.MarkFinalRecommendationIssued();
        repo.AddAsync(p).Wait();
        return p;
    }

    [Fact] 
    public async Task SP001_DirectorCanMakeOfficialOfferAfterFinalRecommendation()
    {
        var (offerSvc, _, personRepo, playerRepo) = Build();
        var director = TestFactory.Director();
        await personRepo.AddAsync(director);
        var player = AddShortListedPlayer(playerRepo, hasFinalRec: true);

        await offerSvc.MakeOfficialOfferAsync(player.Id, director.Id, "Offer details");
    }

    [Fact] 
    public async Task SP002_DirectorCannotMakeOfferWithoutFinalRecommendation()
    {
        var (offerSvc, _, personRepo, playerRepo) = Build();
        var director = TestFactory.Director();
        await personRepo.AddAsync(director);
        var player = AddShortListedPlayer(playerRepo, hasFinalRec: false);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            offerSvc.MakeOfficialOfferAsync(player.Id, director.Id, "Offer"));
        Assert.Contains("rekomendasi Head Scout diperlukan sebelum penawaran dibuat", ex.Message);
    }

    [Fact] 
    public async Task SP003_HeadScoutCannotMakeOfficialOffer()
    {
        var (offerSvc, _, personRepo, playerRepo) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);
        var player = AddShortListedPlayer(playerRepo, hasFinalRec: true);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            offerSvc.MakeOfficialOfferAsync(player.Id, headScout.Id, "Offer"));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task SP004_HeadCoachCannotMakeOfficialOffer()
    {
        var (offerSvc, _, personRepo, playerRepo) = Build();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);
        var player = AddShortListedPlayer(playerRepo, hasFinalRec: true);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            offerSvc.MakeOfficialOfferAsync(player.Id, coach.Id, "Offer"));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task SP005_ScoutCannotMakeOfficialOffer()
    {
        var (offerSvc, _, personRepo, playerRepo) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = AddShortListedPlayer(playerRepo, hasFinalRec: true);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            offerSvc.MakeOfficialOfferAsync(player.Id, scout.Id, "Offer"));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task SP006_HeadScoutCanIssueFinalSigningRecommendation()
    {
        var (_, scoutSvc, personRepo, playerRepo) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);
        var player = AddShortListedPlayer(playerRepo);

        await scoutSvc.IssueFinalSigningRecommendationAsync(player.Id, headScout.Id);

        var updated = await playerRepo.GetByIdAsync(player.Id);
        Assert.True(updated!.HasFinalRecommendation);
    }

    [Fact] 
    public async Task SP007_GenericClassCannotIssueFinalRecommendation()
    {
        var (_, scoutSvc, personRepo, playerRepo) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var player = AddShortListedPlayer(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            scoutSvc.IssueFinalSigningRecommendationAsync(player.Id, scout.Id));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task SP008_HeadCoachCannotIssueFinalRecommendation()
    {
        var (_, scoutSvc, personRepo, playerRepo) = Build();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);
        var player = AddShortListedPlayer(playerRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            scoutSvc.IssueFinalSigningRecommendationAsync(player.Id, coach.Id));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }
}
