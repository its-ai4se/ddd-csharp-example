using TeamSportsScoutingSystem.Domain.Person;
using TeamSportsScoutingSystem.Domain.Person.Repositories;

namespace TeamSportsScoutingSystem.Domain.Services;

public class PersonManagementService
{
    private readonly IPersonRepository _personRepository;

    public PersonManagementService(IPersonRepository personRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task RegisterHeadCoachAsync(PersonAggregate person, CancellationToken cancellationToken = default)
    {
        if (person.HasRole<HeadCoachRole>())
        {
            throw new InvalidOperationException("Person is already registered as a head coach.");
        }

        var headCoachRole = new HeadCoachRole(person.Id);
        person.AddRole(headCoachRole);
        
        await _personRepository.UpdateAsync(person, cancellationToken);
    }

    public async Task RegisterDirectorAsync(PersonAggregate person, CancellationToken cancellationToken = default)
    {
        if (person.HasRole<DirectorRole>())
        {
            throw new InvalidOperationException("Person is already registered as a director.");
        }

        var directorRole = new DirectorRole(person.Id);
        person.AddRole(directorRole);
        
        await _personRepository.UpdateAsync(person, cancellationToken);
    }

    public async Task RegisterScoutAsync(PersonAggregate person, bool isHeadScout = false, CancellationToken cancellationToken = default)
    {
        if (person.HasRole<ScoutRole>())
        {
            throw new InvalidOperationException("Person is already registered as a scout.");
        }

        var scoutRole = new ScoutRole(person.Id, isHeadScout);
        person.AddRole(scoutRole);
        
        await _personRepository.UpdateAsync(person, cancellationToken);
    }

    public async Task PromoteScoutToHeadScoutAsync(PersonAggregate person, CancellationToken cancellationToken = default)
    {
        var scoutRole = person.GetRole<ScoutRole>();
        if (scoutRole == null)
        {
            throw new InvalidOperationException("Person is not registered as a scout.");
        }

        if (scoutRole.IsHeadScout)
        {
            throw new InvalidOperationException("Person is already a head scout.");
        }

        scoutRole.PromoteToHeadScout();
        
        await _personRepository.UpdateAsync(person, cancellationToken);
    }

}
