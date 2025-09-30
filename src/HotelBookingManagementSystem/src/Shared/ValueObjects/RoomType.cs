using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class RoomType : ValueObject
{
    public string Name { get; }
    public int MaxOccupancy { get; }
    public bool HasSingleBed { get; }
    public bool HasDoubleBed { get; }
    public bool HasTwinBeds { get; }

    public RoomType(string name, int maxOccupancy, bool hasSingleBed = false, bool hasDoubleBed = false, bool hasTwinBeds = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Room type name cannot be empty or whitespace.", nameof(name));
        }

        if (maxOccupancy <= 0)
        {
            throw new ArgumentException("Max occupancy must be greater than 0.", nameof(maxOccupancy));
        }

        Name = name.Trim();
        MaxOccupancy = maxOccupancy;
        HasSingleBed = hasSingleBed;
        HasDoubleBed = hasDoubleBed;
        HasTwinBeds = hasTwinBeds;
    }

    public static RoomType Single => new("Single", 1, hasSingleBed: true);
    public static RoomType Double => new("Double", 2, hasDoubleBed: true);
    public static RoomType Twin => new("Twin", 2, hasTwinBeds: true);
    public static RoomType Suite => new("Suite", 4, hasDoubleBed: true, hasTwinBeds: true);

    public string GetBedDescription()
    {
        var beds = new List<string>();
        if (HasSingleBed) beds.Add("Single bed");
        if (HasDoubleBed) beds.Add("Double bed");
        if (HasTwinBeds) beds.Add("Twin beds");
        
        return beds.Count > 0 ? string.Join(", ", beds) : "Standard bedding";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return MaxOccupancy;
        yield return HasSingleBed;
        yield return HasDoubleBed;
        yield return HasTwinBeds;
    }

    public override string ToString() => $"{Name} (Max {MaxOccupancy} guests)";
}
