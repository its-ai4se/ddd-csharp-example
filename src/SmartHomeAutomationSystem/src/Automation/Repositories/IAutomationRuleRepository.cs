namespace SmartHomeAutomationSystem.Domain.Automation.Repositories;

public interface IAutomationRuleRepository
{
    Task<AutomationRuleAggregate?> GetByIdAsync(Guid id);
    Task<List<AutomationRuleAggregate>> GetByHomeIdAsync(Guid homeId);
    Task SaveAsync(AutomationRuleAggregate rule);
}
