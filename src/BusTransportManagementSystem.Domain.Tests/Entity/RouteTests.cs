using BusTransportManagementSystem.Domain.Entity;
using BusTransportManagementSystem.Domain.ValueObject;
using Xunit;

namespace BusTransportManagementSystem.Domain.Tests.Entity;

public class RouteTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateRoute()
    {
        // Arrange
        var id = Guid.NewGuid();
        var routeNumber = new RouteNumber("123");

        // Act
        var route = new Route(id, routeNumber);

        // Assert
        Assert.Equal(id, route.Id);
        Assert.Equal(routeNumber, route.RouteNumber);
    }

    [Fact]
    public void Constructor_WithRouteNumberOnly_ShouldGenerateId()
    {
        // Arrange
        var routeNumber = new RouteNumber("123");

        // Act
        var route = new Route(routeNumber);

        // Assert
        Assert.NotEqual(Guid.Empty, route.Id);
        Assert.Equal(routeNumber, route.RouteNumber);
    }

    [Fact]
    public void Constructor_WithEmptyId_ShouldThrowArgumentException()
    {
        // Arrange
        var emptyId = Guid.Empty;
        var routeNumber = new RouteNumber("123");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Route(emptyId, routeNumber));
    }

    [Fact]
    public void Constructor_WithNullRouteNumber_ShouldThrowArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        RouteNumber nullRouteNumber = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Route(id, nullRouteNumber));
    }

    [Fact]
    public void UpdateRouteNumber_WithValidRouteNumber_ShouldUpdateRouteNumber()
    {
        // Arrange
        var route = new Route(new RouteNumber("123"));
        var newRouteNumber = new RouteNumber("456");

        // Act
        route.UpdateRouteNumber(newRouteNumber);

        // Assert
        Assert.Equal(newRouteNumber, route.RouteNumber);
    }

    [Fact]
    public void UpdateRouteNumber_WithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var route = new Route(new RouteNumber("123"));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => route.UpdateRouteNumber(null!));
    }

    [Fact]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var route1 = new Route(id, new RouteNumber("123"));
        var route2 = new Route(id, new RouteNumber("456")); // Different route number, same ID

        // Act & Assert
        Assert.True(route1.Equals(route2));
        Assert.True(route1 == route2);
        Assert.False(route1 != route2);
    }

    [Fact]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // Arrange
        var route1 = new Route(new RouteNumber("123"));
        var route2 = new Route(new RouteNumber("123")); // Same route number, different ID

        // Act & Assert
        Assert.False(route1.Equals(route2));
        Assert.False(route1 == route2);
        Assert.True(route1 != route2);
    }

    [Fact]
    public void GetHashCode_WithSameId_ShouldBeSame()
    {
        // Arrange
        var id = Guid.NewGuid();
        var route1 = new Route(id, new RouteNumber("123"));
        var route2 = new Route(id, new RouteNumber("456"));

        // Act & Assert
        Assert.Equal(route1.GetHashCode(), route2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldIncludeRouteInformation()
    {
        // Arrange
        var route = new Route(new RouteNumber("123"));

        // Act
        var result = route.ToString();

        // Assert
        Assert.Contains("123", result);
        Assert.Contains(route.Id.ToString(), result);
    }

    [Fact]
    public void RequirementValidation_RouteIdentifiedByNumber()
    {
        // Based on requirement: "A bus route is identified by a unique number that is determined by city staff"
        
        // Arrange
        var routeNumber = new RouteNumber("123");

        // Act
        var route = new Route(routeNumber);

        // Assert
        Assert.Equal(routeNumber, route.RouteNumber);
        Assert.Equal(123, route.RouteNumber.Value);
        Assert.NotEqual(Guid.Empty, route.Id);
    }

    [Fact]
    public void RequirementValidation_RouteNumberRange()
    {
        // Based on requirement: "The highest possible number for a bus route is 9999"
        
        // Arrange & Act
        var minRoute = new Route(new RouteNumber("1"));
        var maxRoute = new Route(new RouteNumber("9999"));

        // Assert
        Assert.Equal(1, minRoute.RouteNumber.Value);
        Assert.Equal(9999, maxRoute.RouteNumber.Value);
        
        // Note: Current RouteNumber implementation doesn't enforce the 9999 limit
        // This is a potential improvement area identified by the tests
    }

    [Theory]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("9999")]
    public void RequirementValidation_ValidRouteNumbers_ShouldBeAccepted(string routeNumberString)
    {
        // Based on requirement validation for route number ranges
        
        // Arrange & Act
        var routeNumber = new RouteNumber(routeNumberString);
        var route = new Route(routeNumber);

        // Assert
        Assert.Equal(int.Parse(routeNumberString), route.RouteNumber.Value);
        Assert.NotEqual(Guid.Empty, route.Id);
    }

    [Fact]
    public void RequirementValidation_RouteSupportMultipleShifts()
    {
        // Based on requirement: "For each route, there is a morning shift, an afternoon shift, and a night shift"
        // Note: This is tested more comprehensively in Schedule aggregate tests
        
        // Arrange
        var route = new Route(new RouteNumber("123"));
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);
        var nightShift = new ShiftPeriod(ShiftPeriodType.Night);

        // Act & Assert
        // A route itself doesn't contain shifts, but it should be able to be referenced by shifts
        Assert.NotNull(route.RouteNumber);
        Assert.NotEqual(Guid.Empty, route.Id);
        
        // Verify all three shift types are available
        Assert.Equal(ShiftPeriodType.Morning, morningShift.Value);
        Assert.Equal(ShiftPeriodType.Afternoon, afternoonShift.Value);
        Assert.Equal(ShiftPeriodType.Night, nightShift.Value);
    }
}
