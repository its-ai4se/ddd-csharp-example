using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.Person;

public class PersonAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }
    public Address Address { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public Password Password { get; private set; }

    private readonly List<UserRole> _roles = new();

    public PersonAggregate(Guid id, PersonName name, Address address, PhoneNumber phoneNumber, EmailAddress emailAddress, Password password) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        Password = password ?? throw new ArgumentNullException(nameof(password));
    }

    public PersonAggregate(PersonName name, Address address, PhoneNumber phoneNumber, EmailAddress emailAddress, Password password) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        Password = password ?? throw new ArgumentNullException(nameof(password));
    }

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    public void UpdateName(PersonName newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
    }

    public void UpdatePhoneNumber(PhoneNumber newPhoneNumber)
    {
        PhoneNumber = newPhoneNumber ?? throw new ArgumentNullException(nameof(newPhoneNumber));
    }

    public void UpdateEmailAddress(EmailAddress newEmailAddress)
    {
        EmailAddress = newEmailAddress ?? throw new ArgumentNullException(nameof(newEmailAddress));
    }

    public void UpdatePassword(Password newPassword)
    {
        Password = newPassword ?? throw new ArgumentNullException(nameof(newPassword));
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

    public bool IsOrganizer => HasRole<OrganizerRole>();
    public bool IsAttendee => HasRole<AttendeeRole>();

    public override string ToString() => $"Person: {Name} (ID: {Id})";
}
