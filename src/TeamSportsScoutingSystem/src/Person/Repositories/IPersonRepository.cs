using TeamSportsScoutingSystem.Domain.Person;

namespace TeamSportsScoutingSystem.Domain.Person.Repositories;

public interface IPersonRepository
{
    Task<PersonAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PersonAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PersonAggregate>> GetByRoleAsync<T>(CancellationToken cancellationToken = default) where T : UserRole;
    Task<PersonAggregate?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(PersonAggregate person, CancellationToken cancellationToken = default);
    Task UpdateAsync(PersonAggregate person, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
