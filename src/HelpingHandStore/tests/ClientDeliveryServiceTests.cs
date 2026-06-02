using HelpingHandStore.Domain.H2S;
using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Person;
using HelpingHandStore.Domain.Services;
using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;
using Xunit;

namespace HelpingHandStore.Domain.Tests;

public class ClientDeliveryServiceTests
{
    private static readonly Guid H2SId = Guid.NewGuid();

    private static DateOnly NextWeekday()
    {
        var d = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) d = d.AddDays(1);
        return d;
    }

    private static H2SAggregate LocationWith(bool offersService)
    {
        var h2s = new H2SAggregate(H2SId);
        h2s.SetClientDeliveryService(offersService);
        return h2s;
    }

    private static ClientRole ClientWhoCannotVisit()
    {
        var c = new ClientRole(Guid.NewGuid());
        c.SetCanVisitDistributionCenter(false);
        return c;
    }

    private static ClientRole ClientWhoCanVisit() => new(Guid.NewGuid());

    private static SecondHandArticle TaggedArticle(string category)
    {
        var a = new SecondHandArticle(H2SId, new ItemDescription("Item"), new Dimensions(1),
            new Weight(5), new ScheduledDate(NextWeekday(), new TimeOnly(9, 0)), Guid.NewGuid());
        a.DropOffAtDistributionCenter();
        a.TagWithRfid(new RfidCode("RFID1"), new ItemCategory(category));
        return a;
    }

    [Fact]
    public void CD001_CityOffersServiceAndClientCannotVisit_ClientAllowedToIndicateCategories()
    {
        var h2s = LocationWith(true);
        var client = ClientWhoCannotVisit();
        Assert.True(ClientDeliveryService.IsEligibleForClientDelivery(h2s, client));
    }

    [Fact]
    public void CD002_CityDoesNotOfferService_ServiceNotProvided()
    {
        var h2s = LocationWith(false);
        var client = ClientWhoCannotVisit();
        Assert.False(ClientDeliveryService.IsEligibleForClientDelivery(h2s, client));
    }

    [Fact]
    public void CD003_ClientCannotVisitDistributionCenter_ClientEligibleForService()
    {
        var h2s = LocationWith(true);
        var client = ClientWhoCannotVisit();
        Assert.True(ClientDeliveryService.IsEligibleForClientDelivery(h2s, client));
    }

    [Fact]
    public void CD004_ClientCanVisitDistributionCenter_ClientNotEligibleForService()
    {
        var h2s = LocationWith(true);
        var client = ClientWhoCanVisit();
        Assert.False(ClientDeliveryService.IsEligibleForClientDelivery(h2s, client));
    }

    [Fact]
    public void CD005_EligibleClient_NeededCategoriesStored()
    {
        var client = ClientWhoCannotVisit();
        client.AddNeededCategory(new ItemCategory("Refrigerator"));
        client.AddNeededCategory(new ItemCategory("Microwave"));
        Assert.True(client.NeedsCategory(new ItemCategory("Refrigerator")));
        Assert.True(client.NeedsCategory(new ItemCategory("Microwave")));
    }

    [Fact]
    public void CD006_RelevantArticleDroppedOff_EmployeeNotifiesClient()
    {
        var h2s = LocationWith(true);
        var client = ClientWhoCannotVisit();
        client.AddNeededCategory(new ItemCategory("Refrigerator"));

        var articles = new[] { TaggedArticle("Refrigerator"), TaggedArticle("Sofa") };
        var relevant = ClientDeliveryService.GetRelevantArticlesForClient(h2s, client, articles).ToList();

        Assert.Single(relevant);
        Assert.Equal("Refrigerator", relevant[0].Category!.Name);
    }

    [Fact]
    public void CD007_NoRelevantArticleDroppedOff_NoCallMadeToClient()
    {
        var h2s = LocationWith(true);
        var client = ClientWhoCannotVisit();
        client.AddNeededCategory(new ItemCategory("Refrigerator"));

        var articles = new[] { TaggedArticle("Sofa") };
        var relevant = ClientDeliveryService.GetRelevantArticlesForClient(h2s, client, articles);

        Assert.Empty(relevant);
    }

    [Fact]
    public void CD008_ClientStillNeedsArticle_DeliveryScheduledToHomeAddress()
    {
        var day = NextWeekday();
        var volunteer = new VolunteerRole(Guid.NewGuid());
        volunteer.AddAvailableDay(day);
        var vehicle = new Vehicle.VehicleAggregate(H2SId, new Dimensions(10), new Weight(1000));
        var route = RoutePlanningService.CreatePickupRoute(vehicle, volunteer, day);

        var article = TaggedArticle("Refrigerator");
        ClientDeliveryService.ArrangeDelivery(route, article);

        Assert.Contains(article.Id, route.DeliveryItemIds);
    }

    [Fact]
    public void CD009_ClientNoLongerNeedsArticle_NoDeliveryScheduled()
    {
        var h2s = LocationWith(true);
        var client = ClientWhoCannotVisit();
        var articles = new[] { TaggedArticle("Refrigerator") };
        var relevant = ClientDeliveryService.GetRelevantArticlesForClient(h2s, client, articles);
        Assert.Empty(relevant);
    }

    [Fact]
    public void CD010_DeliveriesAndPickupsSameDay_DeliveriesCompletedBeforePickupsStart()
    {
        var day = NextWeekday();
        var volunteer = new VolunteerRole(Guid.NewGuid());
        volunteer.AddAvailableDay(day);
        var vehicle = new Vehicle.VehicleAggregate(H2SId, new Dimensions(10), new Weight(1000));
        var route = RoutePlanningService.CreatePickupRoute(vehicle, volunteer, day);

        var article = TaggedArticle("Refrigerator");
        ClientDeliveryService.ArrangeDelivery(route, article);

        Assert.Throws<DomainException>(() => route.StartPickups());

        route.CompleteDeliveries();
        route.StartPickups();
        Assert.True(route.PickupsStarted);
    }
}
