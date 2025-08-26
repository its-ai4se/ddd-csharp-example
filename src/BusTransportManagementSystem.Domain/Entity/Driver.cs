using BusTransportManagementSystem.Domain.ValueObject;

namespace BusTransportManagementSystem.Domain.Entity;

public class Driver : IEquatable<Driver>
{
    public Guid Id { get; }
    public DriverName Name { get; private set; }
    public SickLeaveStatus SickLeaveStatus { get; private set; }

    public Driver(Guid id, DriverName name, SickLeaveStatus? sickLeaveStatus = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Driver ID cannot be empty.", nameof(id));
        }

        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        SickLeaveStatus = sickLeaveStatus ?? SickLeaveStatus.Active;
    }

    public Driver(DriverName name, SickLeaveStatus? sickLeaveStatus = null)
        : this(Guid.NewGuid(), name, sickLeaveStatus)
    {
    }
    
    public void UpdateName(DriverName newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    public void SetSickLeave()
    {
        SickLeaveStatus = SickLeaveStatus.OnSickLeave;
    }

    public void ClearSickLeave()
    {
        SickLeaveStatus = SickLeaveStatus.Active;
    }

    public bool IsAvailable() => SickLeaveStatus.IsActive();

    public bool IsOnSickLeave() => SickLeaveStatus.IsOnSickLeave();

    public bool Equals(Driver? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => obj is Driver other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => $"Driver: {Name} (ID: {Id}, Status: {SickLeaveStatus})";

    public static bool operator ==(Driver left, Driver right) => Equals(left, right);

    public static bool operator !=(Driver left, Driver right) => !Equals(left, right);
}
