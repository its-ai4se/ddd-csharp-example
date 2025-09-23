namespace TeamSportsScoutingSystem.Domain.Shared.Services;

public interface IClock
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
