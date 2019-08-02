namespace QF.GraphDesigner
{
    public interface IGraphItemEvents
    {
        void GraphItemCreated(IGraphItem node);
        void GraphItemRemoved(IGraphItem node);
        void GraphItemRenamed(IGraphItem node);
    }
}