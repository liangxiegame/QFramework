/****************************************************************************
 * Copyright (c) 2016 ~ 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;

namespace QFramework
{
    [DisplayName("PackageKit 插件管理")]
    [DisplayNameEN("Plugin Manager(OnlyCN)")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder("PackageKit")]
    internal class PackageManagerView : IPackageKitView, IController, IUnRegisterList
    {
        private PackageListView mPackageListView;


        public EditorWindow EditorWindow { get; set; }

        public Type Type { get; } = typeof(PackageManagerView);

        public void Init()
        {
            var localPackageVersionModel = this.GetModel<LocalPackageVersionModel>();
            var packageModel = this.GetModel<PackageManagerModel>();
            mPackageListView = new PackageListView();
            mPackageListView.Init(GetArchitecture(), this, localPackageVersionModel, (key) =>
            {
                PackageKit.PackageRepositories.Value = PackageSearchHelper.Search(key.ToLower(),packageModel.Repositories);
            });
            PackageKit.Categories
                .Register(value => mPackageListView.UpdateCategories(value)).AddToUnregisterList(this);
        }

        public void OnUpdate()
        {
            mPackageListView.PackageRepositories = PackageKit.PackageRepositories.Value;
            mPackageListView.Update();
        }

        public void OnGUI()
        {
            mPackageListView.DrawGUI();
        }

        public void OnWindowGUIEnd()
        {
        }

        public void OnDispose()
        {
            this.UnRegisterAll();

            mPackageListView.Dispose();
            mPackageListView = null;
            
            GetArchitecture().Deinit();
        }

        public void OnShow()
        {
            this.SendCommand<PackageManagerInitCommand>();
        }

        public void OnHide()
        {
        }


        public IArchitecture GetArchitecture() => PackageKit.Interface;

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }
}
#endif