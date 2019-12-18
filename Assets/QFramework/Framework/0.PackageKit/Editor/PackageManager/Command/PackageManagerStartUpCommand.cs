

namespace QFramework.PackageKit
{
    public class PackageManagerStartUpCommand : IEditorStrangeMVCCommand
    {
        [Inject]
        public PackageManagerModel Model { get; set; }

        [Inject]
        public IPackageManagerServer Server { get; set; }

        public void Execute()
        {
            TypeEventSystem.Send(new PackageManagerViewUpdate()
            {
                PackageDatas = Model.SelectedPackageType,
                VersionCheck = Model.VersionCheck,
                Categories = Model.Categories
            });

            Server.GetAllRemotePackageInfo(list =>
            {
                RenderEndCommandExecuter.PushCommand(() =>
                {
                    Model.PackageDatas = PackageInfosRequestCache.Get().PackageDatas;
                    
                    TypeEventSystem.Send(new PackageManagerViewUpdate()
                    {
                        PackageDatas = Model.SelectedPackageType,
                        VersionCheck = Model.VersionCheck,
                        Categories = Model.Categories
                    });
                });
            });
            
            
            Server.GetAllRemotePackageInfoV5((list,categories) =>
            {
                RenderEndCommandExecuter.PushCommand(() =>
                {
                    Model.PackageDatas = PackageInfosRequestCache.Get().PackageDatas;
                    Model.Categories = categories;
                    
                    TypeEventSystem.Send(new PackageManagerViewUpdate()
                    {
                        PackageDatas = Model.SelectedPackageType,
                        VersionCheck = Model.VersionCheck,
                        Categories = categories
                    });
                });
            });
        }
    }
}