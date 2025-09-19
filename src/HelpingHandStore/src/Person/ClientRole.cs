using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Person;

public class ClientRole : UserRole
{
    private readonly List<ItemCategory> _neededCategories = new();

    public ClientRole(Guid id, Guid personId) : base(id, personId)
    {
    }

    public ClientRole(Guid personId) : base(personId)
    {
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

    public void RemoveNeededCategory(ItemCategory category)
    {
        _neededCategories.Remove(category);
    }

    public bool NeedsCategory(ItemCategory category)
    {
        return _neededCategories.Contains(category);
    }

    public void ClearNeededCategories()
    {
        _neededCategories.Clear();
    }
}
