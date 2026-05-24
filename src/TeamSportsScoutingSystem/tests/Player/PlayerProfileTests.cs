using TeamSportsScoutingSystem.Domain.Services;
using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using TeamSportsScoutingSystem.Domain.Tests.Helpers;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.Player;

public class PlayerProfileTests
{
    private static PlayerManagementService BuildSvc(FakePersonRepository personRepo)
        => new(new FakePlayerRepository(), new FakePlayerProfileRepository(), personRepo);

    [Fact] 
    public async Task PL001_CreateProfileWithoutPositionShouldThrowsError()
    {
        var personRepo = new FakePersonRepository();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            BuildSvc(personRepo).CreatePlayerProfileAsync(coach.Id, "Profile",
                [], []));
        Assert.Contains("posisi wajib diisi", ex.Message);
    }

    [Fact] 
    public async Task PL002_CreateProfileWithMultiplePositionsShouldSucceeds()
    {
        var personRepo = new FakePersonRepository();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);

        var profile = await BuildSvc(personRepo).CreatePlayerProfileAsync(coach.Id, "GK/CB Profile",
            [new Position("GK"), new Position("CB")],
            []);

        Assert.Equal(2, profile.TargetPositions.Count);
    }

    [Fact] 
    public async Task PL003_CreateProfileWithoutAttributesShouldSucceeds()
    {
        var personRepo = new FakePersonRepository();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);

        var profile = await BuildSvc(personRepo).CreatePlayerProfileAsync(coach.Id, "RB Profile",
            [new Position("RB")], []);

        Assert.NotNull(profile);
        Assert.Empty(profile.RequiredAttributes);
    }

    [Fact] 
    public void PL004_PlayerAttributeWithNullValueShouldThrowsError()
    {
        var ex = Assert.Throws<DomainException>(() => new PlayerAttribute("speed", null!));
        Assert.Contains("nilai atribut wajib diisi", ex.Message);
    }

    [Fact] 
    public void PL005_PlayerAttributeWithNullNameShouldThrowsError()
    {
        var ex = Assert.Throws<DomainException>(() => new PlayerAttribute(null!, "80"));
        Assert.Contains("nama atribut wajib diisi", ex.Message);
    }

    [Fact] 
    public async Task PL006_CreateProfileWithManyAttributesShouldSucceeds()
    {
        var personRepo = new FakePersonRepository();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);

        var profile = await BuildSvc(personRepo).CreatePlayerProfileAsync(coach.Id, "CM Profile",
            [new Position("CM")],
            [
                new PlayerAttribute("speed", "75"),
                new PlayerAttribute("passing", "85"),
                new PlayerAttribute("stamina", "90")
            ]);

        Assert.Equal(3, profile.RequiredAttributes.Count);
    }
}
