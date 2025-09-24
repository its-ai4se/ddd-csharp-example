namespace SmartHomeAutomationSystem.Domain.Shared.Services;

public interface IClock
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
