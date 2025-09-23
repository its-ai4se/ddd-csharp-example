using TeamSportsScoutingSystem.Domain.Shared.Common;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

namespace TeamSportsScoutingSystem.Domain.Person;

public class PersonAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }
    public EmailAddress? EmailAddress { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }

    private readonly List<UserRole> _roles = new();

    public PersonAggregate(Guid id, PersonName name, EmailAddress? emailAddress = null, PhoneNumber? phoneNumber = null) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;
    }

    public PersonAggregate(PersonName name, EmailAddress? emailAddress = null, PhoneNumber? phoneNumber = null) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;
    }

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    public void UpdateName(PersonName newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public void UpdateEmailAddress(EmailAddress? newEmailAddress)
    {
        EmailAddress = newEmailAddress;
    }

    public void UpdatePhoneNumber(PhoneNumber? newPhoneNumber)
    {
        PhoneNumber = newPhoneNumber;
    }

    public void AddRole(UserRole role)
    {
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

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

    public void RemoveRole<T>() where T : UserRole
    {
        var roleToRemove = _roles.OfType<T>().FirstOrDefault();
        if (roleToRemove != null)
        {
            _roles.Remove(roleToRemove);
        }
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
