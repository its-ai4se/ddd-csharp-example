using HelpingHandStore.Domain.H2S;
using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;
using Xunit;

namespace HelpingHandStore.Domain.Tests;

public class OrganizationTests
{
    private static DateOnly NextWeekday()
    {
        var d = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) d = d.AddDays(1);
        return d;
    }

    [Fact]
    public void OG001_LocationIndependence_LocationBRejectsLocationAEntity()
    {
        var locationA = new H2SAggregate();
        var locationB = new H2SAggregate();

        var article = new SecondHandArticle(locationA.Id,
            new ItemDescription("Chair"), new Dimensions(1), new Weight(5),
            new ScheduledDate(NextWeekday(), new TimeOnly(9, 0)), Guid.NewGuid());

        Assert.Throws<DomainException>(() => locationB.EnsureOwns(article.H2SId));
    }
}