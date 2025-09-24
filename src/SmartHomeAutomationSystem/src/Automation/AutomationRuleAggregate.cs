using SmartHomeAutomationSystem.Domain.Shared.Common;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

namespace SmartHomeAutomationSystem.Domain.Automation;

public class AutomationRuleAggregate : AggregateRoot
{
    public AutomationRuleName Name { get; private set; }
    public Guid HomeId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public List<Trigger> Triggers { get; private set; }
    public List<Action> Actions { get; private set; }
    public bool IsEnabled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastExecuted { get; private set; }

    public AutomationRuleAggregate(AutomationRuleName name, Guid homeId, Guid createdByUserId) : base()
    {
        if (homeId == Guid.Empty)
            throw new DomainException("Home ID cannot be empty.");
        
        if (createdByUserId == Guid.Empty)
            throw new DomainException("Created by user ID cannot be empty.");
        
        Name = name ?? throw new ArgumentNullException(nameof(name));
        HomeId = homeId;
        CreatedByUserId = createdByUserId;
        Triggers = new List<Trigger>();
        Actions = new List<Action>();
        IsEnabled = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddTrigger(Trigger trigger)
    {
        if (trigger == null)
            throw new ArgumentNullException(nameof(trigger));
        
        Triggers.Add(trigger);
    }

    public void RemoveTrigger(Guid triggerId)
    {
        var trigger = Triggers.FirstOrDefault(t => t.Id == triggerId);
        if (trigger == null)
            throw new DomainException("Trigger not found.");
        
        Triggers.Remove(trigger);
    }

    public void AddAction(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));
        
        Actions.Add(action);
    }

    public void RemoveAction(Guid actionId)
    {
        var action = Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
            throw new DomainException("Action not found.");
        
        Actions.Remove(action);
    }

    public void Enable()
    {
        if (Triggers.Count == 0)
            throw new DomainException("Cannot enable rule without triggers.");
        
        if (Actions.Count == 0)
            throw new DomainException("Cannot enable rule without actions.");
        
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }

    public void UpdateName(AutomationRuleName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void MarkAsExecuted()
    {
        LastExecuted = DateTime.UtcNow;
    }

    public bool CanExecute()
    {
        return IsEnabled && Triggers.All(t => t.IsTriggered()) && Actions.Count > 0;
    }
}

public abstract class Trigger : Entity
{
    public Guid RuleId { get; protected set; }
    public string TriggerType { get; protected set; }

    protected Trigger(Guid ruleId, string triggerType) : base()
    {
        RuleId = ruleId;
        TriggerType = triggerType;
    }

    public abstract bool IsTriggered();
}

public abstract class Action : Entity
{
    public Guid RuleId { get; protected set; }
    public string ActionType { get; protected set; }

    protected Action(Guid ruleId, string actionType) : base()
    {
        RuleId = ruleId;
        ActionType = actionType;
    }

    public abstract void Execute();
}

public class DeviceStatusTrigger : Trigger
{
    public Guid DeviceId { get; private set; }
    public string ExpectedStatus { get; private set; }

    public DeviceStatusTrigger(Guid ruleId, Guid deviceId, string expectedStatus) 
        : base(ruleId, "DeviceStatus")
    {
        DeviceId = deviceId;
        ExpectedStatus = expectedStatus;
    }

    public override bool IsTriggered()
    {
        // This would typically check the actual device status
        // For now, we'll return false as a placeholder
        return false;
    }
}

public class TimeTrigger : Trigger
{
    public TimeSpan Time { get; private set; }
    public List<DayOfWeek> Days { get; private set; }

    public TimeTrigger(Guid ruleId, TimeSpan time, List<DayOfWeek> days) 
        : base(ruleId, "Time")
    {
        Time = time;
        Days = days ?? new List<DayOfWeek>();
    }

    public override bool IsTriggered()
    {
        var now = DateTime.Now;
        return Days.Contains(now.DayOfWeek) && 
               now.TimeOfDay.Hours == Time.Hours && 
               now.TimeOfDay.Minutes == Time.Minutes;
    }
}

public class DeviceAction : Action
{
    public Guid DeviceId { get; private set; }
    public string Command { get; private set; }
    public Dictionary<string, object> Parameters { get; private set; }

    public DeviceAction(Guid ruleId, Guid deviceId, string command, Dictionary<string, object>? parameters = null) 
        : base(ruleId, "Device")
    {
        DeviceId = deviceId;
        Command = command;
        Parameters = parameters ?? new Dictionary<string, object>();
    }

    public override void Execute()
    {
        // This would typically send the command to the device
        // For now, we'll just mark it as executed
    }
}
