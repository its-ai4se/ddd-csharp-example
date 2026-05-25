using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using Xunit;

namespace SmartHomeAutomationSystem.Domain.Tests;

public class SensorOperationTests
{
    private static DeviceAggregate CreateActiveSensor()
    {
        var sensor = new DeviceAggregate(new DeviceName("TempSensor"), new DeviceType("TemperatureSensor"), Guid.NewGuid());
        sensor.Activate();
        return sensor;
    }

    private static DeviceAggregate CreateInactiveSensor()
        => new(new DeviceName("TempSensor"), new DeviceType("TemperatureSensor"), Guid.NewGuid());

    [Fact]
    public void SO001_ActiveSensor_GeneratesReadingWithValueAndTimestamp()
    {
        var sensor = CreateActiveSensor();
        var timestamp = DateTime.UtcNow;
        var reading = sensor.GenerateReading(22.5, "°C", timestamp);
        Assert.Equal(22.5, reading.Value);
        Assert.Equal(timestamp, reading.Timestamp);
    }

    [Fact]
    public void SO002_InactiveSensor_CannotGenerateReading()
    {
        var sensor = CreateInactiveSensor();
        var ex = Assert.Throws<DomainException>(() => sensor.GenerateReading(22.5, "°C", DateTime.UtcNow));
        Assert.Contains("not active", ex.Message);
    }

    [Fact]
    public void SO003_DeactivatedSensor_CannotGenerateReading()
    {
        var sensor = CreateActiveSensor();
        sensor.Deactivate();
        var ex = Assert.Throws<DomainException>(() => sensor.GenerateReading(22.5, "°C", DateTime.UtcNow));
        Assert.Contains("not active", ex.Message);
    }

    [Fact]
    public void SO004_NullMeasuredValue_ThrowsDomainException()
    {
        var sensor = CreateActiveSensor();
        var ex = Assert.Throws<DomainException>(() => sensor.GenerateReading(null, "°C", DateTime.UtcNow));
        Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SO005_NullTimestamp_ThrowsDomainException()
    {
        var sensor = CreateActiveSensor();
        var ex = Assert.Throws<DomainException>(() => sensor.GenerateReading(22.5, "°C", null));
        Assert.Contains("null", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
