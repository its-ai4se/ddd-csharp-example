using OnlineTutoringSystem.Domain.Person;

namespace OnlineTutoringSystem.Domain.Person.Repositories;

public interface IPersonRepository
{
    Task<PersonAggregate?> GetByIdAsync(Guid id);
    Task<PersonAggregate?> GetByEmailAsync(string email);
    Task<IEnumerable<PersonAggregate>> GetAllAsync();
    Task<IEnumerable<PersonAggregate>> GetByRoleAsync<T>() where T : UserRole;
    Task SaveAsync(PersonAggregate person);
    Task DeleteAsync(Guid id);
}
