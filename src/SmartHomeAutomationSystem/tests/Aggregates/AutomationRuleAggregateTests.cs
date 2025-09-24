using Xunit;
using SmartHomeAutomationSystem.Domain.Automation;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Tests.Aggregates;

public class AutomationRuleAggregateTests
{
    [Fact]
    public void AutomationRuleAggregate_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var homeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ruleName = new AutomationRuleName("Test Rule");

        // Act
        var rule = new AutomationRuleAggregate(ruleName, homeId, userId);

        // Assert
        Assert.Equal(ruleName.Value, rule.Name.Value);
        Assert.Equal(homeId, rule.HomeId);
        Assert.Equal(userId, rule.CreatedByUserId);
        Assert.True(rule.IsEnabled);
        Assert.Empty(rule.Triggers);
        Assert.Empty(rule.Actions);
    }

    [Fact]
    public void AutomationRuleAggregate_WithEmptyHomeId_ShouldThrowDomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ruleName = new AutomationRuleName("Test Rule");

        // Act & Assert
        Assert.Throws<DomainException>(() => new AutomationRuleAggregate(ruleName, Guid.Empty, userId));
    }

    [Fact]
    public void AutomationRuleAggregate_WithEmptyUserId_ShouldThrowDomainException()
    {
        // Arrange
        var homeId = Guid.NewGuid();
        var ruleName = new AutomationRuleName("Test Rule");

        // Act & Assert
        Assert.Throws<DomainException>(() => new AutomationRuleAggregate(ruleName, homeId, Guid.Empty));
    }

    [Fact]
    public void AddTrigger_WithValidTrigger_ShouldAddTrigger()
    {
        // Arrange
        var rule = CreateTestRule();
        var trigger = new TimeTrigger(rule.Id, new TimeSpan(7, 0, 0), new List<DayOfWeek> { DayOfWeek.Monday });

        // Act
        rule.AddTrigger(trigger);

        // Assert
        Assert.Contains(trigger, rule.Triggers);
    }

    [Fact]
    public void AddAction_WithValidAction_ShouldAddAction()
    {
        // Arrange
        var rule = CreateTestRule();
        var deviceId = Guid.NewGuid();
        var action = new DeviceAction(rule.Id, deviceId, "TurnOn");

        // Act
        rule.AddAction(action);

        // Assert
        Assert.Contains(action, rule.Actions);
    }

    [Fact]
    public void Enable_WithoutTriggers_ShouldThrowDomainException()
    {
        // Arrange
        var rule = CreateTestRule();

        // Act & Assert
        Assert.Throws<DomainException>(() => rule.Enable());
    }

    [Fact]
    public void Enable_WithoutActions_ShouldThrowDomainException()
    {
        // Arrange
        var rule = CreateTestRule();
        var trigger = new TimeTrigger(rule.Id, new TimeSpan(7, 0, 0), new List<DayOfWeek> { DayOfWeek.Monday });
        rule.AddTrigger(trigger);

        // Act & Assert
        Assert.Throws<DomainException>(() => rule.Enable());
    }

    [Fact]
    public void Enable_WithTriggersAndActions_ShouldEnableSuccessfully()
    {
        // Arrange
        var rule = CreateTestRule();
        var trigger = new TimeTrigger(rule.Id, new TimeSpan(7, 0, 0), new List<DayOfWeek> { DayOfWeek.Monday });
        var action = new DeviceAction(rule.Id, Guid.NewGuid(), "TurnOn");
        
        rule.AddTrigger(trigger);
        rule.AddAction(action);

        // Act
        rule.Enable();

        // Assert
        Assert.True(rule.IsEnabled);
    }

    [Fact]
    public void Disable_ShouldDisableSuccessfully()
    {
        // Arrange
        var rule = CreateTestRule();

        // Act
        rule.Disable();

        // Assert
        Assert.False(rule.IsEnabled);
    }

    [Fact]
    public void MarkAsExecuted_ShouldUpdateLastExecuted()
    {
        // Arrange
        var rule = CreateTestRule();
        var beforeExecution = DateTime.UtcNow;

        // Act
        rule.MarkAsExecuted();

        // Assert
        Assert.NotNull(rule.LastExecuted);
        Assert.True(rule.LastExecuted >= beforeExecution);
    }

    private static AutomationRuleAggregate CreateTestRule()
    {
        var homeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ruleName = new AutomationRuleName("Test Rule");
        return new AutomationRuleAggregate(ruleName, homeId, userId);
    }
}
