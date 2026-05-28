using DestroyBlockApplication.Domain.Shared.Common;

namespace DestroyBlockApplication.Domain.Shared.ValueObjects;

public sealed class LoginMode : ValueObject
{
    public static readonly LoginMode Player = new("Player", skipValidation: true);
    public static readonly LoginMode Admin = new("Admin", skipValidation: true);

    private static readonly HashSet<string> _valid = ["Player", "Admin"];

    public string Value { get; }

    public LoginMode(string value) : this(value, skipValidation: false) { }

    private LoginMode(string value, bool skipValidation)
    {
        if (!skipValidation)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Login mode cannot be empty.");
            if (!_valid.Contains(value))
                throw new DomainException($"Login mode '{value}' is not valid. Use Player or Admin.");
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents() { yield return Value; }
    public override string ToString() => Value;
}
