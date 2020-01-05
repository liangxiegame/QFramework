

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
            Server.GetAllRemotePackageInfo(list =>
            {
                RenderEndCommandExecuter.PushCommand(() =>
                {
                    Model.PackageDatas = PackageInfosRequestCache.Get().PackageDatas;
                    
                    TypeEventSystem.Send(new PackageManagerViewUpdate()
                    {
                        PackageDatas = Model.SelectedPackageType,
                        VersionCheck = Model.VersionCheck,
                    });
                });
            });
            
            
            Server.GetAllRemotePackageInfoV5((list,categories) =>
            {
                RenderEndCommandExecuter.PushCommand(() =>
                {
                    Model.PackageDatas = PackageInfosRequestCache.Get().PackageDatas;
                    
                    TypeEventSystem.Send(new PackageManagerViewUpdate()
                    {
                        PackageDatas = Model.SelectedPackageType,
                        VersionCheck = Model.VersionCheck,
                    });
                });
            });
        }
    }
}