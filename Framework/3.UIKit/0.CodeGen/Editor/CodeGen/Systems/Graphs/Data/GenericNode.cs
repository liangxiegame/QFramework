using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using QF.Json;
using QF;
using UnityEngine;

namespace QFramework.CodeGen
{
    //public class InheritanceSlot<TFor> : GenericSlot
    //{
    //    [Browsable(false)]
    //    public TFor Item
    //    {
    //        get { return Inputs.Select(p => p.Output).OfType<TFor>().FirstOrDefault(); }
    //    }

    //    public override bool Validate(IDiagramNodeItem a, IDiagramNodeItem b)
    //    {
           
    //        var result = a is TFor && b is BaseClassReference && b.Node != a.Node && b.Node.GetType() == a.GetType();
    //        return result;
    //    }
    //}

    [Browsable(false)]
    public partial class GenericNode : GraphNode, IConnectable, ITypeInfo
    {
        public virtual bool IsArray { get { return false; } }

        public bool IsList
        {
            get { return false; }
        }

        public virtual bool IsEnum
        {
            get { return false; }
        }

        public ITypeInfo InnerType { get { return null; }}

        public virtual string TypeName
        {
            get
            {
                return Name;
            }
        }

        public virtual IEnumerable<IMemberInfo> GetMembers()
        {
            foreach (var item in PersistedItems.OfType<IMemberInfo>())
            {
                yield return item;
            }
        }

        public virtual bool IsAssignableTo(ITypeInfo info)
        {
            return info.FullName == FullName;
        }

        public virtual ITypeInfo BaseTypeInfo
        {
            get { return null; }
        }

        public bool HasAttribute(Type attribute)
        {
            if (BaseTypeInfo == null)
            {
                return false;
            }
            return BaseTypeInfo.HasAttribute(attribute);
        }

        public virtual void MoveItemDown(IDiagramNodeItem nodeItem)
        {
            // TODO 2.0 ORDERING
            //var items = ChildItems.Where(p => p.GetType() == nodeItem.GetType()).ToList();
            //ChildItems.RemoveAll(p => items.Contains(p));

            //items.Move(items.IndexOf(nodeItem),false);
            //ChildItems.AddRange(items);

        }

        public virtual void MoveItemUp(IDiagramNodeItem nodeItem)
        {
            // TODO 2.0 Children re-ordering
            //var items = ChildItems.Where(p => p.GetType() == nodeItem.GetType()).ToList();
            //ChildItems.RemoveAll(p => items.Contains(p));

            //items.Move(items.IndexOf(nodeItem), true);
            //ChildItems.AddRange(items);

        }


        private List<string> _connectedGraphItemIds = new List<string>();

        [Browsable(false)]
        public NodeConfigBase Config
        {
            get
            {
                var config = InvertApplication.Container.Resolve<NodeConfigBase>(this.GetType().Name);
                if (config == null)
                {
                    throw new Exception("Config for type " + this.GetType().Name + " couldn't be found.");
                }
                return config;
            }
        }

        public override string SubTitle
        {
            get { return Config.Name; }
        }

        [JsonProperty]
        public override string Name
        {
            get { return base.Name; }
            set { 
                base.Name = value;
                
            }
        }
        //public List<string> ConnectedGraphItemIds
        //{
        //    get { return _connectedGraphItemIds; }
        //    set { _connectedGraphItemIds = value; }
        //}

        //public IEnumerable<IGraphItem> ConnectedGraphItems
        //{
        //    get
        //    {
        //        foreach (var item in Project.NodeItems)
        //        {
        //            if (ConnectedGraphItemIds.Contains(item.Identifier))
        //                yield return item;

        //            foreach (var child in item.ContainedItems)
        //            {
        //                if (ConnectedGraphItemIds.Contains(child.Identifier))
        //                {
        //                    yield return child;
        //                }
        //            }
        //        }
        //    }
        //}

        [Browsable(false)]
        public override string Label
        {
            get { return Name; }
        }



        public void AddReferenceItem(IGraphItem item, NodeConfigSectionBase mirrorSection)
        {
            AddReferenceItem(PersistedItems.Where(p => p.GetType() == mirrorSection.ReferenceType).Cast<GenericReferenceItem>().ToArray(), item, mirrorSection);
        }

        public TType GetConnectionReference<TType>()
            where TType : GenericSlot, new()
        {
            return (TType)GetConnectionReference(typeof(TType));
        }

        public GenericSlot GetConnectionReference(Type inputType)
        {
            var item = PersistedItems.FirstOrDefault(p => inputType.IsAssignableFrom(p.GetType()));
            if (item == null)
            {
                var input = Activator.CreateInstance(inputType) as GenericSlot;
                Repository.Add(input);
                input.Node = this;
                return input;
            }

            return item as GenericSlot;
        }

        //public IEnumerable<TItem> GetConnections<TConnectionType, TItem>() where TConnectionType : GenericConnectionReference, new()
        //{
        //    return GetConnectionReference<TConnectionType>().ConnectedGraphItems.Cast<TItem>();
        //}

        //public IEnumerable<TChildItem> GetInputChildItems<TSourceNode, TChildItem>()
        //    where TSourceNode : GenericNode
        //{
        //    return InputGraphItems.OfType<TSourceNode>().SelectMany(p => p.ContainedItems.OfType<TChildItem>());
        //}

        //public IEnumerable<TChildItem> GetInputInheritedChildItems<TSourceNode, TChildItem>()
        //  where TSourceNode : GenericInheritableNode
        //{
        //    return InputGraphItems.OfType<TSourceNode>().SelectMany(p => p.ChildItemsWithInherited.OfType<TChildItem>());
        //}

        public override void NodeAddedInFilter(IDiagramNode newNodeData)
        {
            base.NodeAddedInFilter(newNodeData);
        }

        public virtual void NodeItemAdded(IDiagramNodeItem data)
        {
            UpdateReferences();
        }

        public virtual void NodeItemRemoved(IDiagramNodeItem diagramNodeItem)
        {
            // TODO 2.0
            //UpdateReferences();
            //Repository.RemoveAll<GenericReferenceItem>(
            //    p =>
            //        p.Identifier == diagramNodeItem.Identifier || p.SourceIdentifier == diagramNodeItem.Identifier));
        }
        [Browsable(false)]
        public override bool IsValid
        {
            get { return Config.IsValid(this); }
        }

        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);

            if (this is IClassNode)//Repository.AllOf<IClassNode>().Any(p => p != this && p.FullName == this.FullName))
            {
                if (Repository.AllOf<IClassNode>().Any(p => p != this && p.FullName == this.FullName))
                errors.AddError(string.Format("The name {0} is already taken", this.FullName + ":" + this.GetType().Name), this);
            }
            foreach (var slot in AllInputSlots)
            {
                slot.Validate(errors);
            }
            
        }

        public override void NodeRemoved(IDiagramNode nodeData)
        {
            base.NodeRemoved(nodeData);

        }



        private void UpdateReferences()
        {
            // TODO 2.0: Reference Handling
            //foreach (var mirrorSection in Config.Sections.Where(p => p.ReferenceType != null && !p.AllowAdding))
            //{
            //    NodeConfigSectionBase section = mirrorSection;
            //    var mirrorItems = ChildItems.Where(p => p.GetType() == section.ReferenceType).Cast<GenericReferenceItem>().ToArray();
            //    var newItems = mirrorSection.GenericSelector(this).ToArray();
            //    var newItemIds = newItems.Select(p => p.Identifier);

            //    foreach (var item in newItems)
            //    {
            //        if (PersistedItems.OfType<GenericReferenceItem>().Any(p => p.SourceIdentifier == item.Identifier)) continue;
            //        AddReferenceItem(mirrorItems, item, mirrorSection);
            //    }
            //    var items = ChildItems.Where(p => p.GetType() == section.SourceType && p is GenericReferenceItem && !newItemIds.Contains(((GenericReferenceItem)p).SourceIdentifier)).ToArray();
            //    foreach (var item in items)
            //    {
            //        Node.Project.RemoveItem(item);
            //    }
            //}
        }

        internal void AddReferenceItem(GenericReferenceItem[] mirrorItems, IGraphItem item, NodeConfigSectionBase mirrorSection)
        {
            
            var current = mirrorItems.FirstOrDefault(p => p.SourceIdentifier == item.Identifier);
            if (current != null && !mirrorSection.AllowDuplicates) return;

            var newMirror = Activator.CreateInstance(mirrorSection.SourceType) as GenericReferenceItem;
            newMirror.Node = this;
            Node.Repository.Add(newMirror);
            newMirror.SourceIdentifier = item.Identifier;
       
     
            //Node.Project.AddItem(newMirror);
        }
        [Browsable(false)]
        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                foreach (var item in this.PersistedItems)
                {
                    yield return item;
                }
                foreach (var item in AllSlots)
                {
                    yield return item;
                }
            }
        }
        [Browsable(false)]
        public virtual IEnumerable<GenericSlot> AllInputSlots
        {
            get
            {
                foreach (var slot in Config.GraphItemConfigurations.OfType<NodeInputConfig>())
                {
                    if (!slot.IsInput) continue;
                    yield return slot.GetDataObject(this) as GenericSlot;
                }

            }
        }
        [Browsable(false)]
        public virtual IEnumerable<GenericSlot> AllOutputSlots
        {
            get
            {
                foreach (var slot in Config.GraphItemConfigurations.OfType<NodeInputConfig>())
                {
                    if (!slot.IsOutput) continue;
                    yield return slot.GetDataObject(this) as GenericSlot;
                }

            }
        }
        [Browsable(false)]
        public virtual IEnumerable<GenericSlot> AllSlots
        {
            get
            {
                
                foreach (var slot in Config.GraphItemConfigurations.OfType<NodeInputConfig>())
                {
                    yield return slot.GetDataObject(this) as GenericSlot;
                }
                
            }
        }

        protected TSlotType GetSlot<TSlotType>(ref TSlotType field, string name, string identifier = null, Action<TSlotType> initialize = null) where TSlotType : GenericSlot, new()
        {
            if (field != null) return field;
            field = CreateSlot<TSlotType>(name, identifier);
            if (initialize != null)
            {
                initialize(field);
            }
            return field;
        }

        protected TSlotType CreateSlot<TSlotType>(string name, string identifier = null) where TSlotType : GenericSlot, new()
        {
            return new TSlotType()
            {
                Node = this,
                Name = name,
                Identifier = identifier == null?  this.Identifier + ":" + name : this.Identifier + ":" + identifier,
                Repository = Repository,
            };
        }
    }
}