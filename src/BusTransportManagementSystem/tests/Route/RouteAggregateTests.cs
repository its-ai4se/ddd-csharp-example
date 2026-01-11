using BusTransportManagementSystem.Domain.Route;
using BusTransportManagementSystem.Domain.Route.Repositories;
using BusTransportManagementSystem.Domain.Shared.Common;
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
    public async Task RT005_AddRouteWithDuplicateNumber_ShouldPreventDuplicate()
    {
        var repository = new MockRouteRepository();
        var routeNumber = new RouteNumber("1");
        var route = new RouteAggregate(routeNumber);

        await repository.AddAsync(route);

        var duplicateRoute = new RouteAggregate(routeNumber);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => repository.AddAsync(duplicateRoute));

        Assert.Contains("already exists", exception.Message);

        var allRoutes = await repository.GetAllAsync();
        Assert.Single(allRoutes);
    }
    
    [Fact]
    public void RT006_CreateMultipleRoutes_ShouldAllHaveUniqueIds()
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

public class MockRouteRepository : IRouteRepository
{
    private readonly Dictionary<Guid, RouteAggregate> _routes = new();

    public Task<RouteAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _routes.TryGetValue(id, out var route);
        return Task.FromResult(route);
    }

    public Task<IEnumerable<RouteAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_routes.Values.AsEnumerable());
    }

    public Task<RouteAggregate?> GetByRouteNumberAsync(string routeNumber, CancellationToken cancellationToken = default)
    {
        var route = _routes.Values.FirstOrDefault(r => r.RouteNumber.Value.ToString() == routeNumber);
        return Task.FromResult(route);
    }

    public Task AddAsync(RouteAggregate route, CancellationToken cancellationToken = default)
    {
        if (_routes.Values.Any(r => r.RouteNumber.Value == route.RouteNumber.Value))
        {
            throw new DomainException($"Route with number {route.RouteNumber.Value} already exists");
        }

        _routes[route.Id] = route;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RouteAggregate route, CancellationToken cancellationToken = default)
    {
        if (!_routes.ContainsKey(route.Id))
        {
            throw new DomainException($"Route with ID {route.Id} not found");
        }

        _routes[route.Id] = route;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_routes.ContainsKey(id))
        {
            throw new DomainException($"Route with ID {id} not found");
        }

        _routes.Remove(id);
        return Task.CompletedTask;
    }
}
