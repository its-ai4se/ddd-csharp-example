using CelebrationOrganizationSystem.Domain.Person;

namespace CelebrationOrganizationSystem.Domain.Person.Repositories;

public interface IPersonRepository
{
    Task<PersonAggregate?> GetByIdAsync(Guid id);
    Task<PersonAggregate?> GetByEmailAsync(string email);
    System.Threading.Tasks.Task AddAsync(PersonAggregate person);
    System.Threading.Tasks.Task UpdateAsync(PersonAggregate person);
    Task<bool> ExistsByEmailAsync(string email);
}
