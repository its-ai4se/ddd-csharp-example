using HelpingHandStore.Domain.Shared.Services;

namespace HelpingHandStore.Domain.Shared.Services;

public abstract class DomainServiceBase
{
    protected readonly IClock Clock;

    protected DomainServiceBase(IClock clock)
    {
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }
}
