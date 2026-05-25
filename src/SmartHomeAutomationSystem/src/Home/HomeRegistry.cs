using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Home;

/// <summary>
/// Enforces unique physical addresses across smart homes (BR-016).
/// </summary>
public class HomeRegistry
{
    private readonly Dictionary<string, Guid> _addressToHomeId = new(StringComparer.OrdinalIgnoreCase);

    public void Register(HomeAggregate home)
    {
        if (_addressToHomeId.TryGetValue(home.Address, out var existingId) && existingId != home.Id)
            throw new DomainException($"Address '{home.Address}' is already registered to another smart home.");
        _addressToHomeId[home.Address] = home.Id;
    }

    public bool IsAddressRegistered(string address)
        => _addressToHomeId.ContainsKey(address);
}
