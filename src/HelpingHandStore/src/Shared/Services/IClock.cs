namespace HelpingHandStore.Domain.Shared.Services;

public interface IClock
{
    DateTime Now { get; }
    DateOnly Today { get; }
}

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
