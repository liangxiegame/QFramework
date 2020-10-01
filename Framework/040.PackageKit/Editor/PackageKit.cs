using QFramework.PackageKit.Model;
using QFramework.PackageKit.Utility;

namespace QFramework.PackageKit
{
    public class PackageKit : Architecture<PackageKit>
    {
        private PackageKit(){}
        
        protected override void OnSystemConfig(IQFrameworkContainer systemLayer)
        {
        }

        protected override void OnModelConfig(IQFrameworkContainer modelLayer)
        {
            // 包类型
            modelLayer.RegisterInstance<IPackageTypeConfigModel>(new PackageTypeConfigModel());
            
            // 已安装类型
            modelLayer.RegisterInstance<IInstalledPackageVersionsConfigModel>(new InstalledPackageVersionsConfigModel());
            modelLayer.RegisterInstance<IPackageManagerModel>(new PackageManagerModel());
            modelLayer.RegisterInstance<IPackageManagerServer>(new PackageManagerServer());
        }

        protected override void OnUtilityConfig(IQFrameworkContainer utilityLayer)
        {
            // 弹框注册
            utilityLayer.RegisterInstance<IEditorDialogUtility>(new EditorDialogUtility());
        }

        protected override void OnLaunch()
        {
        }
    }
}