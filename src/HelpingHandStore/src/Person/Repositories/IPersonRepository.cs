namespace HelpingHandStore.Domain.Person.Repositories;

public interface IPersonRepository
{
    Task<PersonAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PersonAggregate person, CancellationToken cancellationToken = default);
}
