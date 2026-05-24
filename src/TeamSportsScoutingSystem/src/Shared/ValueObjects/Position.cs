using TeamSportsScoutingSystem.Domain.Shared.Common;

namespace TeamSportsScoutingSystem.Domain.Shared.ValueObjects;

public class Position : ValueObject
{
    public string Code { get; }

    public Position(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Position code cannot be empty or whitespace.", nameof(code));
        Code = code.Trim().ToUpper();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
