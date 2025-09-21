using CelebrationOrganizationSystem.Domain.Person;
using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.Person;

public class PersonAggregateTests
{
    private PersonAggregate CreateValidPerson()
    {
        var name = new PersonName("John", "Doe");
        var address = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
        var phoneNumber = new PhoneNumber("555-123-4567");
        var emailAddress = new EmailAddress("john.doe@email.com");
        var password = new Password("SecurePassword123!");

        return new PersonAggregate(name, address, phoneNumber, emailAddress, password);
    }

    [Fact]
    public void CreatePerson_WithValidData_ShouldSucceed()
    {
        // Arrange
        var name = new PersonName("John", "Doe");
        var address = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
        var phoneNumber = new PhoneNumber("555-123-4567");
        var emailAddress = new EmailAddress("john.doe@email.com");
        var password = new Password("SecurePassword123!");

        // Act
        var person = new PersonAggregate(name, address, phoneNumber, emailAddress, password);

        // Assert
        Assert.Equal(name, person.Name);
        Assert.Equal(address, person.Address);
        Assert.Equal(phoneNumber, person.PhoneNumber);
        Assert.Equal(emailAddress, person.EmailAddress);
        Assert.Equal(password, person.Password);
        Assert.Empty(person.Roles);
    }

    [Fact]
    public void CreatePerson_WithNullValues_ShouldThrowException()
    {
        // Arrange
        var name = new PersonName("John", "Doe");
        var address = new Address("123 Main St", "Anytown", "CA", "12345", "USA");
        var phoneNumber = new PhoneNumber("555-123-4567");
        var emailAddress = new EmailAddress("john.doe@email.com");
        var password = new Password("SecurePassword123!");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PersonAggregate(null!, address, phoneNumber, emailAddress, password));
        Assert.Throws<ArgumentNullException>(() => new PersonAggregate(name, null!, phoneNumber, emailAddress, password));
        Assert.Throws<ArgumentNullException>(() => new PersonAggregate(name, address, null!, emailAddress, password));
        Assert.Throws<ArgumentNullException>(() => new PersonAggregate(name, address, phoneNumber, null!, password));
        Assert.Throws<ArgumentNullException>(() => new PersonAggregate(name, address, phoneNumber, emailAddress, null!));
    }

    [Fact]
    public void AddRole_WithValidRole_ShouldSucceed()
    {
        // Arrange
        var person = CreateValidPerson();
        var organizerRole = new OrganizerRole(person.Id);

        // Act
        person.AddRole(organizerRole);

        // Assert
        Assert.Single(person.Roles);
        Assert.True(person.HasRole<OrganizerRole>());
        Assert.False(person.IsAttendee);
        Assert.True(person.IsOrganizer);
    }

    [Fact]
    public void AddRole_WithAttendeeRole_ShouldSucceed()
    {
        // Arrange
        var person = CreateValidPerson();
        var attendeeRole = new AttendeeRole(person.Id);

        // Act
        person.AddRole(attendeeRole);

        // Assert
        Assert.Single(person.Roles);
        Assert.True(person.HasRole<AttendeeRole>());
        Assert.True(person.IsAttendee);
        Assert.False(person.IsOrganizer);
    }

    [Fact]
    public void AddRole_WithMultipleRoles_ShouldSucceed()
    {
        // Arrange
        var person = CreateValidPerson();
        var organizerRole = new OrganizerRole(person.Id);
        var attendeeRole = new AttendeeRole(person.Id);

        // Act
        person.AddRole(organizerRole);
        person.AddRole(attendeeRole);

        // Assert
        Assert.Equal(2, person.Roles.Count);
        Assert.True(person.HasRole<OrganizerRole>());
        Assert.True(person.HasRole<AttendeeRole>());
        Assert.True(person.IsOrganizer);
        Assert.True(person.IsAttendee);
    }

    [Fact]
    public void AddRole_WithDuplicateRole_ShouldThrowException()
    {
        // Arrange
        var person = CreateValidPerson();
        var organizerRole1 = new OrganizerRole(person.Id);
        var organizerRole2 = new OrganizerRole(person.Id);

        // Act
        person.AddRole(organizerRole1);

        // Assert
        Assert.Throws<InvalidOperationException>(() => person.AddRole(organizerRole2));
    }

    [Fact]
    public void AddRole_WithWrongPersonId_ShouldThrowException()
    {
        // Arrange
        var person = CreateValidPerson();
        var organizerRole = new OrganizerRole(Guid.NewGuid());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => person.AddRole(organizerRole));
    }

    [Fact]
    public void RemoveRole_WithExistingRole_ShouldSucceed()
    {
        // Arrange
        var person = CreateValidPerson();
        var organizerRole = new OrganizerRole(person.Id);
        person.AddRole(organizerRole);

        // Act
        person.RemoveRole<OrganizerRole>();

        // Assert
        Assert.Empty(person.Roles);
        Assert.False(person.HasRole<OrganizerRole>());
        Assert.False(person.IsOrganizer);
    }

    [Fact]
    public void RemoveRole_WithNonExistingRole_ShouldNotThrowException()
    {
        // Arrange
        var person = CreateValidPerson();

        // Act & Assert
        person.RemoveRole<OrganizerRole>();
        Assert.Empty(person.Roles);
    }

    [Fact]
    public void GetRole_WithExistingRole_ShouldReturnRole()
    {
        // Arrange
        var person = CreateValidPerson();
        var organizerRole = new OrganizerRole(person.Id);
        person.AddRole(organizerRole);

        // Act
        var retrievedRole = person.GetRole<OrganizerRole>();

        // Assert
        Assert.NotNull(retrievedRole);
        Assert.Equal(organizerRole.Id, retrievedRole!.Id);
    }

    [Fact]
    public void GetRole_WithNonExistingRole_ShouldReturnNull()
    {
        // Arrange
        var person = CreateValidPerson();

        // Act
        var retrievedRole = person.GetRole<OrganizerRole>();

        // Assert
        Assert.Null(retrievedRole);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldSucceed()
    {
        // Arrange
        var person = CreateValidPerson();
        var newName = new PersonName("Jane", "Smith");

        // Act
        person.UpdateName(newName);

        // Assert
        Assert.Equal(newName, person.Name);
    }

    [Fact]
    public void UpdateName_WithNullName_ShouldThrowException()
    {
        // Arrange
        var person = CreateValidPerson();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => person.UpdateName(null!));
    }

    [Fact]
    public void UpdateAddress_WithValidAddress_ShouldSucceed()
    {
        // Arrange
        var person = CreateValidPerson();
        var newAddress = new Address("456 Oak Ave", "Othertown", "NY", "67890", "USA");

        // Act
        person.UpdateAddress(newAddress);

        // Assert
        Assert.Equal(newAddress, person.Address);
    }

    [Fact]
    public void UpdateEmailAddress_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        var person = CreateValidPerson();
        var newEmail = new EmailAddress("jane.smith@email.com");

        // Act
        person.UpdateEmailAddress(newEmail);

        // Assert
        Assert.Equal(newEmail, person.EmailAddress);
    }

    [Fact]
    public void UpdatePassword_WithValidPassword_ShouldSucceed()
    {
        // Arrange
        var person = CreateValidPerson();
        var newPassword = new Password("NewSecurePassword456!");

        // Act
        person.UpdatePassword(newPassword);

        // Assert
        Assert.Equal(newPassword, person.Password);
    }
}
