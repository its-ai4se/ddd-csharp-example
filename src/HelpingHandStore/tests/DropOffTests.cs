using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;
using Xunit;

namespace HelpingHandStore.Domain.Tests;

public class DropOffTests
{
    private static readonly Guid H2SId = Guid.NewGuid();

    private static DateOnly NextWeekday()
    {
        var d = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) d = d.AddDays(1);
        return d;
    }

    private static SecondHandArticle NewArticle() =>
        new(H2SId, new ItemDescription("Chair"), new Dimensions(1), new Weight(5),
            new ScheduledDate(NextWeekday(), new TimeOnly(9, 0)), Guid.NewGuid());

    private static FoodItem NewFood() =>
        new(H2SId, new ItemDescription("Canned beans"), new Dimensions(1), new Weight(1),
            new ScheduledDate(NextWeekday(), new TimeOnly(9, 0)), Guid.NewGuid());

    [Fact]
    public void DO001_AllPickupsComplete_ArticlesDroppedOffAtDistributionCenter()
    {
        var article = NewArticle();
        article.DropOffAtDistributionCenter();
        Assert.True(article.IsAtDistributionCenter);
    }

    [Fact]
    public void DO002_PickupsNotYetComplete_DropOffNotAllowed()
    {
        var article = NewArticle();
        Assert.Throws<DomainException>(() =>
            article.TagWithRfid(new RfidCode("RFID1"), new ItemCategory("Sofa")));
    }

    [Fact]
    public void DO003_FoodItemsCollected_DeliveredDirectlyToFoodBank()
    {
        var food = NewFood();
        food.MarkAsDeliveredToFoodBank();
        Assert.True(food.IsDeliveredToFoodBank);
    }

    [Fact]
    public void DO004_MixedCollection_ArticlesToDistributionCenterFoodToFoodBank()
    {
        var article = NewArticle();
        var food = NewFood();

        article.DropOffAtDistributionCenter();
        food.MarkAsDeliveredToFoodBank();

        Assert.True(article.IsAtDistributionCenter);
        Assert.True(food.IsDeliveredToFoodBank);
        Assert.IsNotType<SecondHandArticle>(food);
    }
}
