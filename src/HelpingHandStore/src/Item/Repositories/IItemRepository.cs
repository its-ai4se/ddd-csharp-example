namespace HelpingHandStore.Domain.Item.Repositories;

public interface IItemRepository
{
    Task<ItemAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ItemAggregate item, CancellationToken cancellationToken = default);
}
