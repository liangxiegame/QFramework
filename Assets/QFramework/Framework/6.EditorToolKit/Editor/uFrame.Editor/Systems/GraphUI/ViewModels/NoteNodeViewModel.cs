using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class NoteNodeViewModel : DiagramNodeViewModel<NoteNode>
    {
        public NoteNodeViewModel(NoteNode graphItemObject, DiagramViewModel diagramViewModel) : base(graphItemObject, diagramViewModel)
        {
        }

        public Vector2 Size
        {
            get { return GraphItem.Size; }
        }

        public string HeaderText
        {
            get { return GraphItem.HeaderText; }
        }

        public NodeColor MarkColor
        {
            get { return GraphItem.MarkColor; }
        }

        public bool ShowMark
        {
            get { return GraphItem.ShowMark; }
        }

        public override string Comments
        {
            get { return GraphItem.Comments; }
        }

        protected override void CreateContent()
        {
            ContentItems.Clear();
        }

        public override INodeStyleSchema StyleSchema
        {
            get { return CachedStyles.NodeStyleSchemaMinimalistic; }
        }

    }
}
