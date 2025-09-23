using TeamSportsScoutingSystem.Domain.Person;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.Aggregates;

public class PersonAggregateTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePerson()
    {
        // Arrange
        var name = new PersonName("John", "Doe");
        var email = new EmailAddress("john.doe@example.com");

        // Act
        var person = new PersonAggregate(name, email);

        // Assert
        Assert.Equal(name, person.Name);
        Assert.Equal(email, person.EmailAddress);
        Assert.NotEqual(Guid.Empty, person.Id);
        Assert.Empty(person.Roles);
    }

    [Fact]
    public void AddRole_WithValidRole_ShouldAddRole()
    {
        // Arrange
        var person = CreateTestPerson();
        var scoutRole = new ScoutRole(person.Id);

        // Act
        person.AddRole(scoutRole);

        // Assert
        Assert.Single(person.Roles);
        Assert.Contains(scoutRole, person.Roles);
    }

    [Fact]
    public void AddRole_WithDuplicateRoleType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var person = CreateTestPerson();
        var scoutRole1 = new ScoutRole(person.Id);
        var scoutRole2 = new ScoutRole(person.Id);

        // Act
        person.AddRole(scoutRole1);

        // Assert
        Assert.Throws<InvalidOperationException>(() => person.AddRole(scoutRole2));
    }

    [Fact]
    public void HasRole_WithExistingRole_ShouldReturnTrue()
    {
        // Arrange
        var person = CreateTestPerson();
        var scoutRole = new ScoutRole(person.Id);
        person.AddRole(scoutRole);

        // Act
        var hasRole = person.HasRole<ScoutRole>();

        // Assert
        Assert.True(hasRole);
    }

    [Fact]
    public void HasRole_WithNonExistingRole_ShouldReturnFalse()
    {
        // Arrange
        var person = CreateTestPerson();

        // Act
        var hasRole = person.HasRole<ScoutRole>();

        // Assert
        Assert.False(hasRole);
    }

    [Fact]
    public void GetRole_WithExistingRole_ShouldReturnRole()
    {
        // Arrange
        var person = CreateTestPerson();
        var scoutRole = new ScoutRole(person.Id, isHeadScout: true);
        person.AddRole(scoutRole);

        // Act
        var role = person.GetRole<ScoutRole>();

        // Assert
        Assert.NotNull(role);
        Assert.True(role.IsHeadScout);
    }

    [Fact]
    public void RemoveRole_WithExistingRole_ShouldRemoveRole()
    {
        // Arrange
        var person = CreateTestPerson();
        var scoutRole = new ScoutRole(person.Id);
        person.AddRole(scoutRole);

        // Act
        person.RemoveRole<ScoutRole>();

        // Assert
        Assert.Empty(person.Roles);
        Assert.False(person.HasRole<ScoutRole>());
    }

    private static PersonAggregate CreateTestPerson()
    {
        var name = new PersonName("John", "Doe");
        return new PersonAggregate(name);
    }
}
