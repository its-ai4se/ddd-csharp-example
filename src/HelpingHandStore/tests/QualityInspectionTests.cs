using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Person;
using HelpingHandStore.Domain.Services;
using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;
using Xunit;

namespace HelpingHandStore.Domain.Tests;

public class QualityInspectionTests
{
    private static readonly Guid H2SId = Guid.NewGuid();

    private static DateOnly NextWeekday()
    {
        var d = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) d = d.AddDays(1);
        return d;
    }

    private static SecondHandArticle DroppedOffArticle()
    {
        var a = new SecondHandArticle(H2SId, new ItemDescription("Chair"), new Dimensions(1),
            new Weight(5), new ScheduledDate(NextWeekday(), new TimeOnly(9, 0)), Guid.NewGuid());
        a.DropOffAtDistributionCenter();
        return a;
    }

    private static EmployeeRole Employee() => new(Guid.NewGuid());

    [Fact]
    public void QI001_ArticleReceivedAtDistributionCenter_QualityCheckStatusRecorded()
    {
        var article = DroppedOffArticle();
        ItemProcessingService.ProcessSecondHandArticle(Employee(), article, new ItemCategory("Sofa"), isUsable: true);
        Assert.True(article.IsTagged);
    }

    [Fact]
    public void QI002_ArticleStillUsable_ArticleTaggedWithRfid()
    {
        var article = DroppedOffArticle();
        ItemProcessingService.ProcessSecondHandArticle(Employee(), article, new ItemCategory("Sofa"), isUsable: true);
        Assert.NotNull(article.RfidCode);
        Assert.True(article.CanBeDistributed());
    }

    [Fact]
    public void QI003_ArticleNotUsable_ArticleDiscardedNotTagged()
    {
        var article = DroppedOffArticle();
        ItemProcessingService.ProcessSecondHandArticle(Employee(), article, new ItemCategory("Sofa"), isUsable: false);
        Assert.True(article.IsDiscarded);
        Assert.False(article.CanBeDistributed());
    }

    [Fact]
    public void QI004_EmployeeCorrectsDescription_DescriptionUpdatedSuccessfully()
    {
        var article = DroppedOffArticle();
        ItemProcessingService.CorrectDescription(Employee(), article, new ItemDescription("kursi kayu rusak ringan"));
        Assert.Equal("kursi kayu rusak ringan", article.Description.Description);
    }
}