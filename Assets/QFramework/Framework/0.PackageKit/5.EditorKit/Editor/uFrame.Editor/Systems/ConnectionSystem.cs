using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.GraphDesigner;
using Invert.Data;
using QF;

namespace QF.GraphDesigner
{
    
    public class ConnectionSystem : DiagramPlugin
        ,IContextMenuQuery
    {
        public override void Initialize(QFrameworkContainer container)
        {
            base.Initialize(container);
        }

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, params object[] obj)
        {
            var connector = obj.FirstOrDefault() as ConnectorViewModel;
            if (connector != null)
            {
                var connections =
                   InvertGraphEditor.CurrentDiagramViewModel.GraphItems.OfType<ConnectionViewModel>()
                       .Where(p => p.ConnectorA == connector || p.ConnectorB == connector).ToArray();

                foreach (var connection in connections)
                {
                    ConnectionViewModel connection1 = connection;
                     ui.AddCommand(new ContextMenuItem()
                        {
                            Title = string.Format("Remove {0}",connection1.Name),
                            Group="Remove",
                            Command = new LambdaCommand("Remove Connection", ()=> { connection1.Remove(connection1); })
                        });
             
                }
                
            }
        }
    }
}
