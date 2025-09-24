using Xunit;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Tests.ValueObjects;

public class DeviceNameTests
{
    [Fact]
    public void DeviceName_WithValidValue_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var deviceName = new DeviceName("Smart Light");

        // Assert
        Assert.Equal("Smart Light", deviceName.Value);
    }

    [Fact]
    public void DeviceName_WithEmptyValue_ShouldThrowDomainException()
    {
        // Arrange, Act & Assert
        Assert.Throws<DomainException>(() => new DeviceName(""));
    }

    [Fact]
    public void DeviceName_WithNullValue_ShouldThrowDomainException()
    {
        // Arrange, Act & Assert
        Assert.Throws<DomainException>(() => new DeviceName(null!));
    }

    [Fact]
    public void DeviceName_WithTooLongValue_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('A', 101);

        // Act & Assert
        Assert.Throws<DomainException>(() => new DeviceName(longName));
    }

    [Fact]
    public void DeviceName_WithWhitespace_ShouldTrimValue()
    {
        // Arrange & Act
        var deviceName = new DeviceName("  Smart Light  ");

        // Assert
        Assert.Equal("Smart Light", deviceName.Value);
    }
}
