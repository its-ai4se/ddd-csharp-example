using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class RoomType : ValueObject
{
    public string Name { get; }

    public RoomType(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Room type name cannot be empty or whitespace.", nameof(name));

        Name = name.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }

    public override string ToString() => Name;
}
