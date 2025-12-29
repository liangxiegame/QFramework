#if UNITY_EDITOR
namespace QFramework
{
    internal class MyPackage : Architecture<MyPackage>
    {
        protected override void Init()
        {
            this.RegisterModel(LocalPackageVersionModel.Default);
        }
    }
}
#endif