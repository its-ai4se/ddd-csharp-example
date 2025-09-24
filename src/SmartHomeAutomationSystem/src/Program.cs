using SmartHomeAutomationSystem.Domain.Shared.Services;
using SmartHomeAutomationSystem.Domain.Home;
using SmartHomeAutomationSystem.Domain.Room;
using SmartHomeAutomationSystem.Domain.Device;
using SmartHomeAutomationSystem.Domain.User;
using SmartHomeAutomationSystem.Domain.Automation;
using SmartHomeAutomationSystem.Domain.Services;
using SmartHomeAutomationSystem.Domain.Shared.ValueObjects;

namespace SmartHomeAutomationSystem.Domain;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Smart Home Automation System - Domain Model Demo");
        Console.WriteLine("=================================================");

        // Create a home
        var home = new HomeAggregate("Smart Home", "123 Main Street, Montreal, QC");
        Console.WriteLine($"Created home: {home.Name} at {home.Address}");

        // Create rooms
        var livingRoom = new RoomAggregate(new RoomName("Living Room"), home.Id);
        var bedroom = new RoomAggregate(new RoomName("Master Bedroom"), home.Id);
        var kitchen = new RoomAggregate(new RoomName("Kitchen"), home.Id);
        
        home.AddRoom(livingRoom.Id);
        home.AddRoom(bedroom.Id);
        home.AddRoom(kitchen.Id);
        
        Console.WriteLine($"Created rooms: {livingRoom.Name.Value}, {bedroom.Name.Value}, {kitchen.Name.Value}");

        // Create users
        var adminUser = new UserAggregate(new UserName("John Admin"), new EmailAddress("john@smarthome.com"));
        var residentUser = new UserAggregate(new UserName("Jane Resident"), new EmailAddress("jane@smarthome.com"));
        
        adminUser.AddRole(new AdminRole(adminUser.Id));
        residentUser.AddRole(new ResidentRole(residentUser.Id));
        
        home.AddUser(adminUser.Id);
        home.AddUser(residentUser.Id);
        
        Console.WriteLine($"Created users: {adminUser.Name.Value} (Admin), {residentUser.Name.Value} (Resident)");

        // Create devices
        var livingRoomLight = new DeviceAggregate(new DeviceName("Living Room Light"), new DeviceType("Light"), livingRoom.Id);
        var bedroomThermostat = new DeviceAggregate(new DeviceName("Bedroom Thermostat"), new DeviceType("Thermostat"), bedroom.Id);
        var kitchenCamera = new DeviceAggregate(new DeviceName("Kitchen Camera"), new DeviceType("SecurityCamera"), kitchen.Id);
        
        // Set devices online before controlling them
        livingRoomLight.UpdateStatus(new DeviceStatus("Online"));
        bedroomThermostat.UpdateStatus(new DeviceStatus("Online"));
        kitchenCamera.UpdateStatus(new DeviceStatus("Online"));
        
        livingRoomLight.TurnOn();
        bedroomThermostat.UpdateSetting("temperature", 22.0);
        
        Console.WriteLine($"Created devices: {livingRoomLight.Name.Value}, {bedroomThermostat.Name.Value}, {kitchenCamera.Name.Value}");

        // Create automation rules
        var morningRule = new AutomationRuleAggregate(
            new AutomationRuleName("Morning Routine"), 
            home.Id, 
            adminUser.Id);
        
        var morningTrigger = new TimeTrigger(morningRule.Id, new TimeSpan(7, 0, 0), new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday });
        var turnOnLightAction = new DeviceAction(morningRule.Id, livingRoomLight.Id, "TurnOn");
        var adjustTempAction = new DeviceAction(morningRule.Id, bedroomThermostat.Id, "SetTemperature", new Dictionary<string, object> { { "temperature", 21.0 } });
        
        morningRule.AddTrigger(morningTrigger);
        morningRule.AddAction(turnOnLightAction);
        morningRule.AddAction(adjustTempAction);
        morningRule.Enable();
        
        Console.WriteLine($"Created automation rule: {morningRule.Name.Value}");

        // Display system status
        Console.WriteLine("\nSystem Status:");
        Console.WriteLine($"Home: {home.Name} with {home.RoomIds.Count} rooms and {home.UserIds.Count} users");
        Console.WriteLine($"Devices: {livingRoomLight.Name.Value} ({livingRoomLight.Status.Value}), {bedroomThermostat.Name.Value} ({bedroomThermostat.Status.Value}), {kitchenCamera.Name.Value} ({kitchenCamera.Status.Value})");
        Console.WriteLine($"Automation Rules: {morningRule.Name.Value} (Enabled: {morningRule.IsEnabled})");
        
        Console.WriteLine("\nDemo completed successfully!");
    }
}
