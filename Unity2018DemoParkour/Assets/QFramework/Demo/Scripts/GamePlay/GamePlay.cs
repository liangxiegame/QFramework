using System.Collections;
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.PlatformRunner
{
    public partial class GamePlay : ViewController,IController
    {
        IEnumerator Start()
        {
            // 回收
            yield return StageLayer.DespawnStageAB();
            yield return StageLayer.DespawnStageAB();

            // 重置关卡
            yield return StageLayer.ResetLayer();

            yield return StageLayer.Switch(this.GetModel<IGameModel>().CurTheme.Value);


            var playerCtrl = AppLoader.Container.Resolve<PlayerCtrl>();
            
            playerCtrl.OnGameStart();
//
            StageLayer.BeginScroll();


//            uiCtrl.uiFade.FadeOut();

            StageLayer.SwitchEnded(this.GetModel<IGameModel>().CurTheme.Value);

            TypeEventSystem.Global.Register<GameOverEvent>(e =>
                {
                    this.DestroyGameObjGracefully();
                })
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        /// <summary>
        /// 4test
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnScreenPressed();
            }

            if (Input.GetMouseButtonDown(0))
            {
                OnScreenPressed();
            }
        }

        void OnScreenPressed()
        {
            Log.I("onpressed began");
            
            if (PropModel.Instance.prop_auto_on)
                return;
            
            this.GetModel<IPlayerModel>().FSM.HandleEvent("touch_down");
        }

        public IArchitecture GetArchitecture()
        {
            return PlatformRunner.Interface;
        }
    }
}