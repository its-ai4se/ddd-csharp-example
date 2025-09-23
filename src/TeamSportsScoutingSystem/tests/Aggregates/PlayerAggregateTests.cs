using TeamSportsScoutingSystem.Domain.Player;
using TeamSportsScoutingSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace TeamSportsScoutingSystem.Domain.Tests.Aggregates;

public class PlayerAggregateTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePlayer()
    {
        // Arrange
        var name = new PersonName("John", "Doe");
        var dateOfBirth = new DateOnly(1995, 5, 15);
        var listType = PlayerListType.LongList;

        // Act
        var player = new PlayerAggregate(name, dateOfBirth, listType);

        // Assert
        Assert.Equal(name, player.Name);
        Assert.Equal(dateOfBirth, player.DateOfBirth);
        Assert.Equal(listType, player.ListType);
        Assert.NotEqual(Guid.Empty, player.Id);
        Assert.True(player.AddedToListOn <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        PersonName? name = null;
        var dateOfBirth = new DateOnly(1995, 5, 15);
        var listType = PlayerListType.LongList;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PlayerAggregate(name!, dateOfBirth, listType));
    }

    [Fact]
    public void AddAttribute_WithValidAttribute_ShouldAddAttribute()
    {
        // Arrange
        var player = CreateTestPlayer();
        var attribute = new PlayerAttribute("Speed", "Fast");

        // Act
        player.AddAttribute(attribute);

        // Assert
        Assert.Contains(attribute, player.Attributes);
        Assert.Single(player.Attributes);
    }

    [Fact]
    public void AddAttribute_WithExistingAttributeName_ShouldReplaceAttribute()
    {
        // Arrange
        var player = CreateTestPlayer();
        var attribute1 = new PlayerAttribute("Speed", "Fast");
        var attribute2 = new PlayerAttribute("Speed", "Very Fast");

        // Act
        player.AddAttribute(attribute1);
        player.AddAttribute(attribute2);

        // Assert
        Assert.Single(player.Attributes);
        Assert.Equal("Very Fast", player.Attributes.First().Value);
    }

    [Fact]
    public void MoveToList_WithValidListType_ShouldUpdateListType()
    {
        // Arrange
        var player = CreateTestPlayer();
        var shortList = PlayerListType.ShortList;

        // Act
        player.MoveToList(shortList);

        // Assert
        Assert.Equal(shortList, player.ListType);
    }

    [Fact]
    public void GetAttribute_WithExistingAttribute_ShouldReturnAttribute()
    {
        // Arrange
        var player = CreateTestPlayer();
        var attribute = new PlayerAttribute("Speed", "Fast");
        player.AddAttribute(attribute);

        // Act
        var result = player.GetAttribute("Speed");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Fast", result.Value);
    }

    [Fact]
    public void GetAttribute_WithNonExistingAttribute_ShouldReturnNull()
    {
        // Arrange
        var player = CreateTestPlayer();

        // Act
        var result = player.GetAttribute("NonExisting");

        // Assert
        Assert.Null(result);
    }

    private static PlayerAggregate CreateTestPlayer()
    {
        var name = new PersonName("John", "Doe");
        var dateOfBirth = new DateOnly(1995, 5, 15);
        var listType = PlayerListType.LongList;
        return new PlayerAggregate(name, dateOfBirth, listType);
    }
}
