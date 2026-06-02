using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Person;
using HelpingHandStore.Domain.Services;
using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;
using HelpingHandStore.Domain.Vehicle;
using Xunit;

namespace HelpingHandStore.Domain.Tests;

public class RoutingTests
{
    private static readonly Guid H2SId = Guid.NewGuid();

    private static DateOnly NextWeekday()
    {
        var d = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) d = d.AddDays(1);
        return d;
    }

    private static VehicleAggregate Vehicle(decimal volume = 10, decimal maxWeight = 1000) =>
        new(H2SId, new Dimensions(volume), new Weight(maxWeight));

    private static VolunteerRole AvailableVolunteer(DateOnly date)
    {
        var v = new VolunteerRole(Guid.NewGuid());
        v.AddAvailableDay(date);
        return v;
    }

    private static SecondHandArticle Article(decimal volume, decimal weight)
    {
        var date = new ScheduledDate(NextWeekday(), new TimeOnly(9, 0));
        return new SecondHandArticle(H2SId, new ItemDescription("Item"),
            new Dimensions(volume), new Weight(weight), date, Guid.NewGuid());
    }

    [Fact]
    public void RT001_ThreeVehiclesWithAvailableDrivers_ThreeRoutesCreated()
    {
        var day = NextWeekday();
        var routes = Enumerable.Range(0, 3)
            .Select(_ => RoutePlanningService.CreatePickupRoute(Vehicle(), AvailableVolunteer(day), day))
            .ToList();

        Assert.Equal(3, routes.Count);
    }

    [Fact]
    public void RT002_VehicleWithoutAvailableDriver_NoRouteCreated()
    {
        var day = NextWeekday();
        var noDriver = new VolunteerRole(Guid.NewGuid());

        Assert.Throws<DomainException>(() =>
            RoutePlanningService.CreatePickupRoute(Vehicle(), noDriver, day));
    }

    [Fact]
    public void RT003_TotalItemsExceedCapacity_ItemNotAddedToRoute()
    {
        var vehicle = Vehicle(volume: 10);
        var existing = new List<ItemAggregate> { Article(9, 10) };
        var newItem = Article(3, 10);

        Assert.False(RoutePlanningService.CanAccommodateItemInRoute(vehicle, newItem, existing));
    }

    [Fact]
    public void RT004_ItemExceedsRemainingCapacity_ItemNotAddedToRoute()
    {
        var vehicle = Vehicle(volume: 10, maxWeight: 50);
        var existing = new List<ItemAggregate> { Article(5, 30) };
        var newItem = Article(4, 30);

        Assert.False(RoutePlanningService.CanAccommodateItemInRoute(vehicle, newItem, existing));
    }

    [Fact]
    public void RT005_TotalItemsExactlyEqualCapacity_AllItemsAddedWithoutViolation()
    {
        var vehicle = Vehicle(volume: 10, maxWeight: 100);
        var existing = new List<ItemAggregate> { Article(6, 50) };
        var newItem = Article(4, 50);

        Assert.True(RoutePlanningService.CanAccommodateItemInRoute(vehicle, newItem, existing));
    }

    [Fact]
    public void RT006_VolunteerRecordsAvailableDays_DaysStoredAndUsedForRouting()
    {
        var day = NextWeekday();
        var volunteer = new VolunteerRole(Guid.NewGuid());
        volunteer.AddAvailableDay(day);

        Assert.True(volunteer.IsAvailableOn(day));
        var route = RoutePlanningService.CreatePickupRoute(Vehicle(), volunteer, day);
        Assert.Equal(volunteer.PersonId, route.VolunteerId);
    }
}
