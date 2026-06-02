using HelpingHandStore.Domain.Shared.Common;
using HelpingHandStore.Domain.Shared.ValueObjects;
using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Person;

namespace HelpingHandStore.Domain.Services;

public class ItemProcessingService
{
    public static void ProcessSecondHandArticle(EmployeeRole employee, SecondHandArticle article, ItemCategory category, bool isUsable)
    {
        RequireEmployee(employee);

        if (isUsable)
        {
            article.TagWithRfid(new RfidCode(Guid.NewGuid().ToString("N")), category);
        }
        else
        {
            article.Discard();
        }
    }

    public static void CorrectDescription(EmployeeRole employee, SecondHandArticle article, ItemDescription correctedDescription)
    {
        RequireEmployee(employee);
        article.UpdateDescription(correctedDescription);
    }

    private static void RequireEmployee(EmployeeRole employee)
    {
        if (employee == null)
        {
            throw new DomainException("Quality inspection must be performed by an H2S employee.");
        }
    }
}
