using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class ReliabilityRating : ValueObject
{
    public decimal Score { get; }

    public ReliabilityRating(decimal score)
    {
        if (score < 0 || score > 1)
        {
            throw new ArgumentException("Reliability rating must be between 0 and 1.", nameof(score));
        }

        Score = Math.Round(score, 2);
    }

    public ReliabilityRating(int totalBookings, int completedBookings, int cancelledBookings)
    {
        if (totalBookings < 0)
        {
            throw new ArgumentException("Total bookings cannot be negative.", nameof(totalBookings));
        }

        if (completedBookings < 0 || cancelledBookings < 0)
        {
            throw new ArgumentException("Completed and cancelled bookings cannot be negative.");
        }

        if (completedBookings + cancelledBookings > totalBookings)
        {
            throw new ArgumentException("Completed and cancelled bookings cannot exceed total bookings.");
        }

        // For new travellers with no bookings, start with neutral rating
        if (totalBookings == 0)
        {
            Score = 0.5m; // Neutral starting point
        }
        else
        {
            // Calculate reliability based on completion rate and cancellation penalty
            var completionRate = (decimal)completedBookings / totalBookings;
            var cancellationPenalty = (decimal)cancelledBookings / totalBookings * 0.1m; // 10% penalty per cancellation
            
            Score = Math.Max(0, Math.Min(1, completionRate - cancellationPenalty));
        }
        
        Score = Math.Round(Score, 2);
    }

    public string GetRatingDescription()
    {
        return Score switch
        {
            >= 0.9m => "Excellent",
            >= 0.8m => "Good",
            >= 0.7m => "Fair",
            >= 0.6m => "Poor",
            _ => "Very Poor"
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Score;
    }

    public override string ToString() => $"{Score:P0} ({GetRatingDescription()})";
}
