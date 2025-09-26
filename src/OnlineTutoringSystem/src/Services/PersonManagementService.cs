using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Person.Repositories;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.Services;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Services;

public class PersonManagementService : DomainServiceBase
{
    private readonly IPersonRepository _personRepository;

    public PersonManagementService(IClock clock, IPersonRepository personRepository) : base(clock)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<PersonAggregate> RegisterPersonAsync(PersonName name, EmailAddress email, DateTime dateOfBirth, PhoneNumber? phoneNumber = null)
    {
        // Check if person already exists with this email
        var existingPerson = await _personRepository.GetByEmailAsync(email.Value);
        if (existingPerson != null)
            throw new DomainException("A person with this email address already exists.");

        var person = new PersonAggregate(name, email, dateOfBirth, phoneNumber);
        await _personRepository.SaveAsync(person);
        return person;
    }

    public async Task RegisterTutorAsync(Guid personId, List<Subject> subjects, Money hourlyRate, string bio = "")
    {
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
            throw new DomainException("Person not found.");

        if (person.HasRole<TutorRole>())
            throw new DomainException("Person is already registered as a tutor.");

        var tutorRole = new TutorRole(personId, subjects, hourlyRate, bio);
        person.AddRole(tutorRole);
        await _personRepository.SaveAsync(person);
    }

    public async Task RegisterStudentAsync(Guid personId, List<Subject> interestedSubjects, string learningGoals = "", string preferredLearningStyle = "")
    {
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
            throw new DomainException("Person not found.");

        if (person.HasRole<StudentRole>())
            throw new DomainException("Person is already registered as a student.");

        var studentRole = new StudentRole(personId, interestedSubjects, learningGoals, preferredLearningStyle);
        person.AddRole(studentRole);
        await _personRepository.SaveAsync(person);
    }

    public async Task VerifyTutorAsync(Guid personId)
    {
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
            throw new DomainException("Person not found.");

        var tutorRole = person.GetRole<TutorRole>();
        if (tutorRole == null)
            throw new DomainException("Person is not registered as a tutor.");

        tutorRole.Verify();
        await _personRepository.SaveAsync(person);
    }

    public async Task<List<PersonAggregate>> GetVerifiedTutorsAsync()
    {
        var tutors = await _personRepository.GetByRoleAsync<TutorRole>();
        return tutors.Where(t => t.GetRole<TutorRole>()?.IsVerified == true).ToList();
    }

    public async Task<List<PersonAggregate>> GetTutorsBySubjectAsync(Subject subject)
    {
        var tutors = await _personRepository.GetByRoleAsync<TutorRole>();
        return tutors.Where(t => t.GetRole<TutorRole>()?.CanTeachSubject(subject) == true).ToList();
    }
}
