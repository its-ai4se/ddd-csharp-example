using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Person;

public class PersonAggregate : AggregateRoot
{
    public PersonName Name { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<UserRole> _roles = new();

    public PersonAggregate(Guid id, PersonName name, EmailAddress emailAddress, DateTime dateOfBirth, PhoneNumber? phoneNumber = null) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
    }

    public PersonAggregate(PersonName name, EmailAddress emailAddress, DateTime dateOfBirth, PhoneNumber? phoneNumber = null) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        CreatedAt = DateTime.UtcNow;
    }

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    public void UpdateName(PersonName newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public void UpdateEmailAddress(EmailAddress newEmailAddress)
    {
        EmailAddress = newEmailAddress ?? throw new ArgumentNullException(nameof(newEmailAddress));
    }

    public void UpdatePhoneNumber(PhoneNumber? newPhoneNumber)
    {
        PhoneNumber = newPhoneNumber;
    }

    public void AddRole(UserRole role)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));

        if (role.PersonId != Id)
            throw new DomainException("Role must belong to this person.");

        if (_roles.Any(r => r.GetType() == role.GetType()))
            throw new DomainException($"Person already has role of type {role.GetType().Name}.");

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

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    public override string ToString() => $"Person: {Name} (ID: {Id})";
}
