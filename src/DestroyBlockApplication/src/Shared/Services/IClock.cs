namespace DestroyBlockApplication.Domain.Shared.Services;

public interface IClock
{
    DateTime Now { get; }
}
