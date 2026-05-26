using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace OnlineTutoringSystem.Domain.Tests;

public class UserRoleTests
{
    [Fact] 
    public async Task UR001_TutorCanAlsoRegisterAsStudent()
    {
        var (personSvc, _, _, _, _, _, _, _) = TestFixture.Build();
        var tutor = await personSvc.RegisterTutorAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"),
            new BankAccountNumber("1234567890"));

        var person = await personSvc.RegisterStudentAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"));

        Assert.Same(tutor, person);
        Assert.True(person.HasRole<TutorRole>());
        Assert.True(person.HasRole<StudentRole>());
    }

    [Fact] 
    public async Task UR002_StudentCanAlsoRegisterAsTutor()
    {
        var (personSvc, _, _, _, _, _, _, _) = TestFixture.Build();
        var student = await personSvc.RegisterStudentAsync(
            new PersonName("Alice", "Smith"),
            new EmailAddress("alice@email.com"));

        var person = await personSvc.RegisterTutorAsync(
            new PersonName("Alice", "Smith"),
            new EmailAddress("alice@email.com"),
            new BankAccountNumber("9876543210"));

        Assert.Same(student, person);
        Assert.True(person.HasRole<StudentRole>());
        Assert.True(person.HasRole<TutorRole>());
    }

    [Fact] 
    public async Task UR003_TutorAlsoStudent_CanRequestTutoringFromAnotherTutor()
    {
        var (personSvc, sessionSvc, _, _, _, _, _, _) = TestFixture.Build();

        var john = await personSvc.RegisterTutorAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"),
            new BankAccountNumber("1111111111"));
        await personSvc.RegisterStudentAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"));

        var bob = await personSvc.RegisterTutorAsync(
            new PersonName("Bob", "Brown"),
            new EmailAddress("bob@email.com"),
            new BankAccountNumber("2222222222"));
        var lit = new Subject("Literature");
        bob.GetRole<TutorRole>()!.AddOffer(new TutoringOffer(lit, ExpertiseLevel.Advanced, new Money(60)));

        var request = await sessionSvc.RequestBookingAsync(
            john.Id, bob.Id, lit, ExpertiseLevel.Advanced, DateTime.UtcNow.AddDays(1));

        Assert.NotNull(request);
        Assert.Equal(john.Id, request.StudentId);
        Assert.Equal(bob.Id, request.TutorId);
    }

    [Fact] 
    public async Task UR004_TutorAlsoStudent_CannotRequestTutoringFromSelf()
    {
        var (personSvc, sessionSvc, _, _, _, _, _, _) = TestFixture.Build();

        var john = await personSvc.RegisterTutorAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"),
            new BankAccountNumber("1111111111"));
        await personSvc.RegisterStudentAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"));

        var math = new Subject("Mathematics");
        john.GetRole<TutorRole>()!.AddOffer(new TutoringOffer(math, ExpertiseLevel.Intermediate, new Money(50)));

        await Assert.ThrowsAsync<DomainException>(() =>
            sessionSvc.RequestBookingAsync(john.Id, john.Id, math, ExpertiseLevel.Intermediate, DateTime.UtcNow.AddDays(1)));
    }
}
