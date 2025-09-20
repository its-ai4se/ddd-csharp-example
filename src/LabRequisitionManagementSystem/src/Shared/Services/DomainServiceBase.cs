using LabRequisitionManagementSystem.Domain.Shared.Services;

namespace LabRequisitionManagementSystem.Domain.Shared.Services;

public abstract class DomainServiceBase
{
    protected readonly IClock Clock;

    protected DomainServiceBase(IClock clock)
    {
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }
}
