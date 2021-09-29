/****************************************************************************
 * Copyright (c) 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public interface ILocalPackageVersionModel : IModel
    {
        InstalledPackageVersionTable PackageVersionsTable { get; }

        void Reload();

        PackageVersion GetByName(string name);
    }

    public class InstalledPackageVersionTable : Table<PackageVersion>
    {
        public TableIndex<string, PackageVersion> NameIndex = new TableIndex<string, PackageVersion>(p => p.Name);

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

    public class LocalPackageVersionModel : AbstractModel, ILocalPackageVersionModel
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
}