using CelebrationOrganizationSystem.Domain.Shared.Common;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

namespace CelebrationOrganizationSystem.Domain.Person;

public class PersonAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }
    public Address? Address { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public Password Password { get; private set; }

    private readonly List<UserRole> _roles = [];

    public PersonAggregate(Guid id, PersonName name, Address? address, PhoneNumber? phoneNumber, EmailAddress emailAddress, Password password) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        Password = password ?? throw new ArgumentNullException(nameof(password));
    }

    public PersonAggregate(PersonName name, Address? address, PhoneNumber? phoneNumber, EmailAddress emailAddress, Password password) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        Password = password ?? throw new ArgumentNullException(nameof(password));
    }

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    public void AddRole(UserRole role)
    {
        if (role is null)
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

    public bool HasRole<T>() where T : UserRole => _roles.OfType<T>().Any();

    public bool IsOrganizer => HasRole<OrganizerRole>();
    public bool IsAttendee => HasRole<AttendeeRole>();

    public override string ToString() => $"Person: {Name} (ID: {Id})";
}
