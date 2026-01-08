using BusTransportManagementSystem.Domain.Shared.Services;

namespace BusTransportManagementSystem.Tests.TestHelpers;

public class MockClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
