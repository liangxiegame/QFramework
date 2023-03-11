using QFramework;

namespace ShootingEditor2D
{
    public class ShiftGunCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<IGunSystem>()
                .ShiftGun();
        }
    }
}