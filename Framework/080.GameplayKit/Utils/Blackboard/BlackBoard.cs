using System.Collections.Generic;

namespace QFramework
{
    public class BlackBoard
    {
        class BlackboardItem
        {
            private object mValue;

            public void SetValue(object v)
            {
                mValue = v;
            }

            public T GetValue<T>()
            {
                return (T) mValue;
            }
        }

        private Dictionary<string, BlackboardItem> mItems;

        public BlackBoard()
        {
            mItems = new Dictionary<string, BlackboardItem>();
        }

        public void SetValue(string key, object v)
        {
            BlackboardItem item;
            if (mItems.ContainsKey(key) == false)
            {
                item = new BlackboardItem();
                mItems.Add(key, item);
            }
            else
            {
                item = mItems[key];
            }

            item.SetValue(v);
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            if (mItems.ContainsKey(key) == false)
            {
                return defaultValue;
            }

            return mItems[key].GetValue<T>();
        }
    }
}
