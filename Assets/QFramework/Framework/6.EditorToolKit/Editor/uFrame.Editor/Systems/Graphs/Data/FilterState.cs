using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using QF;

[Serializable]
public class FilterState {
    [NonSerialized]
    private Stack<IGraphFilter> _filterStack = new Stack<IGraphFilter>();

 
    //// Filters
    //public IDiagramFilter CurrentFilter
    //{
    //    get
    //    {
            
    //        return FilterStack.Peek();
    //    }
    //}

    public Stack<IGraphFilter> FilterStack
    {
        get
        {
            return _filterStack ?? (_filterStack = new Stack<IGraphFilter>());
        }
        set { _filterStack = value; }
    }

    public List<string> _persistedFilterStack = new List<string>();

    public void FilterPushed(IGraphFilter filter)
    {
        if (!_persistedFilterStack.Contains(filter.Identifier))
            _persistedFilterStack.Add(filter.Identifier);
    }

    public void FilterPoped(IGraphFilter pop)
    {
        _persistedFilterStack.Remove(pop.Identifier);
    }

    public void Reload(IGraphData graphData)
    {
        // TODO 2.0: Filter Stacks?
        //if (_persistedFilterStack.Count < 1) return;
        //if (_persistedFilterStack.Count != (FilterStack.Count))
        //{
        //    foreach (var filterName in _persistedFilterStack)
        //    {
        //        var filter = graphData.Repository.GetFilters().FirstOrDefault(p => p.Identifier == filterName);
        //        if (filter == null)
        //        {
        //            _persistedFilterStack.Clear();
        //            FilterStack.Clear();
        //            break;
        //        }
                
        //        //FilterStack.Push(filter);
        //        graphData.PushFilter(filter);
        //    }
        //}
    }

}