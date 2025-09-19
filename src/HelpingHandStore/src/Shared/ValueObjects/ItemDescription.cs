using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.Shared.ValueObjects;

public class ItemDescription : ValueObject
{
    public string Description { get; }

    public ItemDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Item description cannot be empty or whitespace.", nameof(description));
        }

        if (description.Length > 500)
        {
            throw new ArgumentException("Item description cannot exceed 500 characters.", nameof(description));
        }

        Description = description.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Description;
    }

    public override string ToString() => Description;
}
