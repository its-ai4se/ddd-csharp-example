using BusTransportManagementSystem.Domain.Bus;
using BusTransportManagementSystem.Domain.Bus.Repositories;
using BusTransportManagementSystem.Domain.Driver;
using BusTransportManagementSystem.Domain.Driver.Repositories;
using BusTransportManagementSystem.Domain.Route;
using BusTransportManagementSystem.Domain.Route.Repositories;
using BusTransportManagementSystem.Domain.Schedule;
using BusTransportManagementSystem.Domain.Schedule.Repositories;
using BusTransportManagementSystem.Domain.Schedule.Services;
using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
using BusTransportManagementSystem.Tests.Bus;
using BusTransportManagementSystem.Tests.Driver;
using BusTransportManagementSystem.Tests.Route;
using BusTransportManagementSystem.Tests.Schedule;
using BusTransportManagementSystem.Tests.TestHelpers;
using Xunit;

namespace BusTransportManagementSystem.Tests.Schedule;

public class ScheduleAggregateTests
{
    #region Bus-to-Route Assignment Tests

    [Fact]
    public async Task BA001_AssignBusToRouteToday_ShouldCreateAssignmentSuccessfully()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Single(schedule.BusRouteAssignments);
        Assert.Equal(bus.Id, schedule.BusRouteAssignments[0].BusId);
        Assert.Equal(route.Id, schedule.BusRouteAssignments[0].RouteId);
        Assert.Equal(date, schedule.BusRouteAssignments[0].Date);
    }

    [Fact]
    public async Task BA002_AssignBusToRouteWithNullDate_ShouldThrowArgumentNullException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.AssignBusToRouteAsync(bus.Id, route.Id, null!));
    }

    [Fact]
    public async Task BA003_AssignBusToRoute1YearAhead_ShouldCreateAssignmentSuccessfully()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Exactly365DaysAhead;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Single(schedule.BusRouteAssignments);
        Assert.Equal(date, schedule.BusRouteAssignments[0].Date);
    }

    [Fact]
    public async Task BA004_AssignBusToRouteMoreThan1YearAhead_ShouldThrowInvalidOperationException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.MoreThan1YearAhead;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignBusToRouteAsync(bus.Id, route.Id, date));
    }

    [Fact]
    public async Task BA005_AssignBusToRouteInPast_ShouldThrowInvalidOperationException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Yesterday;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignBusToRouteAsync(bus.Id, route.Id, date));
    }

    [Fact]
    public async Task BA006_AssignMultipleBusesToSameRouteSameDay_ShouldCreateAllAssignments()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var buses = new[]
        {
            TestDataFactory.CreateBus("ABC123"),
            TestDataFactory.CreateBus("DEF456"),
            TestDataFactory.CreateBus("GHI789")
        };
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        foreach (var bus in buses)
        {
            await mockBusRepo.AddAsync(bus);
        }
        await mockRouteRepo.AddAsync(route);

        foreach (var bus in buses)
        {
            await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        }

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Equal(3, schedule.BusRouteAssignments.Count);
    }

    [Fact]
    public async Task BA007_AssignSameBusToRouteTwiceSameDay_ShouldThrowInvalidOperationException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignBusToRouteAsync(bus.Id, route.Id, date));
    }

    [Fact]
    public async Task BA008_AssignBusToDifferentRoutesSameDay_ShouldThrowInvalidOperationException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route1 = TestDataFactory.CreateRoute(1);
        var route2 = TestDataFactory.CreateRoute(2);
        var date = TestDataFactory.Dates.Today;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route1);
        await mockRouteRepo.AddAsync(route2);

        await service.AssignBusToRouteAsync(bus.Id, route1.Id, date);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignBusToRouteAsync(bus.Id, route2.Id, date));
    }

    [Fact]
    public async Task BA009_AssignBusToDifferentRoutesDifferentDays_ShouldCreateBothAssignments()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route1 = TestDataFactory.CreateRoute(1);
        var route2 = TestDataFactory.CreateRoute(2);
        var date1 = TestDataFactory.Dates.Today;
        var date2 = TestDataFactory.Dates.Tomorrow;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route1);
        await mockRouteRepo.AddAsync(route2);

        await service.AssignBusToRouteAsync(bus.Id, route1.Id, date1);
        await service.AssignBusToRouteAsync(bus.Id, route2.Id, date2);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Equal(2, schedule.BusRouteAssignments.Count);
    }

    [Fact]
    public async Task BA010_AssignNonExistentBus_ShouldThrowException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockRouteRepo.AddAsync(route);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.AssignBusToRouteAsync(Guid.NewGuid(), route.Id, date));
    }

    [Fact]
    public async Task BA011_AssignToNonExistentRoute_ShouldThrowException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var date = TestDataFactory.Dates.Today;

        await mockBusRepo.AddAsync(bus);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.AssignBusToRouteAsync(bus.Id, Guid.NewGuid(), date));
    }

    [Fact]
    public async Task BA012_AssignBusInRepairShop_ShouldThrowInvalidOperationException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        bus.SetUnderRepair();
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignBusToRouteAsync(bus.Id, route.Id, date));
    }

    #endregion

    #region Driver Scheduling Tests

    [Fact]
    public async Task DS001_ScheduleDriverToShiftToday_ShouldCreateScheduleSuccessfully()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, shift, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Single(schedule.DriverShiftAssignments);
        Assert.Equal(driver.Id, schedule.DriverShiftAssignments[0].DriverId);
        Assert.Equal(shift, schedule.DriverShiftAssignments[0].ShiftPeriod);
    }

    [Fact]
    public async Task DS002_ScheduleDriverToShift1YearAhead_ShouldCreateScheduleSuccessfully()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Exactly365DaysAhead;
        var shift = TestDataFactory.Shifts.Morning;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, shift, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Single(schedule.DriverShiftAssignments);
    }

    [Fact]
    public async Task DS003_ScheduleDriverToShiftMoreThan1YearAhead_ShouldThrowInvalidOperationException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.MoreThan1YearAhead;
        var shift = TestDataFactory.Shifts.Morning;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignBusToRouteAsync(bus.Id, route.Id, date));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, shift, date));
    }

    [Fact]
    public async Task DS004_ScheduleDriverToShiftMorningShift_ShouldCreateScheduleSuccessfully()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, shift, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Single(schedule.DriverShiftAssignments);
        Assert.Equal(ShiftPeriodType.Morning, schedule.DriverShiftAssignments[0].ShiftPeriod.Value);
    }

    [Fact]
    public async Task DS005_ScheduleDriverToShiftAfternoonShift_ShouldCreateScheduleSuccessfully()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Afternoon;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, shift, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Single(schedule.DriverShiftAssignments);
        Assert.Equal(ShiftPeriodType.Afternoon, schedule.DriverShiftAssignments[0].ShiftPeriod.Value);
    }

    [Fact]
    public async Task DS006_ScheduleDriverToShiftNightShift_ShouldCreateScheduleSuccessfully()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Night;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, shift, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Single(schedule.DriverShiftAssignments);
        Assert.Equal(ShiftPeriodType.Night, schedule.DriverShiftAssignments[0].ShiftPeriod.Value);
    }

    [Fact]
    public async Task DS007_ScheduleDriverToShiftNonExistentDriver_ShouldThrowException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await Assert.ThrowsAsync<DomainException>(() =>
            service.AssignDriverToShiftAsync(Guid.NewGuid(), bus.Id, route.Id, shift, date));
    }

    [Fact]
    public async Task DS008_ScheduleDriverToShiftToNonExistentBus_ShouldThrowException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        await mockDriverRepo.AddAsync(driver);
        await mockRouteRepo.AddAsync(route);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.AssignBusToRouteAsync(Guid.NewGuid(), route.Id, date));
        await Assert.ThrowsAsync<DomainException>(() =>
            service.AssignDriverToShiftAsync(driver.Id, Guid.NewGuid(), route.Id, shift, date));
    }

    [Fact]
    public async Task DS009_ScheduleDriverToShiftToUnassignedBus_ShouldThrowInvalidOperationException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        // assign bus to route first - this should fail
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, shift, date));
    }

    [Fact]
    public async Task DS010_ScheduleDriverToShiftSickDriver_ShouldThrowInvalidOperationException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        driver.SetSickLeave();
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, shift, date));
    }

    [Fact]
    public async Task DS011_ScheduleDriverToShift2ShiftsSameDay_ShouldCreateBothSchedules()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus1 = TestDataFactory.CreateBus("ABC123");
        var bus2 = TestDataFactory.CreateBus("DEF456");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus1);
        await mockBusRepo.AddAsync(bus2);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus1.Id, route.Id, date);
        await service.AssignBusToRouteAsync(bus2.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus1.Id, route.Id, TestDataFactory.Shifts.Morning, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus2.Id, route.Id, TestDataFactory.Shifts.Afternoon, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Equal(2, schedule.DriverShiftAssignments.Count);
    }

    [Fact]
    public async Task DS012_ScheduleDriverToShift3ShiftsSameDay_ShouldCreateAllSchedules()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var bus2 = TestDataFactory.CreateBus("DEF456");
        var bus3 = TestDataFactory.CreateBus("GHI789");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockBusRepo.AddAsync(bus2);
        await mockBusRepo.AddAsync(bus3);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await service.AssignBusToRouteAsync(bus2.Id, route.Id, date);
        await service.AssignBusToRouteAsync(bus3.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus2.Id, route.Id, TestDataFactory.Shifts.Afternoon, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus3.Id, route.Id, TestDataFactory.Shifts.Night, date);

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Equal(3, schedule.DriverShiftAssignments.Count);
    }

    [Fact]
    public async Task DS013_ScheduleDriverToShiftSameShiftTwice_ShouldThrowInvalidOperationException()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus1 = TestDataFactory.CreateBus("ABC123");
        var bus2 = TestDataFactory.CreateBus("DEF456");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus1);
        await mockBusRepo.AddAsync(bus2);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus1.Id, route.Id, date);
        await service.AssignBusToRouteAsync(bus2.Id, route.Id, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus1.Id, route.Id, shift, date);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignDriverToShiftAsync(driver.Id, bus2.Id, route.Id, shift, date));

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Single(schedule.DriverShiftAssignments);
    }

    [Fact]
    public async Task DS014_ScheduleDriverToShiftOverlappingShifts_ShouldPreventDuplicate()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
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
        await service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date));

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Single(schedule.DriverShiftAssignments);
    }

    [Fact]
    public async Task DS015_ScheduleDriverTo5ShiftsSameDay_ShouldPreventExceedingMaxShifts()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var driver = TestDataFactory.CreateDriver("Andi");
        var bus = TestDataFactory.CreateBus("ABC123");
        var bus2 = TestDataFactory.CreateBus("DEF456");
        var bus3 = TestDataFactory.CreateBus("GHI789");
        var bus4 = TestDataFactory.CreateBus("JKL012");
        var bus5 = TestDataFactory.CreateBus("MNO345");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;

        await mockDriverRepo.AddAsync(driver);
        await mockBusRepo.AddAsync(bus);
        await mockBusRepo.AddAsync(bus2);
        await mockBusRepo.AddAsync(bus3);
        await mockBusRepo.AddAsync(bus4);
        await mockBusRepo.AddAsync(bus5);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);
        await service.AssignBusToRouteAsync(bus2.Id, route.Id, date);
        await service.AssignBusToRouteAsync(bus3.Id, route.Id, date);
        await service.AssignBusToRouteAsync(bus4.Id, route.Id, date);
        await service.AssignBusToRouteAsync(bus5.Id, route.Id, date);

        await service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, TestDataFactory.Shifts.Morning, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus2.Id, route.Id, TestDataFactory.Shifts.Afternoon, date);
        await service.AssignDriverToShiftAsync(driver.Id, bus3.Id, route.Id, TestDataFactory.Shifts.Night, date);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignDriverToShiftAsync(driver.Id, bus4.Id, route.Id, TestDataFactory.Shifts.Morning, date));

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Equal(3, schedule.DriverShiftAssignments.Count);
    }

    [Fact]
    public async Task DS016_ScheduleMultipleDriversToShiftSameShift_ShouldCreateAllSchedules()
    {
        var mockBusRepo = new MockBusRepository();
        var mockRouteRepo = new MockRouteRepository();
        var mockScheduleRepo = new MockScheduleRepository();
        var mockDriverRepo = new MockDriverRepository();
        var mockClock = new MockClock();

        var service = new ScheduleManagementService(
            mockClock,
            mockScheduleRepo,
            mockBusRepo,
            mockRouteRepo,
            mockDriverRepo
        );

        var drivers = new[]
        {
            TestDataFactory.CreateDriver("Andi"),
            TestDataFactory.CreateDriver("Jane Smith"),
            TestDataFactory.CreateDriver("Bob Johnson")
        };
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var date = TestDataFactory.Dates.Today;
        var shift = TestDataFactory.Shifts.Morning;

        foreach (var driver in drivers)
        {
            await mockDriverRepo.AddAsync(driver);
        }
        await mockBusRepo.AddAsync(bus);
        await mockRouteRepo.AddAsync(route);

        await service.AssignBusToRouteAsync(bus.Id, route.Id, date);

        foreach (var driver in drivers)
        {
            await service.AssignDriverToShiftAsync(driver.Id, bus.Id, route.Id, shift, date);
        }

        var schedules = await mockScheduleRepo.GetAllAsync();
        var schedule = schedules.First();

        Assert.Equal(3, schedule.DriverShiftAssignments.Count);
    }

    #endregion
}

public class MockScheduleRepository : IScheduleRepository
{
    private readonly Dictionary<Guid, ScheduleAggregate> _schedules = new();

    public Task<ScheduleAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _schedules.TryGetValue(id, out var schedule);
        return Task.FromResult(schedule);
    }

    public Task<IEnumerable<ScheduleAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_schedules.Values.AsEnumerable());
    }

    public Task<IEnumerable<ScheduleAggregate>> GetByDateAsync(ScheduledDate date, CancellationToken cancellationToken = default)
    {
        var schedules = _schedules.Values
            .Where(s => s.BusRouteAssignments.Any(bra => bra.Date.Equals(date)) ||
                        s.DriverShiftAssignments.Any(dsa => dsa.Date.Equals(date)));
        return Task.FromResult(schedules.AsEnumerable());
    }

    public Task AddAsync(ScheduleAggregate schedule, CancellationToken cancellationToken = default)
    {
        _schedules[schedule.Id] = schedule;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ScheduleAggregate schedule, CancellationToken cancellationToken = default)
    {
        if (!_schedules.ContainsKey(schedule.Id))
            throw new DomainException($"Schedule with ID {schedule.Id} not found");
        _schedules[schedule.Id] = schedule;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_schedules.ContainsKey(id))
            throw new DomainException($"Schedule with ID {id} not found");
        _schedules.Remove(id);
        return Task.CompletedTask;
    }
}
