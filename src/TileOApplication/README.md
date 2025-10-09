# Tile-O Application Domain Model

This project implements a Domain-Driven Design (DDD) solution for the Tile-O board game application system.

## Overview

The Tile-O application allows a game designer to design a board game and then allows players to play the game. The objective is to find a hidden tile on the board. Two to four players take turns by moving their playing pieces along connected tiles based on the roll of a die.

## Requirements Description

```txt
The Tile-O application first allows a game designer to design a board game and then allows players to play the game. Only one game can be played at a time and it cannot be paused or saved. The objective of the game is to find a hidden tile on the board. Two to four players take turns by moving their playing pieces along connected tiles based on the roll of a die. Each playing piece has a different color.

A designer first defines the whole game including the layout of the game board. The designer places the tiles on the board and connects them with connection pieces. A tile can be connected to other tiles on its right side, left side, top side, and bottom side. At the most one tile can be connected on each side. In addition, the designer indicates the hidden tile, the starting positions of each player, as well as the location of action tiles.

The designer also defines a deck of 32 action cards by choosing from the following predefined choices: (i) roll the die for an extra turn, (ii) connect two adjacent tiles with a connection piece from the pile of 32 spare connection pieces, (iii) remove a connection piece from the board and place it in the pile of spare connection pieces, (iv) move your playing piece to an arbitrary tile that is not your current tile, and (v) lose your next turn.

Players take turns, with Player 1 starting the game, followed by Player 2, Player 3 (if applicable), and Player 4 (if applicable). The player whose turn it is rolls the die and them moves their playing piece along connected tiles. If the player lands on any tile, the color of the tile changes from white to black to indicate that the tile has been visited during the game. If the player lands on the hidden tile, the game ends and the player wins the game. If the player lands on an action tile, the player takes the first action card from the deck of action cards and follows the instructions on the action card. In addition, the action tile turns into a regular tile for a number of turns as specified by the game designer. Players do not know whether a tile is an action tile until a player lands on it.
```

Source: [Yujing Yang's multi-step domain model generation models](https://github.com/YujingYang666777/DomainModelGeneration/blob/main/models.csv)

## Domain Model Structure

### Core Aggregates

1. **GameAggregate** - The main aggregate root representing each game instance
2. **PlayerAggregate** - Represents players with their colors and positions
3. **BoardAggregate** - Manages the game board with tiles and connections
4. **TileEntity** - Individual tiles on the board
5. **ActionCardEntity** - Action cards that players can use during gameplay

### Value Objects

- **Position** - X,Y coordinates for tile positions
- **PlayerColor** - Player colors with hex codes (Red, Blue, Green, Yellow)
- **ActionCardDescription** - Describes different types of action cards
- **TileState** - State of tiles (Regular, Action, Hidden, Starting)
- **Connection** - Represents connections between tiles in different directions
- **Direction** - Enumeration for North, South, East, West directions

### Action Card Types

The system supports five predefined action card types:

1. **ExtraTurn** - Roll the die for an extra turn
2. **ConnectTiles** - Connect two adjacent tiles with a connection piece
3. **RemoveConnection** - Remove a connection piece from the board
4. **Teleport** - Move your playing piece to an arbitrary tile
5. **SkipTurn** - Lose your next turn

### Domain Services

- **GameDesignService** - Handles game setup and board creation
- **GamePlayService** - Manages gameplay mechanics and action card execution

### Repository Interfaces

- `IGameRepository` - Game aggregate persistence
- `IPlayerRepository` - Player aggregate persistence
- `IBoardRepository` - Board aggregate persistence

## Key Business Rules

1. **Game Phases**: Games progress through Designing → ReadyToPlay → InProgress → Completed
2. **Player Limits**: 2-4 players per game, each with unique colors
3. **Action Cards**: Maximum 32 action cards per game deck
4. **Board Connections**: Tiles can connect in four directions (North, South, East, West)
5. **Hidden Tile**: One tile must be designated as the hidden tile
6. **Starting Positions**: Each player must have a designated starting position
7. **Turn Management**: Players take turns in order, with Player 1 starting
8. **Tile Visitation**: Visited tiles change from white to black
9. **Action Tiles**: Special tiles that become regular tiles after being landed on
10. **Game Completion**: Game ends when a player lands on the hidden tile

## Game Flow

1. **Design Phase**: Designer creates board, sets hidden tile, defines starting positions, and creates action card deck
2. **Ready Phase**: Game validates all requirements and prepares for play
3. **Play Phase**: Players take turns rolling dice and moving along connected tiles
4. **Action Cards**: Players can use action cards when landing on action tiles
5. **Completion**: Game ends when a player finds the hidden tile

## Project Structure

```
src/TileOApplication/
├── src/
│   ├── Shared/
│   │   ├── Common/          # Base classes (Entity, AggregateRoot, ValueObject)
│   │   ├── Services/        # Domain services and interfaces
│   │   └── ValueObjects/    # Value objects
│   ├── Game/                # Game aggregate
│   ├── Player/              # Player aggregate
│   ├── Board/               # Board aggregate
│   ├── Tile/                # Tile entity
│   ├── ActionCard/          # Action card entity
│   ├── Services/            # Domain services
│   ├── Repositories/        # Repository interfaces
│   └── Program.cs           # Demonstration
└── tests/
    ├── Aggregates/          # Aggregate tests
    ├── ValueObjects/        # Value object tests
    └── Services/            # Service tests
```

## Usage Example

```csharp
// Create a new game
var game = new GameAggregate("My Tile-O Game");

// Setup using design service
var designService = new GameDesignService();
designService.SetupDefaultPlayers(game);
designService.CreateDefaultActionCardDeck(game);
designService.CreateSampleBoard(game);

// Start the game
game.StartGame();
game.BeginPlay();

// Play using gameplay service
var playService = new GamePlayService();
var diceRoll = playService.RollDie();
var validMoves = playService.GetValidMoves(game, currentPlayerId, diceRoll);

// Move player
game.MovePlayer(currentPlayerId, targetPosition);
```

## Testing

The solution includes comprehensive tests covering:

- Aggregate behavior and business rules
- Value object equality and validation
- Domain service functionality
- Game flow and state transitions

Run tests using:

```bash
dotnet test
```

## Key Features

- **Domain-Driven Design**: Clean separation of domain logic
- **Aggregate Pattern**: Proper aggregate boundaries and consistency
- **Value Objects**: Immutable objects for domain concepts
- **Domain Services**: Complex business logic encapsulation
- **Repository Pattern**: Data access abstraction
- **Comprehensive Testing**: Unit tests for all domain components
- **Game State Management**: Proper state transitions and validation
- **Action Card System**: Flexible action card mechanics
- **Board Management**: Tile connections and movement validation
