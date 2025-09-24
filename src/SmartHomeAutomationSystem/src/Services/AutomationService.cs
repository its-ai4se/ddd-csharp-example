using SmartHomeAutomationSystem.Domain.Shared.Services;
using SmartHomeAutomationSystem.Domain.Automation;
using SmartHomeAutomationSystem.Domain.Automation.Repositories;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Services;

public class AutomationService : DomainServiceBase
{
    private readonly IAutomationRuleRepository _automationRuleRepository;

    public AutomationService(
        IClock clock,
        IAutomationRuleRepository automationRuleRepository) : base(clock)
    {
        _automationRuleRepository = automationRuleRepository ?? throw new ArgumentNullException(nameof(automationRuleRepository));
    }

    public async Task<AutomationRuleAggregate> CreateRuleAsync(
        AutomationRuleName name,
        Guid homeId,
        Guid createdByUserId)
    {
        var rule = new AutomationRuleAggregate(name, homeId, createdByUserId);
        await _automationRuleRepository.SaveAsync(rule);
        return rule;
    }

    public async Task AddTimeTriggerAsync(
        Guid ruleId,
        TimeSpan time,
        List<DayOfWeek> days)
    {
        var rule = await _automationRuleRepository.GetByIdAsync(ruleId);
        if (rule == null)
            throw new DomainException("Automation rule not found.");

        var trigger = new TimeTrigger(ruleId, time, days);
        rule.AddTrigger(trigger);
        await _automationRuleRepository.SaveAsync(rule);
    }

    public async Task AddDeviceActionAsync(
        Guid ruleId,
        Guid deviceId,
        string command,
        Dictionary<string, object>? parameters = null)
    {
        var rule = await _automationRuleRepository.GetByIdAsync(ruleId);
        if (rule == null)
            throw new DomainException("Automation rule not found.");

        var action = new DeviceAction(ruleId, deviceId, command, parameters);
        rule.AddAction(action);
        await _automationRuleRepository.SaveAsync(rule);
    }

    public async Task EnableRuleAsync(Guid ruleId)
    {
        var rule = await _automationRuleRepository.GetByIdAsync(ruleId);
        if (rule == null)
            throw new DomainException("Automation rule not found.");

        rule.Enable();
        await _automationRuleRepository.SaveAsync(rule);
    }

    public async Task DisableRuleAsync(Guid ruleId)
    {
        var rule = await _automationRuleRepository.GetByIdAsync(ruleId);
        if (rule == null)
            throw new DomainException("Automation rule not found.");

        rule.Disable();
        await _automationRuleRepository.SaveAsync(rule);
    }

    public async Task ExecuteRulesAsync(Guid homeId)
    {
        var rules = await _automationRuleRepository.GetByHomeIdAsync(homeId);
        var executableRules = rules.Where(r => r.CanExecute()).ToList();

        foreach (var rule in executableRules)
        {
            foreach (var action in rule.Actions)
            {
                action.Execute();
            }
            
            rule.MarkAsExecuted();
            await _automationRuleRepository.SaveAsync(rule);
        }
    }

    public async Task<List<AutomationRuleAggregate>> GetRulesByHomeAsync(Guid homeId)
    {
        return await _automationRuleRepository.GetByHomeIdAsync(homeId);
    }
}
