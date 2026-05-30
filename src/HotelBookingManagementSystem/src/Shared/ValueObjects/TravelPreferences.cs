using HotelBookingManagementSystem.Domain.Shared.Common;

namespace HotelBookingManagementSystem.Domain.Shared.ValueObjects;

public class TravelPreferences : ValueObject
{
    public bool BreakfastIncluded { get; }
    public bool FreeWifi { get; }
    public bool FrontDesk24Hours { get; }
    public bool ParkingAvailable { get; }
    public bool PetFriendly { get; }
    public bool FitnessCenter { get; }
    public bool Pool { get; }
    public bool BusinessCenter { get; }

    public TravelPreferences(
        bool breakfastIncluded = false,
        bool freeWifi = false,
        bool frontDesk24Hours = false,
        bool parkingAvailable = false,
        bool petFriendly = false,
        bool fitnessCenter = false,
        bool pool = false,
        bool businessCenter = false)
    {
        BreakfastIncluded = breakfastIncluded;
        FreeWifi = freeWifi;
        FrontDesk24Hours = frontDesk24Hours;
        ParkingAvailable = parkingAvailable;
        PetFriendly = petFriendly;
        FitnessCenter = fitnessCenter;
        Pool = pool;
        BusinessCenter = businessCenter;
    }

    public bool HasAnyPreferences()
    {
        return BreakfastIncluded || FreeWifi || FrontDesk24Hours || ParkingAvailable ||
               PetFriendly || FitnessCenter || Pool || BusinessCenter;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BreakfastIncluded;
        yield return FreeWifi;
        yield return FrontDesk24Hours;
        yield return ParkingAvailable;
        yield return PetFriendly;
        yield return FitnessCenter;
        yield return Pool;
        yield return BusinessCenter;
    }
}
