namespace SmartHomeAutomationSystem.Domain.Device;

public static class AllowedCommands
{
    private static readonly Dictionary<string, HashSet<string>> _map = new()
    {
        ["Light"]     = ["turnOn", "turnOff", "dim", "brighten"],
        ["DoorLock"]  = ["lockDoor", "unlockDoor"],
        ["Thermostat"]= ["turnOnHeating", "turnOffHeating", "setTemperature"],
        ["SmartPlug"] = ["turnOn", "turnOff"],
        ["Blinds"]    = ["open", "close", "setPosition"],
    };

    public static bool IsAllowed(string deviceType, string commandName)
        => _map.TryGetValue(deviceType, out var cmds) && cmds.Contains(commandName);
}
