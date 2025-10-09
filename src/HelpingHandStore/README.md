# Helping Hand Store (H2S) Domain Model

This project implements a Domain-Driven Design (DDD) solution for the Helping Hand Store pickup and delivery service system.

## Overview

The Helping Hand Store (H2S) collects second-hand articles and non-perishable foods from residents of Montreal and distributes them to those in need. This domain model supports the pickup and delivery service that allows residents to schedule item pickups online.

## Requirements Description

```txt
The Helping Hand Store (H2S) collects second hand articles and non-perishable foods from residents. A resident enters a name, street address, phone number, optional email address, as well as a description of the items to be picked up.

At the beginning of every weekday, a pickup route for that day is determined for each vehicle for which a volunteer driver is available. Volunteer drivers indicate their available days on the H2S website. The route takes into account the available storage space of a vehicle and the dimensions and weights of scheduled items. After completing all scheduled pickups, the driver drops off all collected second hand articles at H2S’s distribution center. Those articles that can still be used are tagged with an RFID device. In addition, the H2S employee assigns a category to the article from a standard list of 134 categories (e.g., baby clothing, women’s winter boots, fridge, microwave…).

H2S allows those clients to indicate which categories of articles they need. An H2S employee calls them to let them know about the relevant articles that were dropped off that day. Delivery of such articles is made by a volunteer driver before picking up items according to the pickup route.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model Structure

### Core Aggregates

1. **H2SAggregate** - The main organization aggregate representing each H2S location
2. **PersonAggregate** - Represents people who can have multiple roles (Resident, Volunteer, Client)
3. **ItemAggregate** - Abstract base for items to be picked up
   - **SecondHandArticle** - Second-hand items that go to distribution center
   - **FoodItem** - Non-perishable foods that go directly to food bank
4. **VehicleAggregate** - Pickup vehicles with capacity constraints
5. **RouteAggregate** - Daily pickup routes for vehicles

### Value Objects

- **PersonName** - First and last name validation
- **Address** - Complete address with validation
- **PhoneNumber** - Phone number validation
- **EmailAddress** - Email validation
- **ItemDescription** - Item description with length limits
- **Dimensions** - Length, width, height with volume calculation
- **Weight** - Weight with unit conversion support
- **ScheduledDate** - Pickup scheduling with weekday validation
- **RfidCode** - RFID tagging for second-hand articles
- **ItemCategory** - Enumeration of 134+ item categories

### Player-Role Pattern

The system uses a player-role pattern where a Person can have multiple roles:

- **ResidentRole** - Can schedule item pickups
- **VolunteerRole** - Can drive vehicles and indicate availability
- **ClientRole** - Can receive items and specify needed categories

### Domain Services

- **RoutePlanningService** - Handles route planning and capacity calculations
- **ItemProcessingService** - Manages item processing and RFID tagging
- **PersonManagementService** - Handles person role management

## Key Business Rules

1. **Pickup Scheduling**: Items can only be picked up on weekdays between 8:00 AM and 2:00 PM
2. **Vehicle Capacity**: Routes must respect vehicle dimension and weight limits
3. **Item Processing**: Second-hand articles are tagged with RFID and categorized
4. **Food Distribution**: Food items go directly to food bank, not distribution center
5. **Role Management**: Persons can have multiple roles simultaneously

## Repository Interfaces

- `IPersonRepository` - Person aggregate persistence
- `IItemRepository` - Item aggregate persistence
- `IVehicleRepository` - Vehicle aggregate persistence
- `IRouteRepository` - Route aggregate persistence
- `IH2SRepository` - H2S organization persistence

## Testing

The solution includes a demonstration test that shows the complete domain model in action, including:

- Creating H2S organization
- Registering a person with multiple roles
- Creating items for pickup
- Planning routes
- Processing items

## Project Structure

```
src/HelpingHandStore/
├── src/
│   ├── Shared/
│   │   ├── Common/          # Base classes (Entity, AggregateRoot, ValueObject)
│   │   ├── Services/        # Domain services and interfaces
│   │   └── ValueObjects/    # Value objects
│   ├── Person/              # Person aggregate and roles
│   ├── Item/                # Item aggregates (SecondHandArticle, FoodItem)
│   ├── Vehicle/             # Vehicle aggregate
│   ├── Route/               # Route aggregate
│   ├── H2S/                 # H2S organization aggregate
│   └── Services/            # Domain services
└── tests/
    └── DomainModelDemo.cs   # Demonstration test
```

## Usage Example

```csharp
// Create H2S organization
var h2s = new H2SAggregate("Helping Hand Store Montreal", address, "Montreal");

// Create person with multiple roles
var person = new PersonAggregate(name, address, phone, email);
personService.RegisterResident(person);
personService.RegisterVolunteer(person, availableDays);
personService.RegisterClient(person, neededCategories);

// Create items for pickup
var article = new SecondHandArticle(description, dimensions, weight, pickupDate, person.Id);
var food = new FoodItem(description, dimensions, weight, pickupDate, person.Id);

// Plan route
var route = new RouteAggregate(pickupDate.Date, vehicle.Id, person.Id);
route.AddScheduledItem(article.Id);
route.AddScheduledItem(food.Id);
```

This implementation follows DDD principles with clear aggregate boundaries, rich domain models, and comprehensive business rule enforcement.
