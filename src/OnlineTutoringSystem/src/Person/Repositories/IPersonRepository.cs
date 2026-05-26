namespace OnlineTutoringSystem.Domain.Person.Repositories;

public interface IPersonRepository
{
    Task<PersonAggregate?> GetByIdAsync(Guid id);
    Task<PersonAggregate?> GetByEmailAsync(string email);
    Task SaveAsync(PersonAggregate person);
}
