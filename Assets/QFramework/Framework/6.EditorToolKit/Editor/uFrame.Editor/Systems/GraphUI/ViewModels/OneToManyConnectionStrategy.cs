namespace QF.GraphDesigner
{
    //public class OneToManyConnectionStrategy<TSource,TTarget> : 
    //    DefaultConnectionStrategy<TSource,TTarget>
    //    where TSource : class, IConnectable
    //    where TTarget : class, IConnectable
    //{
    //    private Color _connectionColor;

    //    public OneToManyConnectionStrategy() : this(Color.white)
    //    {
    //    }

    //    public OneToManyConnectionStrategy(Color connectionColor )
    //    {
    //        _connectionColor = connectionColor;
    //    }

    //    public override Color ConnectionColor
    //    {
    //        get { return _connectionColor; }
            
    //    }

    //    public override ConnectionViewModel Connect(DiagramViewModel diagramViewModel, ConnectorViewModel a, ConnectorViewModel b)
    //    {
    //        return base.Connect(diagramViewModel, a, b);
    //    }

    //    public override bool IsConnected(TSource output, TTarget input)
    //    {
            
    //        return output.ConnectedGraphItemIds.Contains(input.Identifier);
    //    }

    //    protected override void ApplyConnection(TSource output, TTarget input)
    //    {
    //        if (output.ConnectedGraphItemIds.Contains(input.Identifier)) return;
    //        output.ConnectedGraphItemIds.Add(input.Identifier);
    //    }

    //    protected override void RemoveConnection(TSource output, TTarget input)
    //    {
    //        output.ConnectedGraphItemIds.Remove(input.Identifier);
    //    }
    //}



}