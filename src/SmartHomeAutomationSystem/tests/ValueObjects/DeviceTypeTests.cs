using Xunit;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Tests.ValueObjects;

public class DeviceTypeTests
{
    [Theory]
    [InlineData("Light")]
    [InlineData("Thermostat")]
    [InlineData("DoorLock")]
    [InlineData("SecurityCamera")]
    [InlineData("MotionSensor")]
    [InlineData("SmokeDetector")]
    [InlineData("WindowSensor")]
    [InlineData("SmartPlug")]
    [InlineData("Speaker")]
    [InlineData("Blinds")]
    public void DeviceType_WithValidType_ShouldCreateSuccessfully(string deviceType)
    {
        // Arrange & Act
        var type = new DeviceType(deviceType);

        // Assert
        Assert.Equal(deviceType, type.Value);
    }

    [Fact]
    public void DeviceType_WithInvalidType_ShouldThrowDomainException()
    {
        // Arrange, Act & Assert
        Assert.Throws<DomainException>(() => new DeviceType("InvalidType"));
    }

    [Fact]
    public void DeviceType_WithEmptyValue_ShouldThrowDomainException()
    {
        // Arrange, Act & Assert
        Assert.Throws<DomainException>(() => new DeviceType(""));
    }
}
