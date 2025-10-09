# Bus Transport Management System Domain Model

This project implements a Domain-Driven Design (DDD) solution for managing bus transportation operations, including bus fleet management, driver scheduling, route planning, and operational assignments.

## Overview

The Bus Transport Management System enables transportation companies to efficiently manage their bus fleet operations. The system handles bus maintenance status, driver availability, route assignments, and comprehensive scheduling to ensure reliable public transportation services.

## Requirements Description

```txt
A city is using the Bus Transportation Management System (BTMS) to simplify the day-to-day activities related to the city’s public bus system.

The BTMS keeps track of a driver’s name and automatically assigns a unique ID to each driver. A bus route is identified by a unique number that is determined by city staff, while a bus is identified by its unique licence plate. The highest possible number for a bus route is 9999, while a licence plate number may be up to 10 characters long, inclusive. For up to a year in advance, city staff assigns buses to routes. Several buses may be assigned to a route per day. Each bus serves at the most one route per day but may be assigned to different routes on different days. Similarly, for up to a year in advance, city staff posts the schedule for its bus drivers. For each route, there is a morning shift, an afternoon shift, and a night shift. A driver is assigned by city staff to a shift for a particular bus on a particular day. The BTMS offers city staff great flexibility, i.e., there are no restrictions in terms of how many shifts a bus driver has per day. It is even possible to assign a bus driver to two shifts at the same time.

The current version of BTMS does not support the information of bus drivers or buses to be updated – only adding and deleting is supported. However, BTMS does support indicating whether a bus driver is on sick leave and whether a bus is in the repair shop. If that is the case, the driver cannot be scheduled or the bus cannot be assigned to a route. For a given day, an overview shows – for each route number – the licence plate number of each assigned bus, the entered shifts and the IDs and names of the assigned drivers. If a driver is currently sick or a bus is in the repair shop, the driver or bus, respectively, is highlighted in the overview.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model Structure

### Core Aggregates

1. **BusAggregate** - Manages individual buses with license plates and repair status
2. **DriverAggregate** - Represents drivers with names and sick leave status
3. **RouteAggregate** - Defines transportation routes with route numbers
4. **ScheduleAggregate** - Coordinates bus and driver assignments to routes and shifts

### Value Objects

- **LicensePlate** - Bus identification with validation (max 10 characters)
- **RepairStatus** - Bus operational status (Operational, UnderRepair, OutOfService)
- **DriverName** - Driver identification with validation
- **SickLeaveStatus** - Driver availability status (Active, OnSickLeave)
- **RouteNumber** - Route identification with validation (1-9999)
- **ShiftPeriod** - Time periods for driver shifts (Morning, Afternoon, Night)
- **ScheduledDate** - Date validation for assignments

### Schedule Management

The system includes sophisticated scheduling capabilities:

- **BusRouteAssignment** - Links buses to specific routes on given dates
- **DriverShiftAssignment** - Assigns drivers to buses and routes for specific shifts
- **Assignment Validation** - Ensures buses and drivers are available before assignment
- **Conflict Prevention** - Prevents double-booking of buses and drivers

### Repository Interfaces

- `IBusRepository` - Bus aggregate persistence
- `IDriverRepository` - Driver aggregate persistence
- `IRouteRepository` - Route aggregate persistence
- `IScheduleRepository` - Schedule aggregate persistence

## Key Business Rules

1. **Bus Availability**: Only operational buses can be assigned to routes
2. **Driver Availability**: Only active drivers (not on sick leave) can be assigned to shifts
3. **Single Assignment**: Each bus can serve at most one route per day
4. **Assignment Dependencies**: Drivers can only be assigned to shifts for routes where buses are already assigned
5. **Date Validation**: Assignments cannot be made for past dates or more than one year in advance
6. **Status Management**: Buses can transition between operational, under repair, and out of service states
7. **Sick Leave**: Drivers can be marked as on sick leave or active
8. **Route Identification**: Routes are identified by unique numbers (1-9999)
9. **Shift Periods**: Three distinct shift periods (Morning, Afternoon, Night)
10. **Assignment Cleanup**: Removing bus assignments automatically removes related driver assignments

## System Operations

### Bus Management

- Register new buses with license plates
- Update bus repair status (operational, under repair, out of service)
- Track bus availability for service assignments

### Driver Management

- Register drivers with names
- Manage driver sick leave status
- Track driver availability for shift assignments

### Route Management

- Create routes with unique numbers
- Update route information
- Maintain route catalog

### Schedule Management

- Assign buses to routes for specific dates
- Assign drivers to shifts on buses and routes
- Query assignments by date, route, or resource
- Remove assignments with proper cleanup

## Project Structure

```
src/BusTransportManagementSystem/
├── src/
│   ├── Shared/
│   │   ├── Common/          # Base classes (Entity, AggregateRoot, ValueObject)
│   │   └── ValueObjects/    # Value objects
│   ├── Bus/                 # Bus aggregate
│   ├── Driver/              # Driver aggregate
│   ├── Route/                # Route aggregate
│   ├── Schedule/             # Schedule aggregate and value objects
│   ├── Repositories/         # Repository interfaces
│   └── Program.cs            # Demonstration
└── tests/
    ├── Entity/               # Entity tests
    ├── ValueObject/          # Value object tests
    └── BusinessRulesTests.cs  # Business rule tests
```

## Usage Example

```csharp
// Create buses
var bus1 = new BusAggregate(new LicensePlate("ABC123"));
var bus2 = new BusAggregate(new LicensePlate("XYZ789"));

// Create drivers
var driver1 = new DriverAggregate(new DriverName("John Smith"));
var driver2 = new DriverAggregate(new DriverName("Jane Doe"));

// Create routes
var route1 = new RouteAggregate(new RouteNumber("101"));
var route2 = new RouteAggregate(new RouteNumber("202"));

// Create schedule
var schedule = new ScheduleAggregate();

// Assign buses to routes
var today = new ScheduledDate(DateTime.Today.AddDays(1));
schedule.AssignBusToRoute(bus1.Id, route1.Id, today, bus1, route1);
schedule.AssignBusToRoute(bus2.Id, route2.Id, today, bus2, route2);

// Assign drivers to shifts
var morningShift = new ShiftPeriod(ShiftPeriodType.Morning);
schedule.AssignDriverToShift(driver1.Id, bus1.Id, route1.Id, morningShift, today, driver1, bus1, route1);

// Check assignments
var busAssignments = schedule.GetBusAssignmentsForDate(today);
var driverAssignments = schedule.GetDriverAssignmentsForDate(today);
```

## Testing

The solution includes comprehensive tests covering:

- Aggregate behavior and business rules
- Value object equality and validation
- Schedule assignment logic and constraints
- Status management and availability checks

Run tests using:

```bash
dotnet test
```

## Key Features

- **Domain-Driven Design**: Clean separation of domain logic
- **Aggregate Pattern**: Proper aggregate boundaries and consistency
- **Value Objects**: Immutable objects for domain concepts
- **Repository Pattern**: Data access abstraction
- **Comprehensive Testing**: Unit tests for all domain components
- **Schedule Management**: Complex assignment logic with validation
- **Status Tracking**: Bus repair status and driver availability
- **Conflict Prevention**: Business rules prevent invalid assignments
- **Assignment Dependencies**: Proper ordering of bus and driver assignments
- **Date Validation**: Prevents past and far-future assignments
