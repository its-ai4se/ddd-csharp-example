using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class HotelRating : ValueObject
{
    public int Stars { get; }

    public HotelRating(int stars)
    {
        if (stars < 1 || stars > 5)
        {
            throw new ArgumentException("Hotel rating must be between 1 and 5 stars.", nameof(stars));
        }

        Stars = stars;
    }

    public static implicit operator int(HotelRating rating) => rating.Stars;

    public static implicit operator HotelRating(int stars) => new(stars);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Stars;
    }

    public override string ToString() => $"{Stars} star{(Stars == 1 ? "" : "s")}";
}
