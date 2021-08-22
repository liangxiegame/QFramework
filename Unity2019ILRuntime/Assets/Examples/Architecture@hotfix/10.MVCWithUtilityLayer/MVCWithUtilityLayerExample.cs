using UnityEngine;
using QFramework.ILRuntime;
using UniRx;

namespace QFramework.Example
{
    public partial class MVCWithUtilityLayerExample : ILController<MVCWithUtilityLayerExample.MyArchitecture>
    {
        public class IncreaseCountCommand : ILCommand<MyArchitecture>
        {
            public override void Execute()
            {
                var countModel = GetModel<ICountModel>();

                countModel.Count.Value++;
            }
        }

        void OnStart()
        {
            var countModel = GetModel<ICountModel>();

            countModel.Count.BindWithInitialValue(count => Text.text = count.ToString()).AddTo(gameObject);

            Button.OnClickAsObservable().Subscribe(_ => { SendCommand<IncreaseCountCommand>(); });
        }

        void OnDestroy()
        {
            // Destory Code Here
        }


        /// <summary>
        /// Model 的接口定义（要继承 ILModel 接口）
        /// </summary>
        public interface ICountModel : ILModel
        {
            Property<int> Count { get; }

            void Load();
            void Save();
        }

        /// <summary>
        /// Model 的实现
        /// </summary>
        public class CountModel : ILModel<MyArchitecture>, ICountModel
        {
            public Property<int> Count { get; private set; }

            public void Load()
            {
                var storage = GetUtility<IStorage>();

                var count = storage.LoadInt("COUNT", 0);

                Count = new Property<int>(count);
            }

            public void Save()
            {
                var storage = GetUtility<IStorage>();

                storage.SaveInt("COUNT", Count.Value);
            }
        }

        /// <summary>
        /// 存储 Utility
        /// </summary>
        public interface IStorage : ILUtility
        {
            int LoadInt(string key, int initialValue = 0);
            void SaveInt(string key, int value);
        }

        public class PlayerPrefStorage : IStorage
        {
            public int LoadInt(string key, int initialValue = 0)
            {
                return PlayerPrefs.GetInt(key, initialValue);
            }

            public void SaveInt(string key, int value)
            {
                PlayerPrefs.SetInt(key, value);
            }
        }


        /// <summary>
        /// QFramework 提供了一个 ILArchitecture 基类，
        /// 方便搭建自己实现自己的 Arcitecture (架构)
        /// </summary>
        public class MyArchitecture : ILArchitecture<MyArchitecture>
        {
            protected override void OnSystemConfig(ILRuntimeIOCContainer systemLayer)
            {
            }

            protected override void OnModelConfig(ILRuntimeIOCContainer modelLayer)
            {
                // 注册 CountModel
                modelLayer.RegisterInstance<ICountModel>(new CountModel());
            }

            protected override void OnUtilityConfig(ILRuntimeIOCContainer utilityLayer)
            {
                // 注册存储工具
                utilityLayer.RegisterInstance<IStorage>(new PlayerPrefStorage());
            }

            protected override void OnLaunch()
            {
                // 加载 Model
                GetModel<ICountModel>()
                    .Load();

                // 退出时自动保存
                Observable.OnceApplicationQuit()
                    .Subscribe(_ => { GetModel<ICountModel>().Save(); });
            }
        }
    }
}