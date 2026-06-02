using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Item;

public class SecondHandArticle : ItemAggregate
{
    public RfidCode? RfidCode { get; private set; }
    public ItemCategory? Category { get; private set; }
    public bool IsDiscarded { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsAtDistributionCenter { get; private set; }

    public SecondHandArticle(Guid id, Guid h2sId, ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) 
        : base(id, h2sId, description, dimensions, weight, requestedPickupDate, residentId)
    {
    }

    public SecondHandArticle(Guid h2sId, ItemDescription description, Dimensions dimensions, Weight weight, ScheduledDate requestedPickupDate, Guid residentId) 
        : base(h2sId, description, dimensions, weight, requestedPickupDate, residentId)
    {
    }

    public void DropOffAtDistributionCenter()
    {
        IsAtDistributionCenter = true;
    }

    public void TagWithRfid(RfidCode rfidCode, ItemCategory category)
    {
        ArgumentNullException.ThrowIfNull(rfidCode);

        if (!IsAtDistributionCenter)
        {
            throw new DomainException("Article must be dropped off at the distribution center before it can be tagged.");
        }

        RfidCode = rfidCode;
        Category = category ?? throw new ArgumentNullException(nameof(category));
        IsTagged = true;
    }

    public void Discard()
    {
        if (!IsAtDistributionCenter)
        {
            throw new DomainException("Article must be dropped off at the distribution center before it can be discarded.");
        }

        IsDiscarded = true;
    }

    public bool CanBeDistributed()
    {
        return IsTagged && !IsDiscarded;
    }

    public void UpdateDescription(ItemDescription newDescription)
    {
        Description = newDescription ?? throw new ArgumentNullException(nameof(newDescription));
    }
}
