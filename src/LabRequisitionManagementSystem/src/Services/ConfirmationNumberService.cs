using LabRequisitionManagementSystem.Domain.Shared.Services;
using LabRequisitionManagementSystem.Domain.Shared.ValueObjects;

namespace LabRequisitionManagementSystem.Domain.Services;

public class ConfirmationNumberService : DomainServiceBase
{
    public ConfirmationNumberService(IClock clock) : base(clock)
    {
    }

    public ConfirmationNumber GenerateConfirmationNumber()
    {
        // Generate a unique confirmation number
        var timestamp = Clock.Now.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        var confirmationNumber = $"APT{timestamp}{random}";
        
        return new ConfirmationNumber(confirmationNumber);
    }

    public bool IsValidConfirmationNumber(string confirmationNumber)
    {
        try
        {
            var confNumber = new ConfirmationNumber(confirmationNumber);
            return confNumber.Value.StartsWith("APT") && confNumber.Value.Length >= 15;
        }
        catch
        {
            return false;
        }
    }
}
