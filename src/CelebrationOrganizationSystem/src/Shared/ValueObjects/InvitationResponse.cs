using CelebrationOrganizationSystem.Domain.Shared.Common;

namespace CelebrationOrganizationSystem.Domain.Shared.ValueObjects;

public enum InvitationStatus
{
    WillAttend,
    MaybeWillAttend,
    CannotAttend
}

public class InvitationResponse : ValueObject
{
    public InvitationStatus Status { get; }

    public InvitationResponse(InvitationStatus status)
    {
        Status = status;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Status;
    }

    public override string ToString() => Status.ToString();
}
