using BusTransportManagementSystem.Domain.Bus;
using BusTransportManagementSystem.Domain.Bus.Repositories;
using BusTransportManagementSystem.Domain.Driver.Repositories;
using BusTransportManagementSystem.Domain.Route.Repositories;
using BusTransportManagementSystem.Domain.Schedule;
using BusTransportManagementSystem.Domain.Schedule.Repositories;
using BusTransportManagementSystem.Domain.Schedule.Services;
using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using BusTransportManagementSystem.Tests.Driver;
using BusTransportManagementSystem.Tests.Route;
using BusTransportManagementSystem.Tests.Schedule;
using BusTransportManagementSystem.Tests.TestHelpers;
using Xunit;

namespace BusTransportManagementSystem.Tests.Bus;

public class BusAggregateTests
{
    #region Add Bus Tests

    [Fact]
    public void BUS001_AddBusWith1CharacterPlate_ShouldCreateSuccessfully()
    {
        var plate = new LicensePlate("A");
        var bus = new BusAggregate(plate);

        Assert.NotNull(bus);
        Assert.Equal("A", bus.LicensePlate.Value);
        Assert.NotEqual(Guid.Empty, bus.Id);
        Assert.True(bus.IsOperational());
    }

    [Fact]
    public void BUS002_AddBusWith10CharacterPlate_ShouldCreateSuccessfully()
    {
        var plate = new LicensePlate("ABCDE12345");
        var bus = new BusAggregate(plate);

        Assert.NotNull(bus);
        Assert.Equal("ABCDE12345", bus.LicensePlate.Value);
        Assert.Equal(10, bus.LicensePlate.Value.Length);
    }

    [Fact]
    public void BUS003_AddBusWith0CharacterPlate_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LicensePlate(""));
    }

    [Fact]
    public void BUS004_AddBusWith11CharacterPlate_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LicensePlate("ABCDE123456"));
    }

    [Fact]
    public async Task BUS005_AddBusWithDuplicatePlate_ShouldPreventDuplicate()
    {
        var repository = new MockBusRepository();
        var plate = new LicensePlate("ABC123");

        var bus = new BusAggregate(plate);
        await repository.AddAsync(bus);

        var duplicateBus = new BusAggregate(plate);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => repository.AddAsync(duplicateBus));

        Assert.Contains("already exists", exception.Message);

        var allBuses = await repository.GetAllAsync();
        Assert.Single(allBuses);
    }

    [Fact]
    public void BUS006_AddBusWithSpecialCharacters_ShouldCreateSuccessfully()
    {
        var plate = new LicensePlate("ABC-123");
        var bus = new BusAggregate(plate);
        
        Assert.NotNull(bus);
        Assert.Equal("ABC-123", bus.LicensePlate.Value);
    }

    [Fact]
    public void BUS007_AddBusWithNullPlate_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BusAggregate((LicensePlate)null!));
    }

    [Fact]
    public void BUS008_AddMultipleBuses_EachShouldHaveUniquePlate()
    {
        var plates = new[] { "ABC123", "DEF456", "GHI789" };
        var buses = new List<BusAggregate>();

        foreach (var plate in plates)
        {
            buses.Add(new BusAggregate(new LicensePlate(plate)));
        }

        Assert.Equal(3, buses.Count);
        var uniquePlates = buses.Select(b => b.LicensePlate.Value).Distinct().ToList();
        Assert.Equal(3, uniquePlates.Count);
    }

    #endregion

    #region Delete Bus Tests

    [Fact]
    public async Task BUS009_DeleteExistingBus_ShouldRemoveSuccessfully()
    {
        var repository = new MockBusRepository();
        var bus = new BusAggregate(new LicensePlate("ABC123"));
        await repository.AddAsync(bus);
        var busId = bus.Id;

        await repository.DeleteAsync(busId);

        var deletedBus = await repository.GetByIdAsync(busId);
        Assert.Null(deletedBus);
    }

    [Fact]
    public async Task BUS010_DeleteNonExistentBus_ShouldFailedToDelete()
    {
        var repository = new MockBusRepository();
        var bus = new BusAggregate(new LicensePlate("ABC123"));
        await repository.AddAsync(bus);

        var nonExistentId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => repository.DeleteAsync(nonExistentId));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task BUS011_DeleteBusWithAssignments_ShouldThrowException()
    {
        var mockScheduleRepo = new MockScheduleRepository();
        var mockBusRepo = new MockBusRepository(mockScheduleRepo);
        var mockRouteRepo = new MockRouteRepository();
        var mockClock = new MockClock();

        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var schedule = TestDataFactory.CreateSchedule();
        var futureDate = TestDataFactory.Dates.NextWeek;
        var busId = bus.Id;
        var routeId = route.Id;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);
        await mockScheduleRepo.AddAsync(schedule);

        schedule.AssignBusToRoute(busId, routeId, futureDate, bus, route);
        await mockScheduleRepo.UpdateAsync(schedule);

        Assert.True(schedule.IsBusAssignedOnDate(busId, futureDate));

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => mockBusRepo.DeleteAsync(busId));

        Assert.Contains("must be cleared from assignments first", exception.Message);

        var busAfterFailedDelete = await mockBusRepo.GetByIdAsync(busId);
        Assert.NotNull(busAfterFailedDelete);
    }

    #endregion

    #region Repair Shop Status Tests

    [Fact]
    public void BUS101_MarkBusInRepair_ShouldSetUnderRepairStatus()
    {
        var bus = new BusAggregate(new LicensePlate("ABC123"));

        bus.SetUnderRepair();

        Assert.True(bus.IsUnderRepair());
        Assert.False(bus.IsOperational());
        Assert.False(bus.IsAvailableForService());
    }

    [Fact]
    public async Task BUS102_MarkNonExistentBusInRepair_ShouldThrowNotFoundException()
    {
        var repository = new MockBusRepository();
        var bus = new BusAggregate(new LicensePlate("ABC123"));
        await repository.AddAsync(bus);

        var nonExistentId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => repository.SetUnderRepairAsync(nonExistentId));
    }

    [Fact]
    public void BUS103_MarkRepairedBusAsOperational_ShouldSetOperationalStatus()
    {
        var bus = new BusAggregate(new LicensePlate("ABC123"));
        bus.SetUnderRepair();

        bus.SetOperational();

        Assert.False(bus.IsUnderRepair());
        Assert.True(bus.IsOperational());
        Assert.True(bus.IsAvailableForService());
    }

    [Fact]
    public async Task BUS104_MarkBusInRepairWithFutureAssignments_ShouldThrowDomainException()
    {
        var mockScheduleRepo = new MockScheduleRepository();
        var mockBusRepo = new MockBusRepository(mockScheduleRepo);
        var mockRouteRepo = new MockRouteRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            new MockDriverRepository()
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);
        await mockScheduleRepo.AddAsync(new ScheduleAggregate());

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();
        Assert.True(schedule.IsBusAssignedOnDate(bus.Id, date));

        // Attempt to mark bus as under repair - should fail
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => mockBusRepo.SetUnderRepairAsync(bus.Id));

        Assert.Contains("must be cleared from assignments first", exception.Message);

        // Verify bus is still operational
        var busAfterFailedRepair = await mockBusRepo.GetByIdAsync(bus.Id);
        Assert.NotNull(busAfterFailedRepair);
        Assert.False(busAfterFailedRepair.IsUnderRepair());
        Assert.True(busAfterFailedRepair.IsOperational());
    }

    #endregion
}

public class MockBusRepository : IBusRepository
{
    private readonly Dictionary<Guid, BusAggregate> _buses = new();
    private readonly IScheduleRepository? _scheduleRepository;

    public MockBusRepository(IScheduleRepository? scheduleRepository = null)
    {
        _scheduleRepository = scheduleRepository;
    }

    public Task<BusAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _buses.TryGetValue(id, out var bus);
        return Task.FromResult(bus);
    }

    public Task<IEnumerable<BusAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_buses.Values.AsEnumerable());
    }

    public Task<IEnumerable<BusAggregate>> GetAvailableBusesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_buses.Values.Where(b => b.IsAvailableForService()).AsEnumerable());
    }

    public Task AddAsync(BusAggregate bus, CancellationToken cancellationToken = default)
    {
        if (_buses.Values.Any(b => b.LicensePlate.Value == bus.LicensePlate.Value))
        {
            throw new DomainException($"Bus with license plate {bus.LicensePlate.Value} already exists");
        }

        _buses[bus.Id] = bus;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_buses.ContainsKey(id))
            throw new DomainException($"Bus with ID {id} not found");

        // Check for active assignments if schedule repository is provided
        if (_scheduleRepository != null)
        {
            var schedules = _scheduleRepository.GetAllAsync(cancellationToken).Result;
            bool hasAssignments = schedules.Any(s =>
                s.BusRouteAssignments.Any(bra => bra.BusId == id) ||
                s.DriverShiftAssignments.Any(dsa => dsa.BusId == id));

            if (hasAssignments)
            {
                throw new DomainException("Bus must be cleared from assignments first");
            }
        }

        _buses.Remove(id);
        return Task.CompletedTask;
    }

    public Task SetUnderRepairAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_buses.ContainsKey(id))
            throw new DomainException($"Bus with ID {id} not found");

        // Check if bus has route assignments
        if (_scheduleRepository != null)
        {
            var schedules = _scheduleRepository.GetAllAsync(cancellationToken).GetAwaiter().GetResult();
            foreach (var schedule in schedules)
            {
                if (schedule.BusRouteAssignments.Any(bra => bra.BusId == id))
                {
                    throw new DomainException("Bus must be cleared from assignments first");
                }
            }
        }

        var bus = _buses[id];
        bus.SetUnderRepair();
        return Task.CompletedTask;
    }

    public Task SetOperationalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_buses.ContainsKey(id))
            throw new DomainException($"Bus with ID {id} not found");

        var bus = _buses[id];
        bus.SetOperational();
        return Task.CompletedTask;
    }
}
