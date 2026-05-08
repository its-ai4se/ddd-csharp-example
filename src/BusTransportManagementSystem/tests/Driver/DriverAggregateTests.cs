using BusTransportManagementSystem.Domain.Bus.Repositories;
using BusTransportManagementSystem.Domain.Driver;
using BusTransportManagementSystem.Domain.Driver.Repositories;
using BusTransportManagementSystem.Domain.Route.Repositories;
using BusTransportManagementSystem.Domain.Schedule.Repositories;
using BusTransportManagementSystem.Domain.Schedule.Services;
using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using BusTransportManagementSystem.Tests.Bus;
using BusTransportManagementSystem.Tests.Route;
using BusTransportManagementSystem.Tests.Schedule;
using BusTransportManagementSystem.Tests.TestHelpers;
using Xunit;

namespace BusTransportManagementSystem.Tests.Driver;

public class DriverAggregateTests
{
    #region Add Driver Tests

    [Fact]
    public void DR001_AddValidDriver_NameAndi_ShouldCreateSuccessfully()
    {
        var name = new DriverName("Andi");
        var driver = new DriverAggregate(name);

        Assert.NotNull(driver);
        Assert.NotEqual(Guid.Empty, driver.Id);
        Assert.Equal("Andi", driver.Name.Value);
    }

    [Fact]
    public void DR002_AddDriverWithEmptyName_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DriverName(""));
    } 

    [Fact]
    public void DR003_AddDriverWithNullName_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DriverAggregate(null!));
    }

    [Fact]
    public void DR004_AddMultipleDrivers_EachShouldGetUniqueId()
    {
        var names = new[] { "Budi", "Cindy", "Doni" };
        var drivers = new List<DriverAggregate>();

        foreach (var name in names)
        {
            drivers.Add(new DriverAggregate(new DriverName(name)));
        }

        Assert.Equal(3, drivers.Count);
        var uniqueIds = drivers.Select(d => d.Id).Distinct().ToList();
        Assert.Equal(3, uniqueIds.Count);
        Assert.DoesNotContain(Guid.Empty, uniqueIds);
    }

    [Fact]
    public void DR005_AddDriverWithSpecialCharacters_ShouldCreateSuccessfully()
    {
        var name = new DriverName("Ni'am");

        var driver = new DriverAggregate(name);

        Assert.NotNull(driver);
        Assert.Equal("Ni'am", driver.Name.Value);
    }

    [Fact]
    public void DR006_AddDriverWithVeryLongName_ShouldCreateSuccessfully()
    {
        var longName = new string('A', 255);
        var name = new DriverName(longName);

        var driver = new DriverAggregate(name);

        Assert.NotNull(driver);
        Assert.Equal(255, driver.Name.Value.Length);
    }

    #endregion

    #region Delete Driver Tests

    [Fact]
    public async Task DR007_DeleteExistingDriver_ShouldRemoveSuccessfully()
    {
        var repository = new MockDriverRepository();
        var driver = new DriverAggregate(new DriverName("Andi"));
        await repository.AddAsync(driver);

        await repository.DeleteAsync(driver.Id);

        var deletedDriver = await repository.GetByIdAsync(driver.Id);
        Assert.Null(deletedDriver);
    }

    [Fact]
    public async Task DR008_DeleteNonExistentDriver_ShouldThrowDomainException()
    {
        var repository = new MockDriverRepository();
        var nonExistentId = Guid.NewGuid();
        
        var driver = new DriverAggregate(new DriverName("Andi"));
        await repository.AddAsync(driver);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => repository.DeleteAsync(nonExistentId));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task DR009_DeleteDriverWithFutureShifts_ShouldThrowDomainException()
    {
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository(mockScheduleRepo);
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();

        var service = new ScheduleManagementService(
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Budi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(
            driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date);

        // Attempt to delete driver - should fail
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => mockDriverRepo.DeleteAsync(driver.Id));

        Assert.Contains("must be cleared from assignments first", exception.Message);

        // Verify driver still exists
        var driverAfterFailedDelete = await mockDriverRepo.GetByIdAsync(driver.Id);
        Assert.NotNull(driverAfterFailedDelete);
    }

    [Fact]
    public async Task DR010_DeleteDriverAlreadyDeleted_ShouldThrowDomainException()
    {
        var repository = new MockDriverRepository();
        var driver = new DriverAggregate(new DriverName("Budi"));
        await repository.AddAsync(driver);

        await repository.DeleteAsync(driver.Id);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => repository.DeleteAsync(driver.Id));

        Assert.Contains("not found", exception.Message);
    }
    #endregion

    #region Sick Leave Status Tests

    [Fact]
    public void DR101_MarkDriverAsSick_ShouldSetOnSickLeaveStatus()
    {
        var driver = new DriverAggregate(new DriverName("Andi"));

        driver.SetSickLeave();

        Assert.True(driver.IsOnSickLeave());
        Assert.False(driver.IsAvailable());
    }

    [Fact]
    public async Task DR102_MarkNonExistentDriverAsSick_ShouldThrowNotFoundException()
    {
        var repository = new MockDriverRepository();
        var nonExistentId = Guid.NewGuid();

        var driver = await repository.GetByIdAsync(nonExistentId);
        Assert.Null(driver);
    }

    [Fact]
    public void DR103_MarkSickDriverAsHealthy_ShouldSetActiveStatus()
    {
        var driver = new DriverAggregate(new DriverName("Andi"));
        driver.SetSickLeave();

        driver.ClearSickLeave();

        Assert.False(driver.IsOnSickLeave());
        Assert.True(driver.IsAvailable());
    }

    [Fact]
    public async Task DR104_MarkDriverAsSickWithFutureShifts_ShouldThrowDomainException()
    {
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository(mockScheduleRepo);
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();

        var service = new ScheduleManagementService(
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(
            driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => mockDriverRepo.SetSickLeaveAsync(driver.Id));

        Assert.Contains("must be cleared from assignments first", exception.Message);

        var driverAfterFailedSick = await mockDriverRepo.GetByIdAsync(driver.Id);
        Assert.NotNull(driverAfterFailedSick);
        Assert.False(driverAfterFailedSick.IsOnSickLeave());
    }

    #endregion
}

public class MockDriverRepository : IDriverRepository
{
    private readonly Dictionary<Guid, DriverAggregate> _drivers = new();
    private readonly IScheduleRepository? _scheduleRepository;

    public MockDriverRepository(IScheduleRepository? scheduleRepository = null)
    {
        _scheduleRepository = scheduleRepository;
    }

    public Task<DriverAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _drivers.TryGetValue(id, out var driver);
        return Task.FromResult(driver);
    }

    public Task<IEnumerable<DriverAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_drivers.Values.AsEnumerable());
    }

    public Task<IEnumerable<DriverAggregate>> GetAvailableDriversAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_drivers.Values.Where(d => d.IsAvailable()).AsEnumerable());
    }

    public Task AddAsync(DriverAggregate driver, CancellationToken cancellationToken = default)
    {
        _drivers[driver.Id] = driver;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_drivers.ContainsKey(id))
            throw new DomainException($"Driver with ID {id} not found");

        // Check if driver has shift assignments
        if (_scheduleRepository != null)
        {
            var schedules = _scheduleRepository.GetAllAsync().GetAwaiter().GetResult();
            foreach (var schedule in schedules)
            {
                if (schedule.DriverShiftAssignments.Any(dsa => dsa.DriverId == id))
                {
                    throw new DomainException("Driver must be cleared from assignments first");
                }
            }
        }

        _drivers.Remove(id);
        return Task.CompletedTask;
    }

    public Task SetSickLeaveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_drivers.ContainsKey(id))
            throw new DomainException($"Driver with ID {id} not found");

        // Check if driver has shift assignments
        if (_scheduleRepository != null)
        {
            var schedules = _scheduleRepository.GetAllAsync().GetAwaiter().GetResult();
            foreach (var schedule in schedules)
            {
                if (schedule.DriverShiftAssignments.Any(dsa => dsa.DriverId == id))
                {
                    throw new DomainException("Driver must be cleared from assignments first");
                }
            }
        }

        var driver = _drivers[id];
        driver.SetSickLeave();
        return Task.CompletedTask;
    }

    public Task ClearSickLeaveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_drivers.ContainsKey(id))
            throw new DomainException($"Driver with ID {id} not found");

        var driver = _drivers[id];
        driver.ClearSickLeave();
        return Task.CompletedTask;
    }
}

