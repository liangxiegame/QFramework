using System.Collections.Generic;
using QF.GraphDesigner;
using QF;
using QF.Json;
using UnityEngine;

public class FilterPositionData : IJsonObject
{
    private Dictionary<string, FilterLocations> _positions;

    public Dictionary<string, FilterLocations> Positions
    {
        get { return _positions ?? (_positions = new Dictionary<string, FilterLocations>()); }
        set { _positions = value; }
    }
    
    public bool HasPosition(IGraphFilter filter, IDiagramNode node)
    {
        if (Positions.ContainsKey(filter.Identifier))
        {
            var filterData = Positions[filter.Identifier];
            if (filterData.Keys.Contains(node.Identifier)) return true;
        }
        return false;
    }
    public Vector2 this[IGraphFilter filter, IDiagramNode node]
    {
        get
        {
            if (Positions.ContainsKey(filter.Identifier))
            {
                var filterData = Positions[filter.Identifier];
                if (filterData.Keys.Contains(node.Identifier))
                    return filterData[node];


            }
            return Vector2.zero;
        }
        set
        {
            if (!Positions.ContainsKey(filter.Identifier))
            {
                Positions.Add(filter.Identifier, new FilterLocations());
            }

            Positions[filter.Identifier][node] = value;
        }
    }
    public Vector2 this[IGraphFilter filter, string node]
    {
        get
        {
            if (Positions.ContainsKey(filter.Identifier))
            {
                var filterData = Positions[filter.Identifier];
                if (filterData.Keys.Contains(node))
                    return filterData[node];


            }
            return Vector2.zero;
        }
        set
        {
            if (!Positions.ContainsKey(filter.Identifier))
            {
                Positions.Add(filter.Identifier, new FilterLocations());
            }

            Positions[filter.Identifier][node] = value;
        }
    }
    public void Serialize(JSONClass cls)
    {
        foreach (var item in Positions)
        {
            cls.Add(item.Key, item.Value.Serialize());
        }
    }

    public void Deserialize(JSONClass cls)
    {

        Positions.Clear();
        foreach (KeyValuePair<string, JSONNode> cl in cls)
        {
            var locations = new FilterLocations();
            if (!(cl.Value is JSONClass)) continue;
            locations.Deserialize(cl.Value.AsObject);
            Positions.Add(cl.Key, locations);
        }
    }

    public void Remove(IGraphFilter currentFilter, string identifier)
    {
        Positions[currentFilter.Identifier].Remove(identifier);
    }
}