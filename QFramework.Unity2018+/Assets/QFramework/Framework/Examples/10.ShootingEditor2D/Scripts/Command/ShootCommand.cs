using QFramework;

namespace ShootingEditor2D
{
    public class ShootCommand : AbstractCommand
    {
        public static readonly ShootCommand Single = new ShootCommand();
        
        protected override void OnExecute()
        {
            var gunSystem = this.GetSystem<IGunSystem>();
            gunSystem.CurrentGun.BulletCountInGun.Value--;
            gunSystem.CurrentGun.GunState.Value = GunState.Shooting;

            var gunConfigItem = this.GetModel<IGunConfigModel>()
                .GetItemByName(gunSystem.CurrentGun.Name.Value);
            
            this.GetSystem<ITimeSystem>().AddDelayTask(1 / gunConfigItem.Frequency, () =>
            {
                gunSystem.CurrentGun.GunState.Value = GunState.Idle;

                if (gunSystem.CurrentGun.BulletCountInGun.Value == 0 &&
                    gunSystem.CurrentGun.BulletCountOutGun.Value > 0)
                {
                    this.SendCommand<ReloadCommand>();
                }
            });
        }
    }
}