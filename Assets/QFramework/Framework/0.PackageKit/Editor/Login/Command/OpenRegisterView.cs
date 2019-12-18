
namespace QFramework.PackageKit
{
    public class OpenRegisterView : IPackageLoginCommand
    {
        [Inject]
        public PackageLoginModel Model { get; set; }

        public void Execute()
        {
            Model.InLoginView = false;

            TypeEventSystem.Send(new LoginSucceed()
            {
                Logined = Model.Logined,
                InLoginView = Model.InLoginView
            });
        }
    }
}