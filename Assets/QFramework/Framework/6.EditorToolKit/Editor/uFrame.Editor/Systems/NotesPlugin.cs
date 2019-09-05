using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF;

namespace QF.GraphDesigner
{
    public class NotesPlugin : DiagramPlugin
    {
        public override decimal LoadPriority
        {
            get { return 1000; }
        }

        public override void Initialize(QFrameworkContainer container)
        {
            base.Initialize(container);
         
            container.AddNode<NoteNode, NoteNodeViewModel, NoteNodeDrawer>("Note");
            container.AddNode<ImageNode, ImageNodeViewModel, ImageNodeDrawer>("Image");
            
            foreach (var node in FilterExtensions.AllowedFilterNodes)
            {
                node.Value.Add(typeof(NoteNode));       
                node.Value.Add(typeof(ImageNode));       
            }
        }
    }
}
