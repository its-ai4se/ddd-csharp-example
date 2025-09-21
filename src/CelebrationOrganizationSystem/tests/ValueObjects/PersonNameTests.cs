using CelebrationOrganizationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace CelebrationOrganizationSystem.Domain.Tests.ValueObjects;

public class PersonNameTests
{
    [Fact]
    public void CreatePersonName_WithValidNames_ShouldSucceed()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var personName = new PersonName(firstName, lastName);

        // Assert
        Assert.Equal("John", personName.FirstName);
        Assert.Equal("Doe", personName.LastName);
        Assert.Equal("John Doe", personName.FullName);
    }

    [Fact]
    public void CreatePersonName_WithWhitespace_ShouldTrim()
    {
        // Arrange
        var firstName = "  John  ";
        var lastName = "  Doe  ";

        // Act
        var personName = new PersonName(firstName, lastName);

        // Assert
        Assert.Equal("John", personName.FirstName);
        Assert.Equal("Doe", personName.LastName);
    }

    [Theory]
    [InlineData("", "Doe")]
    [InlineData("   ", "Doe")]
    [InlineData(null, "Doe")]
    [InlineData("John", "")]
    [InlineData("John", "   ")]
    [InlineData("John", null)]
    public void CreatePersonName_WithInvalidNames_ShouldThrowException(string firstName, string lastName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PersonName(firstName, lastName));
    }

    [Fact]
    public void PersonName_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var name1 = new PersonName("John", "Doe");
        var name2 = new PersonName("John", "Doe");
        var name3 = new PersonName("Jane", "Doe");

        // Assert
        Assert.Equal(name1, name2);
        Assert.NotEqual(name1, name3);
    }
}
