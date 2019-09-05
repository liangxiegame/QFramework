using QF.Json;
using QF;

namespace QF.GraphDesigner.Unity
{
    public class MarketItem : Response
    {
        public MarketItem(JSONClass item)
        {
            ProductId = item["ProductId"].Value;
            Name = item["Name"].Value;
            Price = item["Price"].AsDouble;
            SitePrice = item["SitePrice"].AsDouble;
            SubscriptionPrice = item["SubscriptionPrice"].AsDouble;
            PremiumSupportPerMonth = item["PremiumSupportPerMonth"].AsDouble;
            SiteSubscriptionPrice = item["SiteSubscriptionPrice"].AsDouble;
            PremiumSupportAvailable = item["PremiumSupportAvailable"].AsBool;
            Owned = item["Owned"].AsBool;

        }

        public string ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double SitePrice { get; set; }
        public double SubscriptionPrice { get; set; }
        public bool PremiumSupportAvailable { get; set; }
        public double PremiumSupportPerMonth { get; set; }
        public bool Owned { get; set; }
        public double SiteSubscriptionPrice { get; set; }
    }
}