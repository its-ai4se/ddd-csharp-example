using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;

namespace HelpingHandStore.Domain.Person;

public class PersonAggregate : AggregateRoot
{
    public Guid H2SId { get; private set; }
    public PersonName Name { get; private set; }
    public Address Address { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public EmailAddress? EmailAddress { get; private set; }

    private readonly List<UserRole> _roles = new();

    public PersonAggregate(Guid id, Guid h2sId, PersonName name, Address address, PhoneNumber phoneNumber, EmailAddress? emailAddress = null) : base(id)
    {
        H2SId = h2sId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        EmailAddress = emailAddress;
    }

    public PersonAggregate(Guid h2sId, PersonName name, Address address, PhoneNumber phoneNumber, EmailAddress? emailAddress = null) : base()
    {
        H2SId = h2sId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        EmailAddress = emailAddress;
    }

    public void AddRole(UserRole role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (role.PersonId != Id)
        {
            throw new ArgumentException("Role must belong to this person.", nameof(role));
        }

        if (_roles.Any(r => r.GetType() == role.GetType()))
        {
            throw new InvalidOperationException($"Person already has role of type {role.GetType().Name}.");
        }

        _roles.Add(role);
    }

    public bool HasRole<T>() where T : UserRole
    {
        return _roles.OfType<T>().Any();
    }

    public T? GetRole<T>() where T : UserRole
    {
        return _roles.OfType<T>().FirstOrDefault();
    }

    public override string ToString() => $"Person: {Name} (ID: {Id})";
}
