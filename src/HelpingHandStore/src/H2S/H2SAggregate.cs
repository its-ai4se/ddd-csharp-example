using HelpingHandStore.Domain.Shared.Common;

namespace HelpingHandStore.Domain.H2S;

public class H2SAggregate : AggregateRoot
{
    public bool OffersClientDeliveryService { get; private set; }

    public H2SAggregate(Guid id) : base(id)
    {
    }

    public H2SAggregate() : base()
    {
    }

    public void SetClientDeliveryService(bool offered)
    {
        OffersClientDeliveryService = offered;
    }

    public void EnsureOwns(Guid entityH2SId)
    {
        if (entityH2SId != Id)
        {
            throw new DomainException("Entity belongs to a different H2S location.");
        }
    }
}
