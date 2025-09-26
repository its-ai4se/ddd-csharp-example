using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Person;

public class TutorRole : UserRole
{
    public List<Subject> Subjects { get; private set; }
    public Money HourlyRate { get; private set; }
    public string Bio { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }

    public TutorRole(Guid id, Guid personId, List<Subject> subjects, Money hourlyRate, string bio = "") : base(id, personId)
    {
        Subjects = subjects ?? throw new ArgumentNullException(nameof(subjects));
        HourlyRate = hourlyRate ?? throw new ArgumentNullException(nameof(hourlyRate));
        Bio = bio ?? "";
        IsVerified = false;
    }

    public TutorRole(Guid personId, List<Subject> subjects, Money hourlyRate, string bio = "") : base(personId)
    {
        Subjects = subjects ?? throw new ArgumentNullException(nameof(subjects));
        HourlyRate = hourlyRate ?? throw new ArgumentNullException(nameof(hourlyRate));
        Bio = bio ?? "";
        IsVerified = false;
    }

    public void UpdateSubjects(List<Subject> newSubjects)
    {
        Subjects = newSubjects ?? throw new ArgumentNullException(nameof(newSubjects));
    }

    public void UpdateHourlyRate(Money newRate)
    {
        HourlyRate = newRate ?? throw new ArgumentNullException(nameof(newRate));
    }

    public void UpdateBio(string newBio)
    {
        Bio = newBio ?? "";
    }

    public void Verify()
    {
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
    }

    public void Unverify()
    {
        IsVerified = false;
        VerifiedAt = null;
    }

    public bool CanTeachSubject(Subject subject)
    {
        return Subjects.Contains(subject);
    }
}
