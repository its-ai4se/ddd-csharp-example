using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Shared.Services;

public interface IClock
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
}