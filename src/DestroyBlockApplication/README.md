# DestroyBlockApplication Domain Model

A Domain-Driven Design (DDD) implementation of a DestroyBlock game application using C# and .NET 8.

## Overview

The DestroyBlockApplication allows game admins to design DestroyBlock games and players to play these games, competing for entries in the game's hall of fame. The application follows Domain-Driven Design principles with clear separation of concerns and rich domain models.

## Requirements Description

```txt
The DestroyBlockapplication first allows a game admin to design a DestroyBlockgame and then players to play the game and compete for an entry in the game’s hall of fame.

DESIGN GAME:A user has a unique username. A user is always a player and optionally an admin. A user has the same password as a player and as an admin and chooses the admin mode or play mode when logging into the application. Only an admin may create a game.

Each game has a unique name and its own hall of fame. The admin designs a game by defining a set of blocks. Each block has a color and is worth a certain number of points between 1 and 1000 as specified by the admin.

A game has several levels as defined by the admin. Levels are numbered starting with Level 1 and the maximum number of levels is 99. For each level, the admin specifies the starting arrangement of blocks. Each block is placed in one cell of a grid system. The block at the top left corner is in grid position 1/1, the one to the right of it is in grid position 2/1, the one below it is in grid position 1/2, and so on. The admin may also define a level as random, i.e., the blocks at the top are randomly selected for the level from the set of blocks defined by the admin.

The number of blocks shown at the beginning of each level is the same and is also defined by the admin. With each level, the speed of the ball increases starting at its minimum speed and the length of the paddle is reduced gradually from its maximum length to its minimum length. The minimum speed, speed increase factor, maximum length, and minimum length are all specified by the admin for the game.

PLAY GAME: A player can play a game when it is published by the game admin. At the beginning of a game or level, the DestroyBlockapplication places the blocks at the top of the play area as specified by the admin in the design phase. The ball is placed in the center of the play area and drops in a straight line towards the bottom. The paddle of the player is positioned in the middle at the bottom of the play area. The player moves the paddle to the right or left at the bottom of the play area while trying to bounce the ball towards the blocks. The ball moves at a certain speed in a certain direction. The ball bounces back from the wall at the top as well as the two side walls on the right and left. If the ball hits a block, the ball bounces back, the block disappears, and the player scores the points of the hit block.

When the ball hits the last block, the player advances to the next level. If the ball reaches the bottom wall, the ball is out-of-bounds and the player loses one life. The player starts a game with three lives. When the player has lost all three lives or the player has finished the last level, the game ends and the total score is displayed in the game’s hall of fame.

At the end of a level or when the player pauses the game, the game is saved. A paused game can be resumed by the player. The next level of a game does not start automatically but only upon player confirmation.

A user may be a player for one game and an admin for another game but cannot be both for the same game. There is only one admin per game. Players compete against each other for the high score in the game’s hall of fame. A player may play different games and the same game multiple times. However, only one game may be played at any point in time, i.e., games are not played in parallel.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model

### Aggregates

#### UserAggregate

- Represents users who can be players and/or admins
- Manages user authentication and role assignments
- Supports both player and admin modes for the same user
- Enforces business rules around game role assignments

#### GameAggregate

- Represents a DestroyBlock game with its configuration
- Manages block types, levels, and game settings
- Controls game publishing/unpublishing
- Calculates dynamic game parameters (speed, paddle length) per level

#### GameSessionAggregate

- Represents an active game session for a player
- Tracks score, lives, current level, and session status
- Manages game progression and completion
- Supports pause/resume functionality

#### HallOfFameAggregate

- Represents the high scores for a game
- Manages leaderboard entries and rankings
- Provides query capabilities for top scores and player rankings

### Value Objects

- **Username**: Validates unique usernames (3-50 characters, alphanumeric + underscore/hyphen)
- **Password**: Ensures minimum password requirements (6+ characters)
- **GameName**: Validates game names (3-100 characters)
- **Score**: Constrains scores to 0-1000 range with arithmetic operations
- **LevelNumber**: Validates level numbers (1-99) with increment operations
- **GridPosition**: Represents grid coordinates (x,y) starting from 1,1
- **Color**: Represents block colors
- **Speed**: Represents ball speed with multiplication operations
- **PaddleLength**: Represents paddle length with reduction operations
- **Lives**: Manages player lives (0-3) with decrement operations

### Domain Services

#### GameManagementService

- Handles game creation and configuration
- Manages game publishing/unpublishing
- Enforces admin-only operations

#### GamePlayService

- Manages game session lifecycle
- Handles game progression and completion
- Integrates with hall of fame

#### UserManagementService

- Handles user registration and authentication
- Manages user roles and permissions
- Enforces business rules around role assignments

## Key Business Rules

1. **User Management**:

   - Users are players by default and can optionally be admins
   - Users cannot be both admin and player for the same game
   - Only one admin per game

2. **Game Design**:

   - Only admins can create games
   - Games must have block types and levels before publishing
   - Maximum 99 levels per game
   - Block points range from 1-1000

3. **Game Play**:

   - Players start with 3 lives
   - Only one active game session per player at a time
   - Games cannot be played in parallel
   - Ball speed increases and paddle length decreases with each level

4. **Hall of Fame**:
   - Only completed games are eligible for hall of fame
   - Rankings based on total score, then completion time
   - Players can have multiple entries for the same game

## Project Structure

```
src/DestroyBlockApplication/
├── src/
│   ├── Shared/
│   │   ├── Common/           # Base classes (Entity, AggregateRoot, ValueObject)
│   │   ├── ValueObjects/     # Shared value objects
│   │   └── Services/         # Base service classes
│   ├── User/                 # User aggregate and related classes
│   ├── Game/                 # Game aggregate and related classes
│   ├── GameSession/          # Game session aggregate and related classes
│   ├── HallOfFame/           # Hall of fame aggregate and related classes
│   ├── Services/             # Domain services
│   └── Program.cs            # Demo application
└── tests/
    ├── ValueObjects/         # Value object tests
    ├── Aggregates/           # Aggregate tests
    └── Services/             # Service tests
```

## Running the Application

### Build

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Run Demo

```bash
dotnet run --project src/DestroyBlockApplication.Domain.csproj
```

## Design Patterns Used

- **Aggregate Root**: Encapsulates business logic and maintains consistency boundaries
- **Value Objects**: Immutable objects representing concepts in the domain
- **Domain Services**: Services that don't naturally belong to any aggregate
- **Repository Pattern**: Abstracted data access through interfaces
- **Domain Events**: For future extensibility (infrastructure ready)

## Testing

The project includes comprehensive unit tests covering:

- Value object validation and operations
- Aggregate business logic and invariants
- Domain service operations
- Edge cases and error conditions

All tests follow the Arrange-Act-Assert pattern and use xUnit framework.

## Future Enhancements

- Domain event implementation for cross-aggregate communication
- Repository implementations for data persistence
- Application services for use case orchestration
- API layer for external interfaces
- Real-time multiplayer support
