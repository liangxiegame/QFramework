/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace QFramework
{
#if UNITY_EDITOR
    // v1 No.174
    [ClassAPI("05.TableKit", "Table<T>", 3, "Table<T>")]
    [APIDescriptionCN("一类似表格的数据结构，兼顾查询功能和性能，支持联合查询")]
    [APIDescriptionEN("A tabular like data structure, both query function and performance, support joint query")]
    [APIExampleCode(@"
public class Student
{
    public string Name { get; set; }
    public int Age { get; set; }
    public int Level { get; set; }
}
 
public class School : Table<Student>
{
    public TableIndex<int, Student> AgeIndex = new TableIndex<int, Student>((student) => student.Age);
    public TableIndex<int, Student> LevelIndex = new TableIndex<int, Student>((student) => student.Level);
         
    protected override void OnAdd(Student item)
    {
        AgeIndex.Add(item);
        LevelIndex.Add(item);
    }
 
    protected override void OnRemove(Student item)
    {
        AgeIndex.Remove(item);
        LevelIndex.Remove(item);
    }
 
    protected override void OnClear()
    {
        AgeIndex.Clear();
        LevelIndex.Clear();
    }
 
    public override IEnumerator<Student> GetEnumerator()
    {
        return AgeIndex.Dictionary.Values.SelectMany(s=>s).GetEnumerator();
    }
 
    protected override void OnDispose()
    {
        AgeIndex.Dispose();
        LevelIndex.Dispose();
    }
}
 
 
var school = new School();
school.Add(new Student(){Age = 1,Level = 2,Name = ""liangxie""});
school.Add(new Student(){Age = 2,Level = 2,Name = ""ava""});
school.Add(new Student(){Age = 3,Level = 2,Name = ""abc""});
school.Add(new Student(){Age = 3,Level = 3,Name = ""efg""});
            
foreach (var student in school.LevelIndex.Get(2).Where(s=>s.Age < 3))
{
    Debug.Log(student.Age + "":"" + student.Level + "":"" + student.Name);
}
// 1:2:liangxie
// 2:2:ava
")]
#endif
    public abstract class Table<TDataItem> : IEnumerable<TDataItem>, IDisposable
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

        // 改，由于 TDataItem 通常是引用类型，所以直接改值即可，也有可能是值类型 以后再说
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

    public class TableIndex<TKeyType, TDataItem> : IDisposable
    {
        private Dictionary<TKeyType, List<TDataItem>> mIndex =
            new Dictionary<TKeyType, List<TDataItem>>();

        private Func<TDataItem, TKeyType> mGetKeyByDataItem = null;

        public TableIndex(Func<TDataItem, TKeyType> keyGetter)
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