using QF.GraphDesigner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Invert.Data;
using QF;
using QF.Json;
using UnityEngine;

public abstract class DiagramNodeItem : IDiagramNodeItem, IDataRecordRemoved
{
    public virtual string Title { get { return Name; } }

    

    public virtual string Group
    {
        get { return Node.Name; }
    }

    public override string ToString()
    {
        return Name;
    }

    public virtual string SearchTag { get { return Name; } }

    [JsonProperty, InspectorProperty]
    public virtual string Description
    {
        get { return _description; }
        set { this.Changed("Description",ref _description, value); }
    }

    string IGraphItem.Label
    {
        get { return Name; }
    }

    public bool IsValid
    {
        get { return true; }
    }

    public virtual IEnumerable<IFlagItem> DisplayedFlags
    {
        get { return Flags.OfType<IFlagItem>(); }
    } 

    public IEnumerable<FlagItem> Flags
    {
        get { return Repository.All<FlagItem>().Where(p => p.ParentIdentifier == this.Identifier); }
    }

    public bool this[string flag]
    {
        get { return Flags.Any(p => p.Name == flag); }
        set
        {
            var f = Flags.FirstOrDefault(p => p.Name == flag);
            if (value == false)
            {
                if (f != null)
                {
                    Repository.Remove(f);
                }
            }
            else
            {
                if (f == null)
                {
                    f = new FlagItem();
                    f.ParentIdentifier = this.Identifier;
                    f.Name = flag;
                    Repository.Add(f);
                }
            }
        }
    }


    public bool IsEditing { get; set; }

    private string _identifier;

    private bool _isSelected;
    private string _name = string.Empty;




    private string _oldName;

    private Rect _position;
    private string _nodeId;
    private GraphNode _node;
    private int _order;
    private string _description;

    public abstract string FullLabel { get; }

    public virtual string Highlighter { get { return null; } }
    public IRepository Repository { get; set; }

  
    public virtual string Identifier
    {
        get
        {
            return string.IsNullOrEmpty(_identifier) ? (_identifier = Guid.NewGuid().ToString()) : _identifier;
        }
        set { _identifier = value; }
    }

    public bool Changed { get; set; }

    public IEnumerable<string> ForeignKeys
    {
        get { yield return NodeId; }

    }

    public virtual bool IsSelectable { get { return true; } }

    [JsonProperty, KeyProperty]
    public string NodeId
    {
        get { return _nodeId; }
        set {
            this.Changed("NodeId", ref _nodeId, value);
            _node = null;
        }
    }

    [JsonProperty]
    public int Order
    {
        get { return _order; }
        set
        {
            this.Changed("Order", ref _order, value);
        }
    }
    public GraphNode Node
    {
        get { return _node ?? (_node = Repository.GetById<GraphNode>(NodeId)); }
        set
        {
            if (value != null) NodeId = value.Identifier;
            _node = value;
        }
    }

    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
        }
    }

    public virtual string Label
    {
        get { return Name; }
    }

    public bool Precompiled { get; set; }
    
    [JsonProperty]
    public virtual string Name
    {
        get { return _name; }
        set
        {
            //var oldName = _name;
            //if (string.IsNullOrEmpty(value))
            //{
            //    _name = "NewName";
            //    _name = value;
            //}
            if (!string.IsNullOrEmpty(value))
                _name = value;
            else
            {
                _name = "RenameMe";
            }

            if (AutoFixName)
                _name = Regex.Replace(_name, @"[^a-zA-Z0-9_\.]+", "");
            //else
            //{
            //    _name = value;
            //}

            this.Changed("Name", ref _name, value);
        }
    }

    public virtual bool AutoFixName
    {
        get { return true; }
    }

    public string OldName
    {
        get { return _oldName; }
        set { _oldName = value; }
    }

    public Rect Position
    {
        get { return _position; }
        set { _position = value; }
    }



    public virtual void BeginEditing()
    {

        OldName = Name;
    }


    public virtual void EndEditing()
    {
        
    }

    //public abstract IEnumerable<IDiagramLink> GetLinks(IDiagramNode[] diagramNode);

    public void RefactorApplied()
    {
      
    }

    public abstract void Remove(IDiagramNode diagramNode);

    public string Namespace { get; set; }

    public virtual void Rename(IDiagramNode data, string name)
    {
        Name = name;
    }

    public virtual void NodeRemoved(IDiagramNode nodeData)
    {
        if (this is ITypedItem)
        {
            var typeItem = this as ITypedItem;
            if (typeItem != null && typeItem.RelatedType == nodeData.Identifier)
            {
                typeItem.RemoveType();
            }
        }

     
            this[Identifier] = false;
   
        
    }
    
    public virtual void NodeItemRemoved(IDiagramNodeItem nodeItem)
    {
        if (this[Identifier])
        {
            this[Identifier] = false;
        }
    }

    public virtual void NodeAdded(IDiagramNode data)
    {
       
    }

    public virtual void NodeItemAdded(IDiagramNodeItem data)
    {
        
    }

    public virtual void Validate(List<ErrorInfo> info)
    {
        
    }

    public virtual bool ValidateInput(IDiagramNodeItem arg1, IDiagramNodeItem arg2)
    {
        //if (arg1.GetType() == arg2.GetType()) return false;

        return true;
    }

    public virtual bool ValidateOutput(IDiagramNodeItem arg1, IDiagramNodeItem arg2)
    {
        //if (arg1.GetType() == arg2.GetType()) return false;
        return true;
    }

    public ErrorInfo[] Errors { get; set; }

    public IGraphData Graph
    {
        get
        {
            var node = this.Node;
            if (node == null) return null;
            return node.Graph;
        }
    }

    public IEnumerable<ConnectionData> Inputs
    {
        get
        {
            foreach (var connectionData in Repository.All<ConnectionData>())
            {
                if (connectionData.InputIdentifier == this.Identifier)
                {
                    yield return connectionData;
                }
            }
        }
    }

    public IEnumerable<ConnectionData> Outputs
    {
        get
        {
            foreach (var connectionData in Repository.All<ConnectionData>())
            {
                if (connectionData.OutputIdentifier == this.Identifier)
                {
                    yield return connectionData;
                }
            }
        }
    }

    public virtual bool AllowInputs
    {
        get { return true; }
    }

    public virtual bool AllowOutputs
    {
        get { return true; }
    }

    public virtual string InputDescription
    {
        get { return null; }
    }

    public virtual string OutputDescription
    {
        get { return null; }
    }

    public virtual bool AllowMultipleInputs
    {
        get { return true; }
    }

    public virtual bool AllowMultipleOutputs
    {
        get { return true; }
    }

    public virtual Color Color
    {
        get { return Color.white; }
    }

    public virtual bool CanOutputTo(IConnectable input)
    {
        if (!AllowMultipleOutputs && this.Outputs.Any())
        {
            return false;
        }
        return true;
    }

    public virtual bool CanInputFrom(IConnectable output)
    {
        if (!AllowMultipleInputs && this.Inputs.Any())
        {
            return false;
        }
        return true;
    }

    public virtual void OnOutputConnectionRemoved(IConnectable input)
    {

    }

    public virtual void OnInputConnectionRemoved(IConnectable output)
    {

    }

    public virtual void OnConnectedToInput(IConnectable input)
    {

    }

    public virtual void OnConnectedFromOutput(IConnectable output)
    {

    }
    public virtual TType InputFrom<TType>()
    {
        return Inputs.Select(p => p.GetOutput(this.Node as IConnectableProvider)).OfType<TType>().FirstOrDefault();
    }

    public virtual IEnumerable<TType> InputsFrom<TType>()
    {
        return Inputs.Select(p => p.GetOutput(this.Node as IConnectableProvider)).OfType<TType>();
    }

    public virtual IEnumerable<TType> OutputsTo<TType>()
    {
        return Outputs.Select(p => p.GetInput(this.Node as IConnectableProvider)).OfType<TType>();
    }

    public virtual TType OutputTo<TType>()
    {
        return Outputs.Select(p => p.GetInput(this.Node as IConnectableProvider)).OfType<TType>().FirstOrDefault();
    }

    public virtual void RecordRemoved(IDataRecord record)
    {
        if (record.Identifier == NodeId)
        {
            Repository.Remove(this);
        }
    }
}