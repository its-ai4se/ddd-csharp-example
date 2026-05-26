using OnlineTutoringSystem.Domain.Shared.ValueObjects;

public class CancellationPenalty
{
    public CancelledBy Actor { get; }
    public Money Amount { get; }
    public string Description { get; }

    private CancellationPenalty(CancelledBy actor, Money amount, string description)
    {
        Actor = actor;
        Amount = amount;
        Description = description;
    }

    public static CancellationPenalty StudentCharge(Money sessionPrice)
        => new(CancelledBy.Student, sessionPrice * 0.75m, "Student pays 75% of session price for late cancellation.");

    public static CancellationPenalty TutorDiscount(Money sessionPrice)
        => new(CancelledBy.Tutor, sessionPrice * 0.25m, "Tutor owes 25% discount on next session with this student.");
}