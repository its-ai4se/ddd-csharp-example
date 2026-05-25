using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;
using SmartHomeAutomationSystem.Domain.Automation.Precondition;
using SmartHomeAutomationSystem.Domain.Automation.Events;

namespace SmartHomeAutomationSystem.Domain.Automation;

public class AutomationRuleAggregate : AggregateRoot
{
    public AutomationRuleName Name { get; private set; }
    public Guid HomeId { get; }
    public IPreconditionExpression? Precondition { get; private set; }
    public ActionSequence? ActionSequence { get; private set; }
    public bool IsEnabled { get; private set; }
    public DateTime? LastTriggeredAt { get; private set; }

    private readonly List<Guid> _dependsOnRuleIds = [];
    private readonly List<Guid> _conflictsWithRuleIds = [];
    public IReadOnlyList<Guid> DependsOnRuleIds => _dependsOnRuleIds.AsReadOnly();
    public IReadOnlyList<Guid> ConflictsWithRuleIds => _conflictsWithRuleIds.AsReadOnly();

    public AutomationRuleAggregate(AutomationRuleName name, Guid homeId) : base()
    {
        if (homeId == Guid.Empty)
            throw new DomainException("Home ID cannot be empty.");
        Name = name ?? throw new ArgumentNullException(nameof(name));
        HomeId = homeId;
        IsEnabled = false;
    }

    private void EnsureDeactivated()
    {
        if (IsEnabled)
            throw new DomainException("Rule must be deactivated before editing.");
    }

    public void SetPrecondition(IPreconditionExpression precondition)
    {
        EnsureDeactivated();
        Precondition = precondition ?? throw new ArgumentNullException(nameof(precondition));
    }

    public void SetActionSequence(ActionSequence sequence)
    {
        EnsureDeactivated();
        ActionSequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
    }

    public void Enable()
    {
        if (Precondition is null)
            throw new DomainException("Cannot enable rule without a precondition.");
        if (ActionSequence is null)
            throw new DomainException("Cannot enable rule without an action sequence.");
        IsEnabled = true;
    }

    public void Disable() => IsEnabled = false;

    public void MarkAsTriggered()
    {
        LastTriggeredAt = DateTime.UtcNow;
        AddDomainEvent(new AutomationRuleTriggeredEvent(Id, LastTriggeredAt.Value));
    }

    public bool CanExecute(EvaluationContext context)
        => IsEnabled && Precondition is not null && Precondition.Evaluate(context);

    public void AddDependency(Guid ruleId)
    {
        EnsureDeactivated();
        if (ruleId == Guid.Empty || ruleId == Id)
            throw new DomainException("Invalid dependency rule ID.");
        if (!_dependsOnRuleIds.Contains(ruleId))
            _dependsOnRuleIds.Add(ruleId);
    }

    /// <summary>
    /// Adds a dependency with circular dependency detection.
    /// Pass allRules to enable cycle detection (AR-030).
    /// </summary>
    public void AddDependency(Guid ruleId, IEnumerable<AutomationRuleAggregate> allRules)
    {
        EnsureDeactivated();
        if (ruleId == Guid.Empty || ruleId == Id)
            throw new DomainException("Invalid dependency rule ID.");

        // Check for circular dependency: if ruleId already depends on this rule (directly or transitively)
        var ruleMap = allRules.ToDictionary(r => r.Id);
        if (WouldCreateCycle(ruleId, ruleMap))
            throw new DomainException("Circular dependency detected between automation rules.");

        if (!_dependsOnRuleIds.Contains(ruleId))
            _dependsOnRuleIds.Add(ruleId);
    }

    private bool WouldCreateCycle(Guid targetId, Dictionary<Guid, AutomationRuleAggregate> ruleMap)
    {
        // BFS/DFS: check if targetId transitively depends on this.Id
        var visited = new HashSet<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(targetId);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == Id) return true;
            if (!visited.Add(current)) continue;
            if (ruleMap.TryGetValue(current, out var rule))
                foreach (var dep in rule._dependsOnRuleIds)
                    queue.Enqueue(dep);
        }
        return false;
    }

    public void AddConflict(Guid ruleId)
    {
        EnsureDeactivated();
        if (ruleId == Guid.Empty || ruleId == Id)
            throw new DomainException("Invalid conflict rule ID.");
        if (!_conflictsWithRuleIds.Contains(ruleId))
            _conflictsWithRuleIds.Add(ruleId);
    }

    public bool HasConflictWith(Guid ruleId) => _conflictsWithRuleIds.Contains(ruleId);
    public bool DependsOn(Guid ruleId) => _dependsOnRuleIds.Contains(ruleId);
}
