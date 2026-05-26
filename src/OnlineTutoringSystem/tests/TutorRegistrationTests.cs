using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace OnlineTutoringSystem.Domain.Tests;

public class TutorRegistrationTests
{
    [Fact] 
    public async Task TR001_RegisterTutorWithAllFields_Succeeds()
    {
        var (personSvc, _, _, _, _, _, _, _) = TestFixture.Build();
        var tutor = await personSvc.RegisterTutorAsync(
            new PersonName("John", "Doe"),
            new EmailAddress("john@email.com"),
            new BankAccountNumber("1234567890"));
        Assert.NotNull(tutor);
        Assert.True(tutor.HasRole<TutorRole>());
    }

    [Fact] 
    public void TR002_RegisterTutorWithNullName_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new PersonName(null!, "Doe"));
    }

    [Fact] 
    public void TR003_RegisterTutorWithNullEmail_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new EmailAddress(null!));
    }

    [Fact] 
    public void TR004_RegisterTutorWithNullBankAccount_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new BankAccountNumber(null!));
    }

    [Fact] 
    public void TR005_RegisterTutor_AllFieldsNull_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new PersonName(null!, null!));
        Assert.Throws<DomainException>(() => new EmailAddress(null!));
        Assert.Throws<DomainException>(() => new BankAccountNumber(null!));
    }
}
