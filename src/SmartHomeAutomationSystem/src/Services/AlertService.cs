using SmartHomeAutomationSystem.Domain.Home;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Services;

public class AlertService
{
    private static void EnsureOwner(Guid requestingUserId, HomeAggregate home)
    {
        if (!home.IsOwner(requestingUserId))
            throw new DomainException("Only the home owner may configure alerts.");
    }

    public Alert CreateAlert(HomeAggregate home, Guid requestingUserId, string description, Guid deviceId)
    {
        EnsureOwner(requestingUserId, home);
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Alert description cannot be empty.");
        if (deviceId == Guid.Empty)
            throw new DomainException("Device ID cannot be empty.");
        return new Alert(home.Id, deviceId, description.Trim());
    }
}

public class Alert
{
    public Guid HomeId { get; }
    public Guid DeviceId { get; }
    public string Description { get; }

    public Alert(Guid homeId, Guid deviceId, string description)
    {
        HomeId = homeId;
        DeviceId = deviceId;
        Description = description;
    }
}
