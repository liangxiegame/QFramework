
namespace QFramework.PackageKit
{
    public class PackageLoginStartUpCommand : IPackageLoginCommand
    {
        [Inject]
        public PackageLoginModel Model { get; set; }


        public void Execute()
        {
            TypeEventSystem.Send(new LoginSucceed()
            {
                InLoginView = Model.InLoginView,
                Logined = Model.Logined
            });
        }
    }

}