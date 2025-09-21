using CelebrationOrganizationSystem.Domain.Person;

namespace CelebrationOrganizationSystem.Domain.Person.Repositories;

public interface IPersonRepository
{
    System.Threading.Tasks.Task<PersonAggregate?> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<PersonAggregate?> GetByEmailAsync(string email);
    System.Threading.Tasks.Task<IEnumerable<PersonAggregate>> GetAllAsync();
    System.Threading.Tasks.Task<IEnumerable<PersonAggregate>> GetByRoleAsync<T>() where T : UserRole;
    System.Threading.Tasks.Task AddAsync(PersonAggregate person);
    System.Threading.Tasks.Task UpdateAsync(PersonAggregate person);
    System.Threading.Tasks.Task DeleteAsync(Guid id);
    System.Threading.Tasks.Task<bool> ExistsAsync(Guid id);
    System.Threading.Tasks.Task<bool> ExistsByEmailAsync(string email);
}
