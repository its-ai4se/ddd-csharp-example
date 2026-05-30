using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class ReliabilityRating : ValueObject
{
    public decimal Score { get; }

    public ReliabilityRating(int totalBookings, int completedBookings, int cancelledBookings)
    {
        if (totalBookings < 0)
            throw new ArgumentException("Total bookings cannot be negative.", nameof(totalBookings));
        if (completedBookings < 0 || cancelledBookings < 0)
            throw new ArgumentException("Completed and cancelled bookings cannot be negative.");
        if (completedBookings + cancelledBookings > totalBookings)
            throw new ArgumentException("Completed and cancelled bookings cannot exceed total bookings.");

        if (totalBookings == 0)
        {
            Score = 0; // No bookings yet — no rating
        }
        else
        {
            var completionRate = (decimal)completedBookings / totalBookings;
            var cancellationPenalty = (decimal)cancelledBookings / totalBookings * 0.1m;
            Score = Math.Round(Math.Max(0, Math.Min(1, completionRate - cancellationPenalty)), 2);
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Score;
    }

    public override string ToString() => Score == 0 ? "No rating" : $"{Score:P0}";
}
