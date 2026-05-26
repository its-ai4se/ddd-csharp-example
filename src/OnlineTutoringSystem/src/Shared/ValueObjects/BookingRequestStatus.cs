using OnlineTutoringSystem.Domain.Shared.Common;

namespace OnlineTutoringSystem.Domain.Shared.ValueObjects;

public sealed class BookingRequestStatus : ValueObject
{
    public static readonly BookingRequestStatus Pending = new("Pending", skipValidation: true);
    public static readonly BookingRequestStatus TutorProposed = new("TutorProposed", skipValidation: true);
    public static readonly BookingRequestStatus Confirmed = new("Confirmed", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Pending", "TutorProposed", "Confirmed"];

    public string Value { get; }

    public BookingRequestStatus(string value) : this(value, skipValidation: false) { }

    private BookingRequestStatus(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Booking request status cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Booking request status '{value}' is not valid. Use Pending, TutorProposed, or Confirmed.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
