using DestroyBlockApplication.Domain.Shared.ValueObjects;

namespace DestroyBlockApplication.Domain.User;

// Represents the result of a successful login with a chosen mode (BR-004)
public record UserSession(Guid UserId, Username Username, LoginMode Mode);
