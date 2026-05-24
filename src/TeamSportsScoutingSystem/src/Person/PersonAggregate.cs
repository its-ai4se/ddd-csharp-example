using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Person;

public class PersonAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }

    private readonly List<UserRole> _roles = new();

    public PersonAggregate(Guid id, PersonName name) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public PersonAggregate(PersonName name) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    public void AddRole(UserRole role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (role.PersonId != Id)
            throw new ArgumentException("Role must belong to this person.", nameof(role));

        if (_roles.Any(r => r.GetType() == role.GetType()))
            throw new InvalidOperationException($"Person already has role of type {role.GetType().Name}.");

        _roles.Add(role);
    }

    public bool HasRole<T>() where T : UserRole => _roles.OfType<T>().Any();

    public T? GetRole<T>() where T : UserRole => _roles.OfType<T>().FirstOrDefault();

    public override string ToString() => $"Person: {Name} (ID: {Id})";
}
