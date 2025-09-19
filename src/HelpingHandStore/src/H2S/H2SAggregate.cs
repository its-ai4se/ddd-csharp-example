using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.H2S;

public class H2SAggregate : AggregateRoot
{
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public string City { get; private set; }

    private readonly List<Guid> _personIds = new();
    private readonly List<Guid> _itemIds = new();
    private readonly List<Guid> _vehicleIds = new();
    private readonly List<Guid> _routeIds = new();

    public H2SAggregate(Guid id, string name, Address address, string city) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("H2S name cannot be empty or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty or whitespace.", nameof(city));
        }

        Name = name.Trim();
        Address = address ?? throw new ArgumentNullException(nameof(address));
        City = city.Trim();
    }

    public H2SAggregate(string name, Address address, string city) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("H2S name cannot be empty or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City cannot be empty or whitespace.", nameof(city));
        }

        Name = name.Trim();
        Address = address ?? throw new ArgumentNullException(nameof(address));
        City = city.Trim();
    }

    public IReadOnlyList<Guid> PersonIds => _personIds.AsReadOnly();
    public IReadOnlyList<Guid> ItemIds => _itemIds.AsReadOnly();
    public IReadOnlyList<Guid> VehicleIds => _vehicleIds.AsReadOnly();
    public IReadOnlyList<Guid> RouteIds => _routeIds.AsReadOnly();

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("H2S name cannot be empty or whitespace.", nameof(newName));
        }

        Name = newName.Trim();
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
    }

    public void UpdateCity(string newCity)
    {
        if (string.IsNullOrWhiteSpace(newCity))
        {
            throw new ArgumentException("City cannot be empty or whitespace.", nameof(newCity));
        }

        City = newCity.Trim();
    }

    public void AddPerson(Guid personId)
    {
        if (!_personIds.Contains(personId))
        {
            _personIds.Add(personId);
        }
    }

    public void RemovePerson(Guid personId)
    {
        _personIds.Remove(personId);
    }

    public void AddItem(Guid itemId)
    {
        if (!_itemIds.Contains(itemId))
        {
            _itemIds.Add(itemId);
        }
    }

    public void RemoveItem(Guid itemId)
    {
        _itemIds.Remove(itemId);
    }

    public void AddVehicle(Guid vehicleId)
    {
        if (!_vehicleIds.Contains(vehicleId))
        {
            _vehicleIds.Add(vehicleId);
        }
    }

    public void RemoveVehicle(Guid vehicleId)
    {
        _vehicleIds.Remove(vehicleId);
    }

    public void AddRoute(Guid routeId)
    {
        if (!_routeIds.Contains(routeId))
        {
            _routeIds.Add(routeId);
        }
    }

    public void RemoveRoute(Guid routeId)
    {
        _routeIds.Remove(routeId);
    }

    public override string ToString() => $"H2S: {Name} in {City} (ID: {Id})";
}
