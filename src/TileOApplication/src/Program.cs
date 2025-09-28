using TileOApplication.Domain.Game;
using TileOApplication.Domain.Player;
using TileOApplication.Domain.Board;
using TileOApplication.Domain.Tile;
using TileOApplication.Domain.ActionCard;
using TileOApplication.Domain.Services;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Tile-O Application Domain Model Demo ===\n");

        try
        {
            // Create a new game
            var game = new GameAggregate("My First Tile-O Game");
            Console.WriteLine($"Created game: {game}");

            // Setup the game using the design service
            var designService = new GameDesignService();
            
            Console.WriteLine("\n--- Setting up players ---");
            designService.SetupDefaultPlayers(game);
            Console.WriteLine($"Added {game.Players.Count} players:");
            foreach (var player in game.Players)
            {
                Console.WriteLine($"  - {player}");
            }

            Console.WriteLine("\n--- Creating action card deck ---");
            designService.CreateDefaultActionCardDeck(game);
            Console.WriteLine($"Created {game.ActionCards.Count} action cards");

            Console.WriteLine("\n--- Creating game board ---");
            designService.CreateSampleBoard(game);
            Console.WriteLine($"Created board with {game.Board.Tiles.Count} tiles");
            Console.WriteLine($"Hidden tile at: {game.Board.HiddenTilePosition}");
            Console.WriteLine($"Starting positions: {game.Board.StartingPositions.Count}");

            // Start the game
            Console.WriteLine("\n--- Starting the game ---");
            game.StartGame();
            Console.WriteLine($"Game status: {game.Status}");
            Console.WriteLine($"Current player: {game.Players.First(p => p.Id == game.CurrentPlayerId)}");
            Console.WriteLine($"Current turn: {game.CurrentTurn}");

            // Begin play
            game.BeginPlay();
            Console.WriteLine($"Game status: {game.Status}");

            // Demonstrate gameplay
            Console.WriteLine("\n--- Gameplay Demo ---");
            var playService = new GamePlayService();
            var currentPlayer = game.Players.First(p => p.Id == game.CurrentPlayerId);
            
            Console.WriteLine($"Current player: {currentPlayer}");
            Console.WriteLine($"Player position: {currentPlayer.CurrentPosition}");

            // Roll die and show valid moves
            var diceRoll = playService.RollDie();
            Console.WriteLine($"Dice roll: {diceRoll}");
            
            var validMoves = playService.GetValidMoves(game, currentPlayer.Id, diceRoll);
            Console.WriteLine($"Valid moves from current position: {validMoves.Count}");
            foreach (var move in validMoves.Take(5)) // Show first 5 moves
            {
                Console.WriteLine($"  - {move}");
            }

            // Make a move
            if (validMoves.Count > 1)
            {
                var targetPosition = validMoves[1]; // Move to second valid position
                Console.WriteLine($"\nMoving player to: {targetPosition}");
                game.MovePlayer(currentPlayer.Id, targetPosition);
                
                Console.WriteLine($"Player new position: {currentPlayer.CurrentPosition}");
                Console.WriteLine($"Game status: {game.Status}");
                
                if (game.Status == GameStatus.Completed)
                {
                    var winner = game.Players.First(p => p.Id == game.WinnerId);
                    Console.WriteLine($"🎉 Game completed! Winner: {winner}");
                }
            }

            // Demonstrate action card usage
            Console.WriteLine("\n--- Action Card Demo ---");
            var unusedActionCard = game.ActionCards.FirstOrDefault(ac => !ac.IsUsed);
            if (unusedActionCard != null)
            {
                Console.WriteLine($"Available action card: {unusedActionCard.Description}");
                
                if (unusedActionCard.Description.Type == ActionCardType.ConnectTiles)
                {
                    var parameters = new Dictionary<string, object>
                    {
                        ["fromPosition"] = new Position(0, 0),
                        ["toPosition"] = new Position(1, 1)
                    };
                    
                    try
                    {
                        playService.ExecuteActionCard(game, currentPlayer.Id, unusedActionCard.Id, parameters);
                        Console.WriteLine("Action card executed successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Action card execution failed: {ex.Message}");
                    }
                }
            }

            // Show board state
            Console.WriteLine("\n--- Board State ---");
            Console.WriteLine($"Total tiles: {game.Board.Tiles.Count}");
            Console.WriteLine($"Spare connection pieces: {game.Board.SpareConnectionPieces}");
            
            var visitedTiles = game.Board.Tiles.Values.Count(t => t.IsVisited);
            Console.WriteLine($"Visited tiles: {visitedTiles}");

            Console.WriteLine("\n=== Demo completed successfully! ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during demo: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
