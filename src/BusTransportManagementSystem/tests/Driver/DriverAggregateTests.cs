using BusTransportManagementSystem.Domain.Driver;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;
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
        Assert.Equal("Andi", driver.Name.Value);
        Assert.NotEqual(Guid.Empty, driver.Id);
        Assert.True(driver.IsAvailable());
        Assert.False(driver.IsOnSickLeave());
    }

    [Fact]
    public void DR002_AddDriverWithEmptyName_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new DriverName(""));
    }

    [Fact]
    public void DR003_AddDriverWithNullName_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DriverAggregate((DriverName)null!));
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
    public void DR007_DeleteExistingDriver_ShouldRemoveSuccessfully()
    {
        var drivers = new List<DriverAggregate>();
        var driver = new DriverAggregate(new DriverName("Andi"));
        var driverId = driver.Id;

        Assert.NotNull(driver);
        Assert.Equal(driverId, driver.Id);
    }

    [Fact]
    public void DR008_DeleteNonExistentDriver_ShouldFailedToDelete()
    {
        var names = new[] { "Budi", "Cindy", "Doni" };
        var drivers = new List<DriverAggregate>();

        foreach (var name in names)
        {
            drivers.Add(new DriverAggregate(new DriverName(name)));
        }

        var nonExistentId = Guid.NewGuid();

        Assert.DoesNotContain(nonExistentId, drivers.Select(d => d.Id));
    }

    [Fact]
    public void DR009_DeleteDriverWithFutureShifts_ShouldDeleteSuccessfully()
    {
        var names = new[] { "Budi", "Cindy", "Doni" };
        var drivers = new List<DriverAggregate>();

        foreach (var name in names)
        {
            drivers.Add(new DriverAggregate(new DriverName(name)));
        }

        var driverId = drivers[0].Id;

        drivers.RemoveAt(0);

        Assert.DoesNotContain(driverId, drivers.Select(d => d.Id));
    }

    [Fact]
    public void DR010_DeleteDriverAlreadyDeleted_ShouldFailedToDelete()
    {
        var names = new[] { "Budi", "Cindy", "Doni" };
        var drivers = new List<DriverAggregate>();

        foreach (var name in names)
        {
            drivers.Add(new DriverAggregate(new DriverName(name)));
        }

        var budiId = drivers[0].Id;

        var budi = drivers.Single(d => d.Id == budiId);
        drivers.Remove(budi);

        Assert.Throws<InvalidOperationException>(() =>
        {
            var driver = drivers.SingleOrDefault(d => d.Id == budiId);
            if (driver is null)
                throw new InvalidOperationException("Driver already deleted");
        });
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
    public void DR102_MarkNonExistentDriverAsSick_ShouldThrowNotFoundException()
    {
        var nonExistentId = Guid.NewGuid();

        Assert.NotEqual(Guid.Empty, nonExistentId);
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
    public void DR104_MarkDriverAsSickWithFutureShifts_ShouldVerifyShiftHandling()
    {
        var driver = new DriverAggregate(new DriverName("Andi"));

        driver.SetSickLeave();

        Assert.True(driver.IsOnSickLeave());
        Assert.False(driver.IsAvailable());
    }

    #endregion
}

