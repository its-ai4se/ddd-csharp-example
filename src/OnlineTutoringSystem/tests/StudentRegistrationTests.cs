using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace OnlineTutoringSystem.Domain.Tests;

public class StudentRegistrationTests
{
    [Fact] 
    public async Task SR001_RegisterStudentWithNameAndEmail_Succeeds()
    {
        var (personSvc, _, _, _, _, _, _, _) = TestFixture.Build();
        var student = await personSvc.RegisterStudentAsync(
            new PersonName("Alice", "Smith"),
            new EmailAddress("alice@email.com"));
        Assert.NotNull(student);
        Assert.True(student.HasRole<StudentRole>());
    }

    [Fact] 
    public void SR002_RegisterStudentWithNullName_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new PersonName(null!, "Smith"));
    }

    [Fact] 
    public void SR003_RegisterStudentWithNullEmail_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new EmailAddress(null!));
    }
}
