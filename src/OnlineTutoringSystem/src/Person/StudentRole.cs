using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Person;

public class StudentRole : UserRole
{
    public List<Subject> InterestedSubjects { get; private set; }
    public string LearningGoals { get; private set; }
    public string PreferredLearningStyle { get; private set; }

    public StudentRole(Guid id, Guid personId, List<Subject> interestedSubjects, string learningGoals = "", string preferredLearningStyle = "") : base(id, personId)
    {
        InterestedSubjects = interestedSubjects ?? throw new ArgumentNullException(nameof(interestedSubjects));
        LearningGoals = learningGoals ?? "";
        PreferredLearningStyle = preferredLearningStyle ?? "";
    }

    public StudentRole(Guid personId, List<Subject> interestedSubjects, string learningGoals = "", string preferredLearningStyle = "") : base(personId)
    {
        InterestedSubjects = interestedSubjects ?? throw new ArgumentNullException(nameof(interestedSubjects));
        LearningGoals = learningGoals ?? "";
        PreferredLearningStyle = preferredLearningStyle ?? "";
    }

    public void UpdateInterestedSubjects(List<Subject> newSubjects)
    {
        InterestedSubjects = newSubjects ?? throw new ArgumentNullException(nameof(newSubjects));
    }

    public void UpdateLearningGoals(string newGoals)
    {
        LearningGoals = newGoals ?? "";
    }

    public void UpdatePreferredLearningStyle(string newStyle)
    {
        PreferredLearningStyle = newStyle ?? "";
    }

    public bool IsInterestedInSubject(Subject subject)
    {
        return InterestedSubjects.Contains(subject);
    }
}
