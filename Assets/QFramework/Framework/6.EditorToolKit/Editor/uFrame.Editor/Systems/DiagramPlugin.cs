using System;
using QF.GraphDesigner;
using QF;
using QFramework;

namespace QF.GraphDesigner
{
    public abstract class DiagramPlugin : CorePlugin, IDiagramPlugin
    {
        public void Signal<TInterface>(Action<TInterface> invoke) where TInterface : class
        {
            InvertApplication.SignalEvent<TInterface>(invoke);
        }

        public void ListenFor<TEvents>() where TEvents : class
        {
            InvertApplication.ListenFor<TEvents>(this);
        }
        public override bool Enabled
        {
            get
            {
                if (InvertGraphEditor.Prefs == null) return true; // Testability
                return InvertGraphEditor.Prefs.GetBool("UFRAME_PLUGIN_" + this.GetType().Name, EnabledByDefault);
            }
            set { InvertGraphEditor.Prefs.SetBool("UFRAME_PLUGIN_" + this.GetType().Name, value); }
        }

        public override void Loaded(QFrameworkContainer container)
        {

        }

   
    }

}