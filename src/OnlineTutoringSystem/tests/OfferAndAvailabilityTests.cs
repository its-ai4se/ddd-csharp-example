using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace OnlineTutoringSystem.Domain.Tests;

public class OfferAndAvailabilityTests
{
    private static TutorRole MakeTutor()
        => new(Guid.NewGuid(), new BankAccountNumber("1234567890"));

    [Fact] 
    public void OA001_TutorAddOfferWithAllFields_Succeeds()
    {
        var tutor = MakeTutor();
        tutor.AddOffer(new TutoringOffer(new Subject("Mathematics"), ExpertiseLevel.Intermediate, new Money(50000)));
        Assert.Single(tutor.Offers);
    }

    [Fact] 
    public void OA002_TutorAddMultipleOffersDifferentSubjects_Succeeds()
    {
        var tutor = MakeTutor();
        tutor.AddOffer(new TutoringOffer(new Subject("Math"), ExpertiseLevel.Intermediate, new Money(50000)));
        tutor.AddOffer(new TutoringOffer(new Subject("Science"), ExpertiseLevel.Advanced, new Money(75000)));
        Assert.Equal(2, tutor.Offers.Count);
    }

    [Fact] 
    public void OA003_TutorDifferentSubjectsHaveDifferentPrices_Succeeds()
    {
        var tutor = MakeTutor();
        tutor.AddOffer(new TutoringOffer(new Subject("Math"), ExpertiseLevel.Intermediate, new Money(50000)));
        tutor.AddOffer(new TutoringOffer(new Subject("Science"), ExpertiseLevel.Intermediate, new Money(80000)));
        Assert.Equal(50000m, tutor.Offers[0].HourlyPrice.Amount);
        Assert.Equal(80000m, tutor.Offers[1].HourlyPrice.Amount);
    }

    [Fact] 
    public void OA004_AddOfferWithNullSubject_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new TutoringOffer(null!, ExpertiseLevel.Intermediate, new Money(50000)));
    }

    [Fact] 
    public void OA005_AddOfferWithNullLevel_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new TutoringOffer(new Subject("Math"), null!, new Money(50000)));
    }

    [Fact] 
    public void OA006_AddOfferWithZeroPrice_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new TutoringOffer(new Subject("Math"), ExpertiseLevel.Intermediate, new Money(0)));
    }

    [Fact] 
    public async Task OA007_NonTutor_CannotCreateOffer()
    {
        var (personSvc, _, _, _, _, _, _, _) = TestFixture.Build();
        var alice = await personSvc.RegisterStudentAsync(
            new PersonName("Alice", "Smith"),
            new EmailAddress("alice@email.com"));
        Assert.False(alice.HasRole<TutorRole>());
        Assert.Null(alice.GetRole<TutorRole>());
    }

    [Fact] 
    public void OA008_Tutor_AddAvailabilitySlot_Succeeds()
    {
        var tutor = MakeTutor();
        tutor.AddAvailabilitySlot(new AvailabilitySlot(DayOfWeek.Thursday, new TimeOnly(10, 0), new TimeOnly(11, 30)));
        Assert.Single(tutor.Availability);
    }

    [Fact] 
    public void OA009_Tutor_AddMultipleSlots_DifferentDays_Succeeds()
    {
        var tutor = MakeTutor();
        tutor.AddAvailabilitySlot(new AvailabilitySlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0)));
        tutor.AddAvailabilitySlot(new AvailabilitySlot(DayOfWeek.Thursday, new TimeOnly(10, 0), new TimeOnly(11, 30)));
        Assert.Equal(2, tutor.Availability.Count);
    }

    [Fact] 
    public void OA010_Tutor_AddMultipleSlots_SameDay_NonOverlapping_Succeeds()
    {
        var tutor = MakeTutor();
        tutor.AddAvailabilitySlot(new AvailabilitySlot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0)));
        tutor.AddAvailabilitySlot(new AvailabilitySlot(DayOfWeek.Monday, new TimeOnly(14, 0), new TimeOnly(15, 0)));
        Assert.Equal(2, tutor.Availability.Count);
    }

    [Fact] 
    public void OA011_AddAvailabilitySlot_StartEqualsEnd_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AvailabilitySlot(DayOfWeek.Thursday, new TimeOnly(10, 0), new TimeOnly(10, 0)));
    }

    [Fact] 
    public void OA012_AddAvailabilitySlot_StartAfterEnd_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AvailabilitySlot(DayOfWeek.Thursday, new TimeOnly(11, 30), new TimeOnly(10, 0)));
    }
}
