using HelpingHandStore.Domain.H2S;

namespace HelpingHandStore.Domain.H2S.Repositories;

public interface IH2SRepository
{
    Task<H2SAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<H2SAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<H2SAggregate?> GetByCityAsync(string city, CancellationToken cancellationToken = default);
    Task AddAsync(H2SAggregate h2s, CancellationToken cancellationToken = default);
    Task UpdateAsync(H2SAggregate h2s, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
