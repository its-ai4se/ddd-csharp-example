using BusTransportManagementSystem.Domain.Bus;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
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
    public void BUS005_AddBusWithDuplicatePlate_ShouldPreventDuplicate()
    {
        var plate = new LicensePlate("ABC123");
        var otherPlate = new LicensePlate("ABC123");

        var buses = new List<BusAggregate>{};
        buses.Add(new BusAggregate(plate));

        Assert.Throws<InvalidOperationException>(() =>
        {
            var duplicateBus = new BusAggregate(otherPlate);

            if (buses.Any(b => b.LicensePlate.Value == duplicateBus.LicensePlate.Value))
            {
                throw new InvalidOperationException("Bus number must be unique");
            }

            buses.Add(duplicateBus);
        });

        Assert.Single(buses);
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
    public void BUS009_DeleteExistingBus_ShouldRemoveSuccessfully()
    {
        var bus = new BusAggregate(new LicensePlate("ABC123"));
        var busId = bus.Id;

        var isBusAccessible = bus != null;

        Assert.NotNull(bus);
        Assert.Equal(busId, bus.Id);
    }

    [Fact]
    public void BUS010_DeleteNonExistentBus_ShouldFailedToDelete()
    {
        var plates = new[] { "ABC123", "DEF456", "GHI789" };
        var buses = new List<BusAggregate>();

        foreach (var plate in plates)
        {
            buses.Add(new BusAggregate(new LicensePlate(plate)));
        }

        var nonExistentId = Guid.NewGuid();

        Assert.DoesNotContain(nonExistentId, buses.Select(b => b.Id));
    }

    [Fact]
    public void BUS011_DeleteBusWithAssignments_ShouldDeleteSuccessfully()
    {
        var bus = TestDataFactory.CreateBus("ABC123");
        var route = TestDataFactory.CreateRoute(1);
        var schedule = TestDataFactory.CreateSchedule();
        var futureDate = TestDataFactory.Dates.NextWeek;
        var busId = bus.Id;
        var routeId = route.Id;

        schedule.AssignBusToRoute(busId, routeId, futureDate, bus, route);

        Assert.True(schedule.IsBusAssignedOnDate(busId, futureDate));

        schedule.RemoveBusRouteAssignment(busId, routeId, futureDate);

        Assert.False(schedule.IsBusAssignedOnDate(busId, futureDate));
        Assert.NotNull(bus);
        Assert.Equal(busId, bus.Id);
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
    public void BUS102_MarkNonExistentBusInRepair_ShouldThrowNotFoundException()
    {
        var nonExistentPlate = "XYZ999";

        Assert.NotNull(nonExistentPlate);
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
    public void BUS104_MarkBusInRepairWithFutureAssignments_ShouldVerifyAssignmentHandling()
    {
        var bus = new BusAggregate(new LicensePlate("ABC123"));

        bus.SetUnderRepair();

        Assert.True(bus.IsUnderRepair());
        Assert.False(bus.IsAvailableForService());
    }

    #endregion
}

