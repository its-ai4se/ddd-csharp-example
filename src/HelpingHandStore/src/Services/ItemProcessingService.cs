using HelpingHandStore.Domain.Shared.Services;
using HelpingHandStore.Domain.Shared.ValueObjects;
using HelpingHandStore.Domain.Item;

namespace HelpingHandStore.Domain.Services;

public class ItemProcessingService : DomainServiceBase
{
    public ItemProcessingService(IClock clock) : base(clock)
    {
    }

    public void ProcessSecondHandArticle(SecondHandArticle article, ItemCategory category, bool isUsable)
    {
        if (isUsable)
        {
            var rfidCode = GenerateRfidCode();
            article.TagWithRfid(rfidCode, category);
        }
        else
        {
            article.Discard();
        }
    }

    public bool ShouldDeliverToClient(SecondHandArticle article, IEnumerable<ItemCategory> clientNeededCategories)
    {
        return article.CanBeDistributed() && clientNeededCategories.Contains(article.Category);
    }

    private RfidCode GenerateRfidCode()
    {
        // Generate a random 16-character alphanumeric RFID code
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var code = new string(Enumerable.Repeat(chars, 16)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        
        return new RfidCode(code);
    }
}
