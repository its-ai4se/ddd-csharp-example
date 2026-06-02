using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Person;

public class ClientRole : UserRole
{
    private readonly List<ItemCategory> _neededCategories = new();

    public bool CanVisitDistributionCenter { get; private set; } = true;

    public ClientRole(Guid id, Guid personId) : base(id, personId)
    {
    }

    public ClientRole(Guid personId) : base(personId)
    {
    }

    public void SetCanVisitDistributionCenter(bool canVisit)
    {
        CanVisitDistributionCenter = canVisit;
    }

    public IReadOnlyList<ItemCategory> NeededCategories => _neededCategories.AsReadOnly();

    public void AddNeededCategory(ItemCategory category)
    {
        if (_neededCategories.Contains(category))
        {
            return; // Already needs this category
        }

        _neededCategories.Add(category);
    }

    public bool NeedsCategory(ItemCategory category)
    {
        return _neededCategories.Contains(category);
    }
}
