using HelpingHandStore.Domain.H2S;
using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Person;
using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;
using Xunit;

namespace HelpingHandStore.Domain.Tests;

public class PickupRequestTests
{
    private static DateOnly NextWeekday()
    {
        var d = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        while (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) d = d.AddDays(1);
        return d;
    }

    private static DateOnly NextSaturday()
    {
        var d = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        while (d.DayOfWeek != DayOfWeek.Saturday) d = d.AddDays(1);
        return d;
    }

    private static readonly Guid H2SId = Guid.NewGuid();
    private static readonly ScheduledDate ValidDate = new(NextWeekday(), new TimeOnly(9, 0));
    private static readonly Dimensions Dims = new(1);
    private static readonly Weight Wt = new(5);

    [Fact]
    public void PR001_AllRequiredFieldsProvided_PickupCreatedSuccessfully()
    {
        var person = new PersonAggregate(H2SId,
            new PersonName("Andi"), new Address("Jl. Merdeka 10"), new PhoneNumber("08123456789"));
        var article = new SecondHandArticle(H2SId,
            new ItemDescription("1 kulkas bekas"), Dims, Wt, ValidDate, person.Id);

        Assert.NotNull(article);
        Assert.Equal("1 kulkas bekas", article.Description.Description);
    }

    [Fact]
    public void PR002_EmptyName_PickupRejectedWithError()
    {
        Assert.Throws<ArgumentException>(() => new PersonName(""));
    }

    [Fact]
    public void PR003_EmptyAddress_PickupRejectedWithError()
    {
        Assert.Throws<ArgumentException>(() => new Address(""));
    }

    [Fact]
    public void PR004_EmptyPhone_PickupRejectedWithError()
    {
        Assert.Throws<ArgumentException>(() => new PhoneNumber(""));
    }

    [Fact]
    public void PR005_EmptyDescription_PickupRejectedWithError()
    {
        Assert.Throws<ArgumentException>(() => new ItemDescription(""));
    }

    [Fact]
    public void PR006_NoEmail_PickupCreatedWithoutEmail()
    {
        var person = new PersonAggregate(H2SId,
            new PersonName("Andi"), new Address("Jl. Merdeka 10"), new PhoneNumber("08123456789"));
        Assert.Null(person.EmailAddress);
    }

    [Fact]
    public void PR007_ValidEmail_PickupCreatedWithEmailStored()
    {
        var person = new PersonAggregate(H2SId,
            new PersonName("Andi"), new Address("Jl. Merdeka 10"), new PhoneNumber("08123456789"),
            new EmailAddress("andi@mail.com"));
        Assert.NotNull(person.EmailAddress);
        Assert.Equal("andi@mail.com", person.EmailAddress.Value);
    }

    [Fact]
    public void PR008_WeekendDate_PickupRejectedAsNotWeekday()
    {
        Assert.Throws<DomainException>(() =>
            new ScheduledDate(NextSaturday(), new TimeOnly(9, 0)));
    }

    [Fact]
    public void PR009_WeekdayDate_PickupScheduledSuccessfully()
    {
        var sd = new ScheduledDate(NextWeekday(), new TimeOnly(9, 0));
        Assert.Equal(NextWeekday(), sd.Date);
    }

    [Fact]
    public void PR010_ScheduledPickup_AddressRecordedOnScheduledItem()
    {
        var person = new PersonAggregate(H2SId,
            new PersonName("Andi"), new Address("Jl. Merdeka 10"), new PhoneNumber("08123456789"));
        var article = new SecondHandArticle(H2SId,
            new ItemDescription("Kulkas"), Dims, Wt, ValidDate, person.Id);

        Assert.Equal("Jl. Merdeka 10", person.Address.StreetAddress);
        Assert.Equal(person.Id, article.ResidentId);
    }

    [Fact]
    public void PR011_PickupTimeAt0800_TimeAcceptedAsValid()
    {
        var sd = new ScheduledDate(NextWeekday(), new TimeOnly(8, 0));
        Assert.Equal(new TimeOnly(8, 0), sd.PickupTime);
    }

    [Fact]
    public void PR012_PickupTimeAt1400_TimeAcceptedAsValid()
    {
        var sd = new ScheduledDate(NextWeekday(), new TimeOnly(14, 0));
        Assert.Equal(new TimeOnly(14, 0), sd.PickupTime);
    }

    [Fact]
    public void PR013_PickupTimeBefore0800_TimeRejectedOutsideRange()
    {
        Assert.Throws<DomainException>(() =>
            new ScheduledDate(NextWeekday(), new TimeOnly(7, 59)));
    }

    [Fact]
    public void PR014_PickupTimeAfter1400_TimeRejectedOutsideRange()
    {
        Assert.Throws<DomainException>(() =>
            new ScheduledDate(NextWeekday(), new TimeOnly(14, 1)));
    }
}
