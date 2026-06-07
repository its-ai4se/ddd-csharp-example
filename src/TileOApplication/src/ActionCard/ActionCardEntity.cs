using TileOApplication.Domain.Shared.Common;
using TileOApplication.Domain.Shared.ValueObjects;

namespace TileOApplication.Domain.ActionCard;

// BR-011: Action cards from five predefined types
public class ActionCardEntity : Entity
{
    public ActionCardDescription Description { get; private set; }
    public bool IsUsed { get; private set; }

    public ActionCardEntity(ActionCardDescription description) : base()
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        IsUsed = false;
    }

    public void Use()
    {
        if (IsUsed)
            throw new InvalidOperationException("Action card has already been used.");
        IsUsed = true;
    }
}
