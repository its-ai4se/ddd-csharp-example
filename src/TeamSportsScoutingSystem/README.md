# Team Sports Scouting System Domain Model

This project implements a Domain-Driven Design (DDD) domain model for a Team Sports Scouting System based on the provided requirements.

## Overview

The Team Sports Scouting System is used by various employees of a sports club, including the head coach, director, and scouts. The system manages the process of identifying, scouting, and evaluating players for potential signings.

## Requirements Description

```txt
Team Sports Scouting (TSS),"The Team Sports Scouting System is used by various employees of the club, including the head coach, the director of the club and the scouts. It is the role of the head coach to identify designated player profiles for future signings, which includes designated target positions for a player (e.g. GK for goalkeeper, LB for left back, etc.), and other player attributes (identified by a name and a value).

Scouts may note players a long list who seem to match a designated target profile at any time. This long list is periodically evaluated by the head scout when setting up scouting assignments for his team to investigate a specific player more thoroughly. As the completion of a scouting assignment, the scout submits a scouting report about the player, which includes the pros and cons of the player as well as a recommendation (e.g. key player, first team player, reserve team player, prospective player, not a good signing).

After comparing first scouting results for a designated player profile, the head coach and the head scout decide upon which players to move to the short list. Several other rounds of scouting can be carried out for each short-listed player as part of scouting assignments – some of which is carried out by the head scout himself. If a player is finally recommended for signing by the head scout, the director makes an official offer for the player.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model Structure

### Core Aggregates

#### PersonAggregate

Represents all club employees (head coach, director, scouts) with their roles and contact information.

**Roles:**

- `HeadCoachRole`: Identifies designated player profiles for future signings
- `DirectorRole`: Makes official offers for players
- `ScoutRole`: Scouts players and submits reports (can be promoted to head scout)

#### PlayerAggregate

Represents players being scouted with their attributes and list status.

**Key Features:**

- Player information (name, date of birth, current club, nationality)
- Dynamic attributes (name-value pairs)
- List management (Long List ↔ Short List)
- Age calculation

#### PlayerProfileAggregate

Designated target profiles created by the head coach for future signings.

**Key Features:**

- Target positions (GK, LB, ST, etc.)
- Required player attributes
- Profile matching against players
- Active/inactive status

#### ScoutingAssignmentAggregate

Assignments given to scouts to investigate specific players.

**Key Features:**

- Assignment lifecycle (Created → InProgress → Completed/Cancelled)
- Scout assignment and tracking
- Notes and completion details

#### ScoutingReportAggregate

Reports submitted by scouts after completing assignments.

**Key Features:**

- Pros and cons analysis
- Recommendation (Key Player, First Team Player, etc.)
- Observed player attributes
- Report submission tracking

### Value Objects

- `PersonName`: First and last name with validation
- `Position`: Player position codes (GK, LB, CB, etc.)
- `PlayerAttribute`: Name-value pairs for player characteristics
- `Recommendation`: Scouting recommendations with predefined types
- `PlayerListType`: Long List vs Short List classification
- `EmailAddress`: Email validation
- `PhoneNumber`: Phone number storage

### Domain Services

#### PersonManagementService

Manages person registration and role assignments.

#### PlayerManagementService

Handles player list management and profile matching.

#### ScoutingManagementService

Orchestrates the scouting process from assignment to report submission.

#### OfferManagementService

Manages the official offer process based on scouting results.

## Business Rules

1. **Player Profile Creation**: Only head coaches can create player profiles
2. **Scouting Assignments**: Can be created by head scouts or scouts themselves
3. **Report Submission**: Only completed assignments can have reports submitted
4. **Short List Movement**: Players need positive recommendations to move to short list
5. **Official Offers**: Only directors can make offers, and only for short-listed players with head scout recommendations

## Usage Example

```csharp
// Create a head coach
var headCoach = new PersonAggregate(new PersonName("John", "Smith"));
var headCoachRole = new HeadCoachRole(headCoach.Id);
headCoach.AddRole(headCoachRole);

// Create a player profile
var profile = new PlayerProfileAggregate("Striker Profile", "Fast striker with good finishing", headCoach.Id);
profile.AddTargetPosition(Position.Striker);
profile.AddRequiredAttribute(new PlayerAttribute("Speed", "Fast"));

// Add player to long list
var player = new PlayerAggregate(new PersonName("Lionel", "Messi"), new DateOnly(1987, 6, 24), PlayerListType.LongList);

// Create scouting assignment
var assignment = new ScoutingAssignmentAggregate(player.Id, scout.Id, "Evaluate striker potential");

// Submit scouting report
var report = new ScoutingReportAggregate(player.Id, scout.Id, assignment.Id,
    "Excellent finishing, great vision", "Age concerns", Recommendation.KeyPlayer);
```

## Testing

The project includes comprehensive unit tests covering:

- Value object validation and equality
- Aggregate business logic
- Domain service operations
- Business rule enforcement

## Architecture

This implementation follows DDD principles with:

- Clear aggregate boundaries
- Rich domain models with business logic
- Repository interfaces for persistence
- Domain services for complex operations
- Value objects for type safety and validation
