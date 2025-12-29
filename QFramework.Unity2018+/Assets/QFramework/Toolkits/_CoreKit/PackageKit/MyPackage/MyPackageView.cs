#if UNITY_EDITOR
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;

namespace QFramework
{
    [DisplayName("我的插件")]
    [DisplayNameEN("My Package(OnlyCN)")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder("MyPackage")]
    internal class MyPackageView : IPackageKitView, IController, IUnRegisterList
    {
        public EditorWindow EditorWindow { get; set; }

        public void Init()
        {
            var localPackageVersionModel = this.GetModel<LocalPackageVersionModel>();
            localPackageVersionModel.Reload();
            mPackageListView.Init(GetArchitecture(), this, localPackageVersionModel, (key) =>
            {
                mSearchResult = PackageSearchHelper.Search(key.ToLower(), mMyPackages);
            });
        }

        private List<PackageRepository> mMyPackages = new List<PackageRepository>();
        private List<PackageRepository> mSearchResult = new List<PackageRepository>();

        private MyPackageListView mPackageListView = new MyPackageListView();

        public void OnShow()
        {
            this.SendCommand(new ListMyPackageCommand(list =>
            {
                mMyPackages = list;
                mSearchResult = mMyPackages;
            }));
        }

        public void OnUpdate()
        {
            mPackageListView.PackageRepositories = mSearchResult;
            mPackageListView.Update();
        }

        public void OnGUI()
        {
            mPackageListView.DrawGUI();
        }

        public void OnHide()
        {
        }

        public void OnWindowGUIEnd()
        {
        }

        public void OnDispose()
        {
            mPackageListView.Dispose();
            mPackageListView = null;
        }

        public IArchitecture GetArchitecture() => MyPackage.Interface;
        
        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }
}

#endif