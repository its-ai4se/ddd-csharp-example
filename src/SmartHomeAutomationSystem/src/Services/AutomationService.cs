using SmartHomeAutomationSystem.Domain.Automation;
using SmartHomeAutomationSystem.Domain.Automation.Repositories;
using SmartHomeAutomationSystem.Domain.Automation.Precondition;
using SmartHomeAutomationSystem.Domain.Home;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Services;

public class AutomationService
{
    private readonly IAutomationRuleRepository _ruleRepository;

    public AutomationService(IAutomationRuleRepository ruleRepository)
    {
        _ruleRepository = ruleRepository ?? throw new ArgumentNullException(nameof(ruleRepository));
    }

    private static void EnsureOwner(Guid requestingUserId, HomeAggregate home)
    {
        if (!home.IsOwner(requestingUserId))
            throw new DomainException("Only the home owner may manage automation rules.");
    }

    private static void EnsureRuleBelongsToHome(AutomationRuleAggregate rule, HomeAggregate home)
    {
        if (rule.HomeId != home.Id)
            throw new DomainException("Rule does not belong to the specified home.");
    }

    public async Task<AutomationRuleAggregate> CreateRuleAsync(
        AutomationRuleName name, HomeAggregate home, Guid requestingUserId)
    {
        EnsureOwner(requestingUserId, home);
        var rule = new AutomationRuleAggregate(name, home.Id);
        await _ruleRepository.SaveAsync(rule);
        return rule;
    }

    public async Task SetPreconditionAsync(
        Guid ruleId, IPreconditionExpression precondition, HomeAggregate home, Guid requestingUserId)
    {
        EnsureOwner(requestingUserId, home);
        var rule = await GetRuleAsync(ruleId);
        EnsureRuleBelongsToHome(rule, home);
        rule.SetPrecondition(precondition);
        await _ruleRepository.SaveAsync(rule);
    }

    public async Task SetActionSequenceAsync(
        Guid ruleId, ActionSequence sequence, HomeAggregate home, Guid requestingUserId)
    {
        EnsureOwner(requestingUserId, home);
        var rule = await GetRuleAsync(ruleId);
        EnsureRuleBelongsToHome(rule, home);
        rule.SetActionSequence(sequence);
        await _ruleRepository.SaveAsync(rule);
    }

    public async Task EnableRuleAsync(Guid ruleId, HomeAggregate home, Guid requestingUserId)
    {
        EnsureOwner(requestingUserId, home);
        var rule = await GetRuleAsync(ruleId);
        EnsureRuleBelongsToHome(rule, home);
        rule.Enable();
        await _ruleRepository.SaveAsync(rule);
    }

    public async Task DisableRuleAsync(Guid ruleId, HomeAggregate home, Guid requestingUserId)
    {
        EnsureOwner(requestingUserId, home);
        var rule = await GetRuleAsync(ruleId);
        EnsureRuleBelongsToHome(rule, home);
        rule.Disable();
        await _ruleRepository.SaveAsync(rule);
    }

    public async Task ExecuteRulesAsync(Guid homeId, EvaluationContext context)
    {
        var rules = await _ruleRepository.GetByHomeIdAsync(homeId);
        var activeRuleIds = rules.Where(r => r.IsEnabled).Select(r => r.Id).ToHashSet();

        foreach (var rule in rules)
        {
            if (rule.ConflictsWithRuleIds.Any(id => activeRuleIds.Contains(id)))
                continue;
            if (rule.CanExecute(context))
            {
                rule.MarkAsTriggered();
                await _ruleRepository.SaveAsync(rule);
            }
        }
    }

    private async Task<AutomationRuleAggregate> GetRuleAsync(Guid ruleId)
        => await _ruleRepository.GetByIdAsync(ruleId)
            ?? throw new DomainException("Automation rule not found.");
}
