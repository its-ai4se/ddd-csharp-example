# Smart Home Automation System Domain Model

This project implements a Domain-Driven Design (DDD) solution for a Smart Home Automation System that manages homes, rooms, devices, users, and automation rules.

## Overview

The Smart Home Automation System enables homeowners to manage their smart devices, create automation rules, and control their home environment through a comprehensive domain model that follows DDD principles.

## Requirements Description

```txt
Smart home automation system (SHAS),"A smart home automation system (SHAS) offers various users to automatically manage smart home automation tasks. A smart home (located at a physical address) consists of several rooms, each of which may contain sensor devices and actuator (controller) devices of different types (e.g. temperature sensor, movement sensor, light controller, lock controller). Each sensor and actuator have a unique device identifier. Once a new sensor or actuator is activated or deactivated, SHAS will recognize the change and update its infrastructure map.

When SHAS is operational, a sensor device periodically provides sensor readings (recording the measured value and the timestamp). Similarly, a predefined set of control commands (e.g. lockDoor, turnOnHeating) can be sent to the actuator devices with the timestamp and the status of the command (e.g. requested, completed, failed, etc.). All sensor readings and control commands for a smart home are recorded by SHAS in an activity log.

Relevant alerts in a smart home can be set up and managed by its owner by setting up automation rules. An automation rule has a precondition and an action. The precondition is a Boolean expression constructed from relational terms connected by basic Boolean operators (AND, OR, NOT). Atomic relational terms may refer to rooms, sensors, actuators, sensor readings and control commands. The action is a sequence of control commands. For example, a sample rule could specify:
when actualTemperature by Device #1244 in Living Room < 18 and window is closed
then turnOnHeating in Living Room

Automation rules can be created, edited, activated and deactivated by owners. Only deactivated rules can be edited. Rules can also depend on or conflict with other rules, thus a complex rule hierarchy can be designed. SHAS records whenever an active rule was triggered using a timestamp.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model Structure

### Core Aggregates

1. **HomeAggregate** - Represents a smart home with rooms and users
2. **RoomAggregate** - Represents rooms within a home that contain devices
3. **DeviceAggregate** - Represents smart devices (lights, thermostats, cameras, etc.)
4. **UserAggregate** - Represents users with different roles (Admin, Resident, Guest)
5. **AutomationRuleAggregate** - Represents automation rules with triggers and actions

### Value Objects

- **DeviceName** - Device name validation
- **RoomName** - Room name validation
- **DeviceType** - Device type enumeration (Light, Thermostat, DoorLock, etc.)
- **DeviceStatus** - Device status enumeration (Online, Offline, Error, Maintenance)
- **Temperature** - Temperature with unit conversion support
- **UserName** - User name validation
- **EmailAddress** - Email validation
- **AutomationRuleName** - Automation rule name validation

### User Roles

The system uses a role-based access control pattern:

- **AdminRole** - Full system access and management
- **ResidentRole** - Home resident with device control access
- **GuestRole** - Temporary access with expiration

### Automation System

The automation system supports:

- **Triggers** - Conditions that activate automation rules
  - TimeTrigger - Time-based triggers
  - DeviceStatusTrigger - Device state-based triggers
- **Actions** - Commands executed when triggers are activated
  - DeviceAction - Device control actions

### Domain Services

- **DeviceManagementService** - Handles device creation, movement, and status updates
- **AutomationService** - Manages automation rules and execution
- **UserManagementService** - Handles user creation and role management

## Key Business Rules

1. **Device Management**: Devices must be assigned to rooms and can be moved between rooms
2. **User Roles**: Users can have multiple roles simultaneously
3. **Automation Rules**: Rules must have at least one trigger and one action to be enabled
4. **Device Control**: Devices can only be controlled when online
5. **Temperature Control**: Temperature values must be within valid ranges

## Repository Interfaces

- `IHomeRepository` - Home aggregate persistence
- `IRoomRepository` - Room aggregate persistence
- `IDeviceRepository` - Device aggregate persistence
- `IUserRepository` - User aggregate persistence
- `IAutomationRuleRepository` - Automation rule persistence

## Testing

The solution includes comprehensive unit tests covering:

- Value object validation
- Aggregate behavior
- Domain service operations
- Business rule enforcement

## Project Structure

```
src/SmartHomeAutomationSystem/
├── src/
│   ├── Shared/
│   │   ├── Common/          # Base classes (Entity, AggregateRoot, ValueObject)
│   │   ├── Services/        # Domain services and interfaces
│   │   └── ValueObjects/    # Value objects
│   ├── Home/                # Home aggregate
│   ├── Room/                # Room aggregate
│   ├── Device/              # Device aggregate
│   ├── User/                # User aggregate and roles
│   ├── Automation/          # Automation rule aggregate
│   ├── Services/            # Domain services
│   └── Program.cs           # Demonstration program
└── tests/
    ├── ValueObjects/        # Value object tests
    ├── Aggregates/          # Aggregate tests
    └── Services/            # Service tests
```

## Usage Example

```csharp
// Create a home
var home = new HomeAggregate("Smart Home", "123 Main Street, Montreal, QC");

// Create rooms
var livingRoom = new RoomAggregate(new RoomName("Living Room"), home.Id);
var bedroom = new RoomAggregate(new RoomName("Master Bedroom"), home.Id);

// Create users
var adminUser = new UserAggregate(new UserName("John Admin"), new EmailAddress("john@smarthome.com"));
adminUser.AddRole(new AdminRole(adminUser.Id));

// Create devices
var light = new DeviceAggregate(new DeviceName("Living Room Light"), new DeviceType("Light"), livingRoom.Id);
var thermostat = new DeviceAggregate(new DeviceName("Bedroom Thermostat"), new DeviceType("Thermostat"), bedroom.Id);

// Create automation rule
var morningRule = new AutomationRuleAggregate(
    new AutomationRuleName("Morning Routine"),
    home.Id,
    adminUser.Id);

var timeTrigger = new TimeTrigger(morningRule.Id, new TimeSpan(7, 0, 0), new List<DayOfWeek> { DayOfWeek.Monday });
var lightAction = new DeviceAction(morningRule.Id, light.Id, "TurnOn");

morningRule.AddTrigger(timeTrigger);
morningRule.AddAction(lightAction);
morningRule.Enable();
```

## Device Types Supported

- Light
- Thermostat
- DoorLock
- SecurityCamera
- MotionSensor
- SmokeDetector
- WindowSensor
- SmartPlug
- Speaker
- Blinds

This implementation follows DDD principles with clear aggregate boundaries, rich domain models, comprehensive business rule enforcement, and extensive test coverage.
