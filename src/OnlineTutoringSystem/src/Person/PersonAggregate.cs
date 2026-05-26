using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Person;

public class PersonAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }
    public EmailAddress EmailAddress { get; private set; }

    private readonly List<UserRole> _roles = [];

    public PersonAggregate(PersonName name, EmailAddress emailAddress) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
    }

    public void AddRole(UserRole role)
    {
        if (role == null) throw new ArgumentNullException(nameof(role));
        if (role.PersonId != Id)
            throw new DomainException("Role must belong to this person.");
        if (_roles.Any(r => r.GetType() == role.GetType()))
            throw new DomainException($"Person already has role of type {role.GetType().Name}.");
        _roles.Add(role);
    }

    public bool HasRole<T>() where T : UserRole => _roles.OfType<T>().Any();

    public T? GetRole<T>() where T : UserRole => _roles.OfType<T>().FirstOrDefault();

    public override string ToString() => $"Person: {Name} (ID: {Id})";
}
