using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.ValueObjects;

public class PersonNameTests
{
    [Fact]
    public void Constructor_WithValidNames_ShouldCreatePersonName()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var personName = new PersonName(firstName, lastName);

        // Assert
        Assert.Equal(firstName, personName.FirstName);
        Assert.Equal(lastName, personName.LastName);
        Assert.Equal("John Doe", personName.FullName);
    }

    [Fact]
    public void Constructor_WithEmptyFirstName_ShouldThrowArgumentException()
    {
        // Arrange
        var firstName = "";
        var lastName = "Doe";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PersonName(firstName, lastName));
    }

    [Fact]
    public void Constructor_WithWhitespaceLastName_ShouldThrowArgumentException()
    {
        // Arrange
        var firstName = "John";
        var lastName = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PersonName(firstName, lastName));
    }

    [Fact]
    public void Equals_WithSameNames_ShouldReturnTrue()
    {
        // Arrange
        var name1 = new PersonName("John", "Doe");
        var name2 = new PersonName("John", "Doe");

        // Act & Assert
        Assert.Equal(name1, name2);
        Assert.True(name1 == name2);
    }

    [Fact]
    public void Equals_WithDifferentNames_ShouldReturnFalse()
    {
        // Arrange
        var name1 = new PersonName("John", "Doe");
        var name2 = new PersonName("Jane", "Doe");

        // Act & Assert
        Assert.NotEqual(name1, name2);
        Assert.True(name1 != name2);
    }
}
