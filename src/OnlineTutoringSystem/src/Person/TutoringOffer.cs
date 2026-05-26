using OnlineTutoringSystem.Domain.Shared.Common;
using OnlineTutoringSystem.Domain.Shared.ValueObjects;

namespace OnlineTutoringSystem.Domain.Person;

public class TutoringOffer : ValueObject
{
    public Subject Subject { get; private set; }
    public ExpertiseLevel Level { get; private set; }
    public Money HourlyPrice { get; private set; }

    public TutoringOffer(Subject subject, ExpertiseLevel level, Money hourlyPrice)
    {
        if (subject is null) throw new DomainException("Subject is required.");
        if (level is null) throw new DomainException("Expertise level is required.");
        Subject = subject;
        Level = level;
        HourlyPrice = hourlyPrice ?? throw new DomainException("Hourly price is required.");
        if (hourlyPrice.Amount <= 0)
            throw new DomainException("Hourly price must be greater than zero.");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Subject;
        yield return Level;
    }

    public override string ToString() => $"{Subject} ({Level}) - {HourlyPrice}/hr";
}
