using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class ItemCategory : ValueObject
{
    public string Name { get; }

    public ItemCategory(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Category name cannot be empty.");
        }

        var trimmed = name.Trim();
        if (!CategoryCatalog.Contains(trimmed))
        {
            throw new DomainException($"'{trimmed}' is not in the standard list of categories.");
        }

        Name = CategoryCatalog.Canonical(trimmed);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }

    public override string ToString() => Name;
}
