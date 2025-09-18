using BusTransportManagementSystem.Domain.Shared.Services;

namespace BusTransportManagementSystem.Domain.Shared.Services;

public abstract class DomainServiceBase
{
    protected readonly IClock Clock;

    protected DomainServiceBase(IClock clock)
    {
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }
}
