using TileOApplication.Domain.Game;
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
            var game = new GameAggregate();
            var designService = new GameDesignService();
            var playService = new GamePlayService();

            designService.SetupDefaultPlayers(game);
            designService.CreateDefaultActionCardDeck(game);
            designService.CreateSampleBoard(game);

            Console.WriteLine($"Board: {game.Board.Tiles.Count} tiles, hidden at {game.Board.HiddenTilePosition}");
            Console.WriteLine($"Spare pieces: {game.Board.SpareConnectionPieces}");

            // BR-001/BR-002: in production use GameSessionService; direct call here for demo
            game.StartGame();
            Console.WriteLine($"Game started. Status: {game.Status}");

            var currentPlayer = game.Players.First(p => p.Id == game.CurrentPlayerId);
            Console.WriteLine($"Current player: {currentPlayer} at {currentPlayer.CurrentPosition}");

            // BR-015: Must roll dice before moving
            var roll = playService.RollDice(game);
            Console.WriteLine($"Dice roll: {roll}");

            var validMoves = playService.GetValidMoves(game, currentPlayer.Id);
            Console.WriteLine($"Valid moves: {validMoves.Count}");

            if (validMoves.Count > 0)
            {
                var target = validMoves[0];
                Console.WriteLine($"Moving to: {target}");
                game.MovePlayer(currentPlayer.Id, target);
                Console.WriteLine($"Game status: {game.Status}");
                if (game.Status == GameStatus.Completed)
                    Console.WriteLine($"Winner: {game.Players.First(p => p.Id == game.WinnerId)}");
            }

            // BR-021: Board view hides action/hidden tile identity
            Console.WriteLine("\n--- Board View (player perspective) ---");
            foreach (var pos in game.Board.Tiles.Keys.Take(4))
            {
                var view = game.Board.GetTileView(pos);
                Console.WriteLine($"  {pos} => {view?.DisplayType}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
