using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Item.Repositories;

public interface IItemRepository
{
    Task<ItemAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemAggregate>> GetByResidentIdAsync(Guid residentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SecondHandArticle>> GetSecondHandArticlesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FoodItem>> GetFoodItemsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SecondHandArticle>> GetByCategoryAsync(ItemCategory category, CancellationToken cancellationToken = default);
    Task AddAsync(ItemAggregate item, CancellationToken cancellationToken = default);
    Task UpdateAsync(ItemAggregate item, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
