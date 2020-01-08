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
            var categories = Model.Repositories.Select(p => p.type).Distinct()
                .Select(t=>PackageTypeHelper.TryGetFullName(t))
                .ToList();
            categories.Insert(0, "all");
            Categories = categories;
        }

        public override void OnInit()
        {
            PackageRepositories = Model.Repositories.OrderBy(p => p.name).ToList();

            UpdateCategoriesFromModel();

            Server.GetAllRemotePackageInfoV5((list, categories) =>
            {
                Model.Repositories = list.OrderBy(p=>p.name).ToList();
                PackageRepositories = Model.Repositories;
                UpdateCategoriesFromModel();
            });
        }
        
        private List<PackageRepository> mPackageRepositories = new List<PackageRepository>();

        public List<PackageRepository> PackageRepositories
        {
            get { return mPackageRepositories; }
            set { this.Set(ref mPackageRepositories, value, "PackageRepositories"); }
        }

        private List<string> mCategories = new List<string>();

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
            var repositories = Model
                .Repositories
                .Where(p => p.name.ToLower().Contains(mSearchKey))
                .Where(p=>CategoryIndex == 0 || p.type.ToString() == Categories[CategoryIndex])
                .Where(p=>AccessRightIndex == 0 || 
                          AccessRightIndex == 1 && p.accessRight == "public" ||
                          AccessRightIndex == 2 && p.accessRight == "private"
                )
                .OrderBy(p=>p.name)
                .ToList();
                    
            PackageRepositories = repositories;
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