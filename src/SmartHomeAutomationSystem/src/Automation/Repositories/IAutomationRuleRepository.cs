using SmartHomeAutomationSystem.Domain.Automation;

namespace SmartHomeAutomationSystem.Domain.Automation.Repositories;

public interface IAutomationRuleRepository
{
    Task<AutomationRuleAggregate?> GetByIdAsync(Guid id);
    Task<List<AutomationRuleAggregate>> GetByHomeIdAsync(Guid homeId);
    Task<List<AutomationRuleAggregate>> GetByUserIdAsync(Guid userId);
    Task<List<AutomationRuleAggregate>> GetAllAsync();
    Task SaveAsync(AutomationRuleAggregate rule);
    Task DeleteAsync(Guid id);
}
