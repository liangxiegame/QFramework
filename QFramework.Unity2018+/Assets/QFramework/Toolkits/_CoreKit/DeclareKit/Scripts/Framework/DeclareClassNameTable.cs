/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    internal class DeclareClassNameRuleTable : Table<DeclareClassNameRuleItem>
    {
        public TableIndex<string, DeclareClassNameRuleItem> ClassNameIndex =
            new TableIndex<string, DeclareClassNameRuleItem>(item => item.ClassName);
        protected override void OnAdd(DeclareClassNameRuleItem item)
        {
            ClassNameIndex.Add(item);
        }

        protected override void OnRemove(DeclareClassNameRuleItem item)
        {
            ClassNameIndex.Remove(item);
        }

        protected override void OnClear()
        {
            ClassNameIndex.Clear();
        }

        public override IEnumerator<DeclareClassNameRuleItem> GetEnumerator()
        {
            return ClassNameIndex.Dictionary.Values.SelectMany(d => d).GetEnumerator();
        }

        protected override void OnDispose()
        {
            ClassNameIndex.Dispose();
            ClassNameIndex = null;
        }
    }

    internal class DeclareClassNameRuleItem
    {
        public string ClassName;
        public Action<DeclareComponent> OnRule;
    }
}