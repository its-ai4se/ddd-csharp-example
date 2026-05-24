using TeamSportsScoutingSystem.Domain.PlayerProfile;
using TeamSportsScoutingSystem.Domain.Services;
using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using TeamSportsScoutingSystem.Domain.Tests.Helpers;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.LongList;

public class LongListTests
{
    private static (PlayerManagementService svc, FakePersonRepository personRepo,
        FakePlayerProfileRepository profileRepo, FakePlayerRepository playerRepo) Build()
    {
        var personRepo = new FakePersonRepository();
        var profileRepo = new FakePlayerProfileRepository();
        var playerRepo = new FakePlayerRepository();
        return (new PlayerManagementService(playerRepo, profileRepo, personRepo),
            personRepo, profileRepo, playerRepo);
    }

    private static PlayerProfileAggregate MakeProfile(string posCode)
    {
        var profile = new PlayerProfileAggregate("Profile", Guid.NewGuid());
        profile.AddTargetPosition(new Position(posCode));
        return profile;
    }

    [Fact] 
    public async Task LL001_ScoutCanAddPlayerToLongList()
    {
        var (svc, personRepo, profileRepo, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var profile = MakeProfile("GK");
        await profileRepo.AddAsync(profile);

        var player = await svc.AddPlayerToLongListAsync(scout.Id,
            new PersonName("John", "Doe"), new DateOnly(2000, 1, 1),
            profile.Id, [new PlayerAttribute("position", "GK")]);

        Assert.NotNull(player);
        Assert.Equal(PlayerListType.LongList.Type, player.ListType.Type);
    }

    [Fact] 
    public async Task LL002_AddPlayerWithNullProfileShouldThrows()
    {
        var (svc, personRepo, _, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.AddPlayerToLongListAsync(scout.Id,
                new PersonName("Jane", "Smith"), new DateOnly(2000, 1, 1),
                null, []));
        Assert.Contains("pemain tidak cocok dengan profil target", ex.Message);
    }

    [Fact] 
    public async Task LL003_HeadCoachCannotAddPlayerToLongList()
    {
        var (svc, personRepo, profileRepo, _) = Build();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);
        var profile = MakeProfile("LB");
        await profileRepo.AddAsync(profile);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.AddPlayerToLongListAsync(coach.Id,
                new PersonName("John", "Doe"), new DateOnly(2000, 1, 1),
                profile.Id, []));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task LL004_DirectorCannotAddPlayerToLongList()
    {
        var (svc, personRepo, profileRepo, _) = Build();
        var director = TestFactory.Director();
        await personRepo.AddAsync(director);
        var profile = MakeProfile("ST");
        await profileRepo.AddAsync(profile);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.AddPlayerToLongListAsync(director.Id,
                new PersonName("John", "Doe"), new DateOnly(2000, 1, 1),
                profile.Id, []));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task LL005_ScoutCanAddPlayerAtAnyTime()
    {
        var (svc, personRepo, profileRepo, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var profile = MakeProfile("CB");
        await profileRepo.AddAsync(profile);

        var player = await svc.AddPlayerToLongListAsync(scout.Id,
            new PersonName("Carlos", "Ruiz"), new DateOnly(1998, 3, 15),
            profile.Id, [new PlayerAttribute("position", "CB")]);

        Assert.NotNull(player);
    }

    [Fact] 
    public async Task LL006_HeadScoutCanEvaluateLongList()
    {
        var (svc, personRepo, _, _) = Build();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(headScout);

        var result = await svc.EvaluateLongListAsync(headScout.Id);
        Assert.NotNull(result);
    }

    [Fact] 
    public async Task LL007_RegularScoutCannotEvaluateLongList()
    {
        var (svc, personRepo, _, _) = Build();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.EvaluateLongListAsync(scout.Id));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task LL008_HeadCoachCannotEvaluateLongList()
    {
        var (svc, personRepo, _, _) = Build();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.EvaluateLongListAsync(coach.Id));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact] 
    public async Task LL009_AddingPlayerDoesNotAutoTriggerEvaluation()
    {
        var (svc, personRepo, profileRepo, playerRepo) = Build();
        var scout = TestFactory.Scout();
        var headScout = TestFactory.HeadScout();
        await personRepo.AddAsync(scout);
        await personRepo.AddAsync(headScout);
        var profile = MakeProfile("GK");
        await profileRepo.AddAsync(profile);

        await svc.AddPlayerToLongListAsync(scout.Id,
            new PersonName("Auto", "Test"), new DateOnly(2000, 1, 1),
            profile.Id, [new PlayerAttribute("position", "GK")]);

        var longList = await playerRepo.GetByListTypeAsync(PlayerListType.LongList.Type);
        Assert.Single(longList);
    }
}
