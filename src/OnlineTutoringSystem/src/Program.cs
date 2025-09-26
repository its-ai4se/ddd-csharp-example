using OnlineTutoringSystem.Domain.Services;
using OnlineTutoringSystem.Domain.Shared.Services;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Online Tutoring System - Domain Model Demo");
        Console.WriteLine("==========================================");

        var clock = new SystemClock();
        
        Console.WriteLine("Domain model created successfully!");
        Console.WriteLine("Key features:");
        Console.WriteLine("- Person management with Tutor and Student roles");
        Console.WriteLine("- Course creation and management");
        Console.WriteLine("- Session scheduling and management");
        Console.WriteLine("- Payment processing");
        Console.WriteLine("- Rich domain model with business rules");
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
