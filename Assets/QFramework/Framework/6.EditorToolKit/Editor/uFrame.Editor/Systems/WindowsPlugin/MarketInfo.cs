using System.Collections.Generic;

namespace QF.GraphDesigner.Unity
{
    public class MarketInfo : Response
    {
        private List<MarketItem> _marketItems;

        public List<MarketItem> MarketItems
        {
            get { return _marketItems ?? (_marketItems = new List<MarketItem>()); }
            set { _marketItems = value; }
        }
    }
}