using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class UIPanelTable : Table<IPanel>
    {
        public TableIndex<string, IPanel> GameObjectNameIndex =
            new TableIndex<string, IPanel>(panel => panel.Transform.name);

        public TableIndex<Type, IPanel> TypeIndex = new TableIndex<Type, IPanel>(panel => panel.GetType());


        public IEnumerable<IPanel> GetPanelsByPanelSearchKeys(PanelSearchKeys panelSearchKeys)
        {
            if (panelSearchKeys.PanelType.IsNotNull() && (panelSearchKeys.GameObjName.IsNotNullAndEmpty() || panelSearchKeys.Panel.IsNotNull()))
            {
                return TypeIndex.Get(panelSearchKeys.PanelType)
                    .Where(p => p.Transform.name == panelSearchKeys.GameObjName || p == panelSearchKeys.Panel);
            }

            if (panelSearchKeys.PanelType.IsNotNull())
            {
                return TypeIndex.Get(panelSearchKeys.PanelType);
            }
            
            if (panelSearchKeys.Panel.IsNotNull())
            {
                return GameObjectNameIndex.Get(panelSearchKeys.Panel.Transform.gameObject.name).Where(p => p == panelSearchKeys.Panel);
            }

            if (panelSearchKeys.GameObjName.IsNotNullAndEmpty())
            {
                return GameObjectNameIndex.Get(panelSearchKeys.GameObjName);
            }

            return Enumerable.Empty<IPanel>();
        }

        protected override void OnAdd(IPanel item)
        {
            GameObjectNameIndex.Add(item);
            TypeIndex.Add(item);
        }

        protected override void OnRemove(IPanel item)
        {
            GameObjectNameIndex.Remove(item);
            TypeIndex.Remove(item);
            
        }

        protected override void OnClear()
        {
            GameObjectNameIndex.Clear();
            TypeIndex.Clear();
            ;
        }


        public override IEnumerator<IPanel> GetEnumerator()
        {
            return GameObjectNameIndex.Dictionary.SelectMany(kv => kv.Value).GetEnumerator();
        }

        protected override void OnDispose()
        {
            GameObjectNameIndex.Dispose();
            TypeIndex.Dispose();

            GameObjectNameIndex = null;
            TypeIndex = null;
        }
    }
}