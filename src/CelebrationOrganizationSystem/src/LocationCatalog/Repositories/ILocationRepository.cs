using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.LocationCatalog.Repositories;

public interface ILocationRepository
{
    Task<Location?> GetByNameAsync(string name);
    Task<IEnumerable<Location>> GetPredefinedAsync();
    System.Threading.Tasks.Task AddAsync(Location location);
    Task<bool> ExistsAsync(string name);
}
