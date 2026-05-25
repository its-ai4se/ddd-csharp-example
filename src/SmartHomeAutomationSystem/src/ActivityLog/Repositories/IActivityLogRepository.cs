namespace SmartHomeAutomationSystem.Domain.ActivityLog.Repositories;

public interface IActivityLogRepository
{
    Task<ActivityLogAggregate?> GetByHomeIdAsync(Guid homeId);
    Task SaveAsync(ActivityLogAggregate log);
}
