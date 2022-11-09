using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    
    // 1. 定义一个 Model 对象
    public interface ICounterAppModel : IModel
    {
        BindableProperty<int> Count { get; }
    }
    public class CounterAppModel : AbstractModel,ICounterAppModel
    {
        public BindableProperty<int> Count { get; } = new BindableProperty<int>();

        protected override void OnInit()
        {
            var storage = this.GetUtility<IStorage>();
            
            // 设置初始值（不触发事件）
            Count.SetValueWithoutEvent(storage.LoadInt(nameof(Count)));

            // 当数据变更时 存储数据
            Count.Register(newCount =>
            {
                storage.SaveInt(nameof(Count),newCount);
            });
        }
    }

    public interface IAchievementSystem : ISystem
    {
        
    }

    public class AchievementSystem : AbstractSystem ,IAchievementSystem
    {
        protected override void OnInit()
        {
            this.GetModel<ICounterAppModel>() // -+
                .Count
                .Register(newCount =>
                {
                    if (newCount == 10)
                    {
                        Debug.Log("触发 点击达人 成就");
                    }
                    else if (newCount == 20)
                    {
                        Debug.Log("触发 点击专家 成就");
                    }
                    else if (newCount == -10)
                    {
                        Debug.Log("触发 点击菜鸟 成就");
                    }
                });
        }
    }

    public interface IStorage : IUtility
    {
        void SaveInt(string key, int value);
        int LoadInt(string key, int defaultValue = 0);
    }
    
    public class Storage : IStorage
    {
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key,value);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }


    // 2.定义一个架构（提供 MVC、分层、模块管理等）
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            // 注册 System 
            this.RegisterSystem<IAchievementSystem>(new AchievementSystem()); 
             
            // 注册 Model
            this.RegisterModel<ICounterAppModel>(new CounterAppModel());
            
            // 注册存储工具的对象
            this.RegisterUtility<IStorage>(new Storage());
        }


        protected override void ExecuteCommand(ICommand command)
        {
            Debug.Log("Before " + command.GetType().Name + "Execute");
            base.ExecuteCommand(command);
            Debug.Log("After " + command.GetType().Name + "Execute");
        }
    }

    // 引入 Command
    public class IncreaseCountCommand : AbstractCommand 
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<ICounterAppModel>();
                
            model.Count.Value++;
        }
    }
    
    public class DecreaseCountCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<ICounterAppModel>().Count.Value--; // -+
        }
    }

    // Controller
    public class CounterAppController : MonoBehaviour , IController /* 3.实现 IController 接口 */
    {
        // View
        private Button mBtnAdd;
        private Button mBtnSub;
        private Text mCountText;
        
        // 4. Model
        private ICounterAppModel mModel;

        void Start()
        {
            // 5. 获取模型
            mModel = this.GetModel<ICounterAppModel>();
            
            // View 组件获取
            mBtnAdd = transform.Find("BtnAdd").GetComponent<Button>();
            mBtnSub = transform.Find("BtnSub").GetComponent<Button>();
            mCountText = transform.Find("CountText").GetComponent<Text>();
            
            
            // 监听输入
            mBtnAdd.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand<IncreaseCountCommand>();
            });
            
            mBtnSub.onClick.AddListener(() =>
            {
                // 交互逻辑
                this.SendCommand(new DecreaseCountCommand(/* 这里可以传参（如果有） */));
            });

            // 表现逻辑
            mModel.Count.RegisterWithInitValue(newCount => // -+
            {
                UpdateView();

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void UpdateView()
        {
            mCountText.text = mModel.Count.ToString();
        }

        // 3.
        public IArchitecture GetArchitecture()
        {
            return CounterApp.Interface;
        }

        private void OnDestroy()
        {
            // 8. 将 Model 设置为空
            mModel = null;
        }
    }
}
