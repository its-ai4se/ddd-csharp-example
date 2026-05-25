using SmartHomeAutomationSystem.Domain.Shared.Common;

namespace SmartHomeAutomationSystem.Domain.Automation.Precondition;

/// <summary>
/// Validates precondition expression strings for allowed Boolean operators (BR-010).
/// Only AND, OR, NOT are allowed. XOR and others are rejected.
/// </summary>
public static class PreconditionParser
{
    private static readonly HashSet<string> AllowedOperators = ["AND", "OR", "NOT"];
    private static readonly HashSet<string> KnownInvalidOperators = ["XOR", "NAND", "NOR", "XNOR"];

    public static void Validate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new DomainException("Precondition expression cannot be empty.");

        var tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            var upper = token.ToUpperInvariant();
            if (KnownInvalidOperators.Contains(upper))
                throw new DomainException($"Operator '{token}' is not recognized. Only AND, OR, NOT are allowed.");
        }
    }
}
