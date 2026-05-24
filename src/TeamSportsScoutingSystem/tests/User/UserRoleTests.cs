using TeamSportsScoutingSystem.Domain.Person;
using TeamSportsScoutingSystem.Domain.Services;
using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using TeamSportsScoutingSystem.Domain.Tests.Helpers;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.User;

public class UserRoleTests
{
    [Fact]
    public void US001_HeadCoachShouldHasHeadCoachRole()
    {
        var p = TestFactory.HeadCoach();
        Assert.True(p.HasRole<HeadCoachRole>());
    }

    [Fact] 
    public void US002_DirectorShouldHasDirectorRole()
    {
        var p = TestFactory.Director();
        Assert.True(p.HasRole<DirectorRole>());
    }

    [Fact] 
    public void US003_ScoutShouldHasScoutRole()
    {
        var p = TestFactory.Scout();
        Assert.True(p.HasRole<ScoutRole>());
    }

    [Fact] 
    public void US004_InvalidRoleShouldNotRecognised()
    {
        var p = TestFactory.NoRole();
        Assert.False(p.HasRole<HeadCoachRole>());
        Assert.False(p.HasRole<DirectorRole>());
        Assert.False(p.HasRole<ScoutRole>());
    }

    [Fact] 
    public void US005_PersonHasNoRolesShouldHaveNullOrEmptyRole()
    {
        var p = new PersonAggregate(new PersonName("No", "Role"));
        Assert.Empty(p.Roles);
    }

    [Fact] 
    public async Task US006_HeadCoachCanCreatePlayerProfile()
    {
        var personRepo = new FakePersonRepository();
        var coach = TestFactory.HeadCoach();
        await personRepo.AddAsync(coach);
        var svc = new PlayerManagementService(new FakePlayerRepository(), new FakePlayerProfileRepository(), personRepo);

        var profile = await svc.CreatePlayerProfileAsync(coach.Id, "GK Profile",
            [new Position("GK")],
            [new PlayerAttribute("speed", "80")]);

        Assert.NotNull(profile);
        Assert.Single(profile.TargetPositions);
    }

    [Fact] 
    public async Task US007_ScoutCannotCreatePlayerProfile()
    {
        var personRepo = new FakePersonRepository();
        var scout = TestFactory.Scout();
        await personRepo.AddAsync(scout);
        var svc = new PlayerManagementService(new FakePlayerRepository(), new FakePlayerProfileRepository(), personRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.CreatePlayerProfileAsync(scout.Id, "LB Profile",
                [new Position("LB")], []));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }

    [Fact]
    public async Task US008_DirectorCannotCreatePlayerProfile()
    {
        var personRepo = new FakePersonRepository();
        var director = TestFactory.Director();
        await personRepo.AddAsync(director);
        var svc = new PlayerManagementService(new FakePlayerRepository(), new FakePlayerProfileRepository(), personRepo);

        var ex = await Assert.ThrowsAsync<DomainException>(() =>
            svc.CreatePlayerProfileAsync(director.Id, "ST Profile",
                [new Position("ST")], []));
        Assert.Contains("tidak memiliki izin", ex.Message);
    }
}
