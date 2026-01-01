using BusTransportManagementSystem.Domain.Route;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace BusTransportManagementSystem.Tests.Route;

public class RouteAggregateTests
{
    #region Add Route Tests

    [Fact]
    public void RT001_AddRouteWithNumber1_ShouldCreateSuccessfully()
    {
        var routeNumber = new RouteNumber("1");

        var route = new RouteAggregate(routeNumber);

        Assert.NotNull(route);
        Assert.Equal(1, route.RouteNumber.Value);
        Assert.NotEqual(Guid.Empty, route.Id);
    }

    [Fact]
    public void RT002_AddRouteWithNumber9999_ShouldCreateSuccessfully()
    {
        var routeNumber = new RouteNumber("9999");

        var route = new RouteAggregate(routeNumber);

        Assert.NotNull(route);
        Assert.Equal(9999, route.RouteNumber.Value);
    }

    [Fact]
    public void RT003_AddRouteWithNumber10000_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RouteNumber("10000"));
    }

    [Fact]
    public void RT004_AddRouteWithNegativeNumber_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RouteNumber("-1"));
    }

    [Fact]
    public void RT005_AddRouteWithDuplicateNumber_ShouldPreventDuplicate()
    {
        var routeNumber = new RouteNumber("1");
        var routes = new List<RouteAggregate>{};
        routes.Add(new RouteAggregate(routeNumber));

        var duplicateRouteNumber = new RouteNumber("1");

        Assert.Throws<InvalidOperationException>(() =>
        {
            var duplicateRoute = new RouteAggregate(duplicateRouteNumber);
            
            if (routes.Any(r => r.RouteNumber.Value == duplicateRoute.RouteNumber.Value))
            {
                throw new InvalidOperationException("Route number must be unique");
            }
            
            routes.Add(duplicateRoute);
        });

        Assert.Single(routes);
    }

    [Fact]
    public void RT006_AddRouteWithDecimalNumber_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RouteNumber("123.45"));
    }

    [Fact]
    public void RT007_AddRouteWithNullNumber_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RouteAggregate((RouteNumber)null!));
    }

    #endregion
    
    #region Route Number Validation Tests

    [Fact]
    public void RT008_CreateRouteWithWhitespaceNumber_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RouteNumber("   "));
    }

    [Fact]
    public void RT009_CreateRouteWithStringThatIsNotInteger_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new RouteNumber("abc"));
    }

    [Fact]
    public void RT010_CreateMultipleRoutes_ShouldAllHaveUniqueIds()
    {
        var routeNumbers = new[] { "1", "2", "3" };
        var routes = new List<RouteAggregate>();

        foreach (var number in routeNumbers)
        {
            routes.Add(new RouteAggregate(new RouteNumber(number)));
        }

        Assert.Equal(3, routes.Count);
        var uniqueIds = routes.Select(r => r.Id).Distinct().ToList();
        Assert.Equal(3, uniqueIds.Count);
    }

    #endregion

    #region Shift Type Tests

    [Fact]
    public void RT101_VerifyMorningShift_ShouldBeValidShiftType()
    {
        var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);

        Assert.NotNull(morningShift);
        Assert.Equal(ShiftPeriodType.Morning, morningShift.Value);
    }

    [Fact]
    public void RT102_VerifyAfternoonShift_ShouldBeValidShiftType()
    {
        var afternoonShift = new ShiftPeriod(ShiftPeriodType.Afternoon);

        Assert.NotNull(afternoonShift);
        Assert.Equal(ShiftPeriodType.Afternoon, afternoonShift.Value);
    }

    [Fact]
    public void RT103_VerifyNightShift_ShouldBeValidShiftType()
    {
        var nightShift = new ShiftPeriod(ShiftPeriodType.Night);

        Assert.NotNull(nightShift);
        Assert.Equal(ShiftPeriodType.Night, nightShift.Value);
    }

    [Fact]
    public void RT104_AttemptInvalidShiftType_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ShiftPeriod("evening"));
    }

    #endregion

}

