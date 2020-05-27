using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class UIDefaultPanel : UIPanel
    {
        private Dictionary<string, Bind> mBinds;

        public Bind GetBind(string name)
        {
            return mBinds[name];
        }
        
        protected override void OnInit(IUIData uiData = null)
        {
            var binds = GetComponentsInChildren<Bind>();

            mBinds = binds.ToDictionary(b => b.name);
        }

        protected override void OnClose()
        {
            mBinds.Clear();
            mBinds = null;
        }
    }
}