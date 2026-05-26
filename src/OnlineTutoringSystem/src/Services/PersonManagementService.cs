using OnlineTutoringSystem.Domain.Person;
using OnlineTutoringSystem.Domain.Person.Repositories;
using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Services;

public class PersonManagementService
{
    private readonly IPersonRepository _personRepository;

    public PersonManagementService(IPersonRepository personRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<PersonAggregate> RegisterStudentAsync(PersonName name, EmailAddress email)
    {
        var existing = await _personRepository.GetByEmailAsync(email.Value);
        if (existing != null)
        {
            // BR-001: a tutor may also be a student — add student role to existing person
            if (existing.HasRole<StudentRole>())
                throw new DomainException("This person is already registered as a student.");
            existing.AddRole(new StudentRole(existing.Id));
            await _personRepository.SaveAsync(existing);
            return existing;
        }

        var person = new PersonAggregate(name, email);
        person.AddRole(new StudentRole(person.Id));
        await _personRepository.SaveAsync(person);
        return person;
    }

    public async Task<PersonAggregate> RegisterTutorAsync(PersonName name, EmailAddress email, BankAccountNumber bankAccountNumber)
    {
        var existing = await _personRepository.GetByEmailAsync(email.Value);
        if (existing != null)
        {
            // BR-001: a student may also register as a tutor
            if (existing.HasRole<TutorRole>())
                throw new DomainException("This person is already registered as a tutor.");
            existing.AddRole(new TutorRole(existing.Id, bankAccountNumber));
            await _personRepository.SaveAsync(existing);
            return existing;
        }

        var person = new PersonAggregate(name, email);
        person.AddRole(new TutorRole(person.Id, bankAccountNumber));
        await _personRepository.SaveAsync(person);
        return person;
    }
}
