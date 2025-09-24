using Xunit;
using SmartHomeAutomationSystem.Domain.Home;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Tests.Aggregates;

public class HomeAggregateTests
{
    [Fact]
    public void HomeAggregate_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var home = new HomeAggregate("Smart Home", "123 Main Street");

        // Assert
        Assert.Equal("Smart Home", home.Name);
        Assert.Equal("123 Main Street", home.Address);
        Assert.Empty(home.RoomIds);
        Assert.Empty(home.UserIds);
        Assert.NotEqual(Guid.Empty, home.Id);
    }

    [Fact]
    public void HomeAggregate_WithEmptyName_ShouldThrowDomainException()
    {
        // Arrange, Act & Assert
        Assert.Throws<DomainException>(() => new HomeAggregate("", "123 Main Street"));
    }

    [Fact]
    public void HomeAggregate_WithEmptyAddress_ShouldThrowDomainException()
    {
        // Arrange, Act & Assert
        Assert.Throws<DomainException>(() => new HomeAggregate("Smart Home", ""));
    }

    [Fact]
    public void AddRoom_WithValidRoomId_ShouldAddRoom()
    {
        // Arrange
        var home = new HomeAggregate("Smart Home", "123 Main Street");
        var roomId = Guid.NewGuid();

        // Act
        home.AddRoom(roomId);

        // Assert
        Assert.Contains(roomId, home.RoomIds);
    }

    [Fact]
    public void AddRoom_WithEmptyRoomId_ShouldThrowDomainException()
    {
        // Arrange
        var home = new HomeAggregate("Smart Home", "123 Main Street");

        // Act & Assert
        Assert.Throws<DomainException>(() => home.AddRoom(Guid.Empty));
    }

    [Fact]
    public void AddRoom_WithDuplicateRoomId_ShouldThrowDomainException()
    {
        // Arrange
        var home = new HomeAggregate("Smart Home", "123 Main Street");
        var roomId = Guid.NewGuid();
        home.AddRoom(roomId);

        // Act & Assert
        Assert.Throws<DomainException>(() => home.AddRoom(roomId));
    }

    [Fact]
    public void RemoveRoom_WithExistingRoomId_ShouldRemoveRoom()
    {
        // Arrange
        var home = new HomeAggregate("Smart Home", "123 Main Street");
        var roomId = Guid.NewGuid();
        home.AddRoom(roomId);

        // Act
        home.RemoveRoom(roomId);

        // Assert
        Assert.DoesNotContain(roomId, home.RoomIds);
    }

    [Fact]
    public void RemoveRoom_WithNonExistentRoomId_ShouldThrowDomainException()
    {
        // Arrange
        var home = new HomeAggregate("Smart Home", "123 Main Street");
        var roomId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<DomainException>(() => home.RemoveRoom(roomId));
    }
}
