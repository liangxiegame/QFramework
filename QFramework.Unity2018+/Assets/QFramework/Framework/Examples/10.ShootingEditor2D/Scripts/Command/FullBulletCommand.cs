using QFramework;

namespace ShootingEditor2D
{
    public class FullBulletCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var gunSystem = this.GetSystem<IGunSystem>();
            var gunConfigModel = this.GetModel<IGunConfigModel>();

            gunSystem.CurrentGun.BulletCountInGun.Value =
                gunConfigModel.GetItemByName(gunSystem.CurrentGun.Name.Value).BulletMaxCount;
            
            foreach (var gunSystemGunInfo in gunSystem.GunInfos)
            {
                gunSystemGunInfo.BulletCountInGun.Value =
                    gunConfigModel.GetItemByName(gunSystemGunInfo.Name.Value).BulletMaxCount;
            }
        }
    }
}