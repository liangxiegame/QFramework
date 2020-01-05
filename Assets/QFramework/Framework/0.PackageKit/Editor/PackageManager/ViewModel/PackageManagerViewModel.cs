using System.Collections.Generic;
using System.Linq;
using BindKit.Commands;
using BindKit.ViewModels;
using UnityEngine;

namespace QFramework.PackageKit
{
    public class PackageManagerViewModel : ViewModelBase
    {
        [Inject]
        public PackageManagerModel Model { get; set; }
        
        [Inject]
        public IPackageManagerServer Server { get; set; }


        void UpdateCategoriesFromModel()
        {
            var categories = Model.PackageDatas.Select(p => p.Type.ToString()).Distinct().ToList();
            categories.Insert(0, "all");
            mCategories = categories;
        }

        public override void OnInit()
        {
            mPackageDatas = Model.PackageDatas.OrderBy(p => p.Name).ToList();

            UpdateCategoriesFromModel();

            Server.GetAllRemotePackageInfo(list =>
            {
                Model.PackageDatas = PackageInfosRequestCache.Get().PackageDatas;
                PackageDatas = Model.PackageDatas.OrderBy(p => p.Name).ToList();
                UpdateCategoriesFromModel();

            });

            Server.GetAllRemotePackageInfoV5((list, categories) =>
            {
                Model.PackageDatas = PackageInfosRequestCache.Get().PackageDatas;
                PackageDatas = Model.PackageDatas.OrderBy(p => p.Name).ToList();
                UpdateCategoriesFromModel();
            });
        }
        

        private List<PackageData> mPackageDatas;

        public List<PackageData> PackageDatas
        {
            get { return mPackageDatas; }
            set { this.Set(ref mPackageDatas, value, "PackageDatas"); }
        }


        private List<string> mCategories;

        public List<string> Categories
        {
            get { return mCategories; }
            set { this.Set(ref mCategories, value, "Categories"); }
        }

        public int CategoryIndex
        {
            get { return mCategoryIndex; }
            set
            {
                this.Set(ref mCategoryIndex, value, "CategoryIndex");
                OnSearch(mSearchKey);
            }
        }

        public int AccessRightIndex
        {
            get { return mAccessRightIndex; }
            set
            {
                this.Set(ref mAccessRightIndex,value,"AccessRightIndex");
                OnSearch(mSearchKey);
            }
        }

        private string mSearchKey = "";
        private int mCategoryIndex;
        private int mAccessRightIndex;

        void OnSearch(string key)
        {
            mSearchKey = key.ToLower();
            var packageDatas = Model
                .PackageDatas
                .Where(p => p.Name.ToLower().Contains(mSearchKey))
                .Where(p=>CategoryIndex == 0 || p.Type.ToString() == Categories[CategoryIndex])
                .Where(p=>AccessRightIndex == 0 || 
                          AccessRightIndex == 1 && p.AccessRight == PackageAccessRight.Public ||
                          AccessRightIndex == 2 && p.AccessRight == PackageAccessRight.Private
                )
                .OrderBy(p=>p.Name)
                .ToList();
                    
            PackageDatas = packageDatas;
        }

        public SimpleCommand<Property<string>> Search
        {
            get
            {
                return new SimpleCommand<Property<string>>((key) =>
                {
                    OnSearch(key.Value);
                });
            }
        }
    }
}