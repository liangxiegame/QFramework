using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class UIPanelTable : UIKitTable<IPanel>
    {
        public UIKitTableIndex<string, IPanel> GameObjectNameIndex =
            new UIKitTableIndex<string, IPanel>(panel => panel.Transform.name);

        public UIKitTableIndex<Type, IPanel> TypeIndex = new UIKitTableIndex<Type, IPanel>(panel => panel.GetType());


        public IEnumerable<IPanel> GetPanelsByPanelSearchKeys(PanelSearchKeys panelSearchKeys)
        {
            if (panelSearchKeys.PanelType != null && (!string.IsNullOrEmpty(panelSearchKeys.GameObjName) || panelSearchKeys.Panel != null ))
            {
                return TypeIndex.Get(panelSearchKeys.PanelType)
                    .Where(p => p.Transform.name == panelSearchKeys.GameObjName || p == panelSearchKeys.Panel);
            }

            if (panelSearchKeys.PanelType != null)
            {
                return TypeIndex.Get(panelSearchKeys.PanelType);
            }
            
            if (panelSearchKeys.Panel != null)
            {
                return GameObjectNameIndex.Get(panelSearchKeys.Panel.Transform.gameObject.name).Where(p => p == panelSearchKeys.Panel);
            }

            // 感谢 QF 群友 王某人 提供bug反馈
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
    
     public abstract class UIKitTable<TDataItem> : IEnumerable<TDataItem>, IDisposable
    {
        public void Add(TDataItem item)
        {
            OnAdd(item);
        }

        public void Remove(TDataItem item)
        {
            OnRemove(item);
        }

        public void Clear()
        {
            OnClear();
        }

        // 改，由于 TDataItem 是引用类型，所以直接改值即可。
        public void Update()
        {
        }

        protected abstract void OnAdd(TDataItem item);
        protected abstract void OnRemove(TDataItem item);

        protected abstract void OnClear();


        public abstract IEnumerator<TDataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected abstract void OnDispose();
    }

    public class UIKitTableIndex<TKeyType, TDataItem> : IDisposable
    {
        private Dictionary<TKeyType, List<TDataItem>> mIndex = new Dictionary<TKeyType, List<TDataItem>>();

        private Func<TDataItem, TKeyType> mGetKeyByDataItem = null;

        public UIKitTableIndex(Func<TDataItem, TKeyType> keyGetter)
        {
            mGetKeyByDataItem = keyGetter;
        }

        public IDictionary<TKeyType, List<TDataItem>> Dictionary
        {
            get { return mIndex; }
        }

        public void Add(TDataItem dataItem)
        {
            var key = mGetKeyByDataItem(dataItem);

            if (mIndex.ContainsKey(key))
            {
                mIndex[key].Add(dataItem);
            }
            else
            {
                var list = ListPool<TDataItem>.Get();

                list.Add(dataItem);

                mIndex.Add(key, list);
            }
        }

        public void Remove(TDataItem dataItem)
        {
            var key = mGetKeyByDataItem(dataItem);

            mIndex[key].Remove(dataItem);
        }

        public IEnumerable<TDataItem> Get(TKeyType key)
        {
            List<TDataItem> retList = null;

            if (mIndex.TryGetValue(key, out retList))
            {
                return retList;
            }

            // 返回一个空的集合
            return Enumerable.Empty<TDataItem>();
        }

        public void Clear()
        {
            foreach (var value in mIndex.Values)
            {
                value.Clear();
            }

            mIndex.Clear();
        }


        public void Dispose()
        {
            foreach (var value in mIndex.Values)
            {
                value.Release2Pool();
            }

            mIndex.Release2Pool();

            mIndex = null;
        }
    }
}