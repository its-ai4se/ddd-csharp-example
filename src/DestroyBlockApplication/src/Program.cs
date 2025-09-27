using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("DestroyBlockApplication Domain Model Demo");
        Console.WriteLine("========================================");
        
        // Demo basic value objects
        DemoValueObjects();
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void DemoValueObjects()
    {
        Console.WriteLine("\n--- Value Objects Demo ---");
        
        try
        {
            var username = new Username("player123");
            var password = new Password("secret123");
            var gameName = new GameName("Epic Destroy Block");
            var score = new Score(150);
            var levelNumber = new LevelNumber(5);
            var position = new GridPosition(3, 4);
            var color = new Color("Red");
            var speed = new Speed(2.5);
            var paddleLength = new PaddleLength(10.0);
            var lives = new Lives(3);

            Console.WriteLine($"Username: {username}");
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Game Name: {gameName}");
            Console.WriteLine($"Score: {score}");
            Console.WriteLine($"Level Number: {levelNumber}");
            Console.WriteLine($"Grid Position: {position}");
            Console.WriteLine($"Color: {color}");
            Console.WriteLine($"Speed: {speed}");
            Console.WriteLine($"Paddle Length: {paddleLength}");
            Console.WriteLine($"Lives: {lives}");
            Console.WriteLine($"Is Alive: {lives.IsAlive}");

            // Demo operations
            var newScore = score + new Score(50);
            Console.WriteLine($"Score after adding 50: {newScore}");
            
            var newLevel = ++levelNumber;
            Console.WriteLine($"Next level: {newLevel}");
            
            var newLives = --lives;
            Console.WriteLine($"Lives after losing one: {newLives}");
            Console.WriteLine($"Still alive: {newLives.IsAlive}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
