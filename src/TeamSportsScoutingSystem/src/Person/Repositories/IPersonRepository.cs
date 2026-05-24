using TeamSportsScoutingSystem.Domain.Person;

namespace TeamSportsScoutingSystem.Domain.Person.Repositories;

public interface IPersonRepository
{
    Task<PersonAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PersonAggregate person, CancellationToken cancellationToken = default);
    Task UpdateAsync(PersonAggregate person, CancellationToken cancellationToken = default);
}
