namespace DestroyBlockApplication.Domain.Shared.Common;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
