using UnityEngine;

namespace QFramework.PlatformRunner
{
    public interface IPlayerModel : IModel
    {
        /// <summary>
        /// 当前的能量
        /// </summary>
        BindableProperty<int> CurEnergy { get; }


        /// <summary>
        /// 当前的速度
        /// </summary>
        BindableProperty<float> CurSpeed { get; }


        /// <summary>
        /// 起跳要用的力量
        /// </summary>
        BindableProperty<int> CurJumpPower { get; }
        
        QFSMLite FSM { get; }
        void HandleTriggerEnterEvent(GameObject otherGameObject);
    }

    public class PlayerModel : AbstractModel, IPlayerModel
    {
        protected override void OnInit()
        {
        }

        public BindableProperty<int> CurEnergy { get; } = new BindableProperty<int>
        {
            Value = 0
        };

        public BindableProperty<float> CurSpeed { get; } = new BindableProperty<float>()
        {
            Value = 8.0f
        };

        public BindableProperty<int> CurJumpPower { get; } = new BindableProperty<int>()
        {
            Value = 800
        };

        public QFSMLite FSM { get; } = new QFSMLite();

         /// <summary>
         /// 处理Player的碰撞触发事件,改变状态,发送消息
         /// </summary>
         public void HandleTriggerEnterEvent(GameObject otherGO)
         {
             if (otherGO.CompareTag("block_air"))
             {
                 otherGO.GetComponent<BoxCollider2D>().enabled = true;
                 return;
             }

             if (otherGO.CompareTag("coin"))
             {
                 otherGO.GetComponent<ICmd>().Execute();
                 return;
             }
             

             // 如果是水果
             if (otherGO.tag.Substring(0, 5).CompareTo("fruit") == 0)
             {
                 var energyValue = FruitModel.Instance.HandleGetFruitEvent(otherGO.tag);
                 var fruitCommand = otherGO.GetComponent<Fruit>();
                 fruitCommand.Value = (int) energyValue;
                 fruitCommand.Execute();

                 return;

                 // 如果是道具
             }

             if (otherGO.tag.Substring(0, 4).CompareTo("prop") == 0)
             {
                 otherGO.GetComponent<Prop>().Execute();
                 PropModel.Instance.HandleGetPropEvent(otherGO.tag);
                 return;
             }

             if (otherGO.tag.Substring(0, 5).CompareTo("enemy") == 0)
             {
                 if (PropModel.Instance.prop_auto_on)
                 {
//					otherGO.GetComponent<Enemy>()
//						.Kick(GameManager.Instance.playerCtrl.transform.position - otherGO.transform.position);
                     return;
                 }

                 otherGO.GetComponent<Enemy>().StepOn();

                 FSM.HandleEvent("land");
                 FSM.HandleEvent("touch_down");
                 return;
             }
         }


    }
    
}