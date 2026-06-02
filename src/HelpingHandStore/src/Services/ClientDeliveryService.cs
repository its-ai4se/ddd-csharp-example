using HelpingHandStore.Domain.H2S;
using HelpingHandStore.Domain.Person;
using HelpingHandStore.Domain.Item;
using HelpingHandStore.Domain.Route;

namespace HelpingHandStore.Domain.Services;

public class ClientDeliveryService
{
    public static bool IsEligibleForClientDelivery(H2SAggregate h2s, ClientRole client)
    {
        return h2s.OffersClientDeliveryService && !client.CanVisitDistributionCenter;
    }

    public static IEnumerable<SecondHandArticle> GetRelevantArticlesForClient(
        H2SAggregate h2s, ClientRole client, IEnumerable<SecondHandArticle> droppedOffArticles)
    {
        if (!IsEligibleForClientDelivery(h2s, client))
        {
            return [];
        }

        return droppedOffArticles.Where(a =>
            a.CanBeDistributed() && a.Category != null && client.NeedsCategory(a.Category));
    }

    public static void ArrangeDelivery(RouteAggregate route, SecondHandArticle article)
    {
        route.AddDeliveryItem(article.Id);
    }
}
