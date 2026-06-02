namespace HelpingHandStore.Domain.H2S.Repositories;

public interface IH2SRepository
{
    Task<H2SAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(H2SAggregate h2s, CancellationToken cancellationToken = default);
}
