using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Item;

public class SecondHandArticle : ItemAggregate
{
    public RfidCode? RfidCode { get; private set; }
    public ItemCategory Category { get; private set; }
    public bool IsDiscarded { get; private set; }
    public bool IsTagged { get; private set; }

    public SecondHandArticle(Guid id, ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) 
        : base(id, description, dimensions, weight, requestedPickupDate, residentId)
    {
        IsDiscarded = false;
        IsTagged = false;
    }

    public SecondHandArticle(ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) 
        : base(description, dimensions, weight, requestedPickupDate, residentId)
    {
        IsDiscarded = false;
        IsTagged = false;
    }

    public void TagWithRfid(RfidCode rfidCode, ItemCategory category)
    {
        if (rfidCode == null)
        {
            throw new ArgumentNullException(nameof(rfidCode));
        }

        RfidCode = rfidCode;
        Category = category;
        IsTagged = true;
        IsDiscarded = false;
    }

    public void Discard()
    {
        IsDiscarded = true;
        IsTagged = false;
        RfidCode = null;
    }

    public void UpdateCategory(ItemCategory newCategory)
    {
        Category = newCategory;
    }

    public bool CanBeDistributed()
    {
        return IsTagged && !IsDiscarded;
    }

    public override string ToString() => $"SecondHandArticle: {Description} (Category: {Category}, Tagged: {IsTagged})";
}
