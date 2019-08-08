namespace QF.GraphDesigner
{
    public class GenericGraphData<T> : InvertGraph where T : IGraphFilter, new()
    {
        public T FilterNode
        {
            get { return (T) RootFilter; }
        }

        public override IGraphFilter CreateDefaultFilter(string identifier = null)
        {
            var filterItem = new T()
            {

            };
            filterItem.Identifier = identifier;
            Repository.Add(filterItem);
            var item = new FilterItem();
            item.NodeId = filterItem.Identifier;
            item.FilterId = filterItem.Identifier;
            Repository.Add(item);
            return filterItem;
        }
    }
}