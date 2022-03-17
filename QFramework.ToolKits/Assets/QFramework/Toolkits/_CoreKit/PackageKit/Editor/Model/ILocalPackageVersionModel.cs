/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal interface ILocalPackageVersionModel : IModel
    {
        InstalledPackageVersionTable PackageVersionsTable { get; }

        void Reload();

        PackageVersion GetByName(string name);
    }

    internal class InstalledPackageVersionTable : PackageKitTable<PackageVersion>
    {
        public PackageKitTableIndex<string, PackageVersion> NameIndex =
            new PackageKitTableIndex<string, PackageVersion>(p => p.Name);

        protected override void OnAdd(PackageVersion item)
        {
            NameIndex.Add(item);
        }

        protected override void OnRemove(PackageVersion item)
        {
            NameIndex.Remove(item);
        }

        protected override void OnClear()
        {
            NameIndex.Clear();
        }

        public override IEnumerator<PackageVersion> GetEnumerator()
        {
            return NameIndex.Dictionary.SelectMany(n => n.Value).GetEnumerator();
        }

        protected override void OnDispose()
        {
            NameIndex.Dispose();
            NameIndex = null;
        }
    }

    internal class LocalPackageVersionModel : AbstractModel, ILocalPackageVersionModel
    {
        public InstalledPackageVersionTable PackageVersionsTable { get; private set; }

        public LocalPackageVersionModel()
        {
            PackageVersionsTable = new InstalledPackageVersionTable();

            Reload();
        }

        public void Reload()
        {
            PackageVersionsTable.Clear();

            var versionFiles = Array.FindAll(AssetDatabase.GetAllAssetPaths(),
                name => name.EndsWith("PackageVersion.json"));

            foreach (var fileName in versionFiles)
            {
                var text = File.ReadAllText(fileName);
                var packageVersion = JsonUtility.FromJson<PackageVersion>(text);

                PackageVersionsTable.Add(packageVersion);
            }
        }

        public PackageVersion GetByName(string name)
        {
            return PackageVersionsTable.NameIndex.Get(name).FirstOrDefault();
        }

        protected override void OnInit()
        {
        }
    }

    public abstract class PackageKitTable<TDataItem> : IEnumerable<TDataItem>, IDisposable
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

    public class PackageKitTableIndex<TKeyType, TDataItem> : IDisposable
    {
        private Dictionary<TKeyType, List<TDataItem>> mIndex = new Dictionary<TKeyType, List<TDataItem>>();

        private Func<TDataItem, TKeyType> mGetKeyByDataItem = null;

        public PackageKitTableIndex(Func<TDataItem, TKeyType> keyGetter)
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
#endif