using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace QFramework.Example
{
    /// <summary>
    /// 支持 Controller
    /// </summary>
    public partial class MVCWithSystemLayerExample : ILController<MVCWithSystemLayerExample.MVCWithSystemLayer>
    {
        private ILEventSystemNode<MVCWithSystemLayer> mEventSystemNode =
            ILEventSystemNode<MVCWithSystemLayer>.Allocate();
        
        void OnStart()
        {
            var countModel = GetModel<ICountModel>();

            countModel.Count.BindWithInitialValue(c =>
            {
                Text.text = c.ToString();
            }).AddTo(gameObject);
            
            // 按钮点击注册
            Button.onClick.AddListener(()=>
            {
                SendCommand<IncreaseCommand>();
            });

            mEventSystemNode.Register<OnMissionComplete>((complete =>
            {
                MissionText.text = complete.MissionName;
            }));
        }

        void OnDestroy()
        {
            
            mEventSystemNode.Recycle2Cache();
            mEventSystemNode = null;
        }
        
        public class IncreaseCommand : ILCommand<MVCWithSystemLayer>
        {
            public override void Execute()
            {
                Debug.Log("increase");
                GetModel<ICountModel>()
                    .Count.Value++;
            }
        }



        /// <summary>
        /// 任务完成事件
        /// </summary>
        public class OnMissionComplete
        {
            public string MissionName { get; set; }
        }

        /// <summary>
        /// 任务系统
        /// </summary>
        public interface IMissionSystem : ILSystem
        {
            void Launch();
        }

        public class MissionSystem : ILSystem<MVCWithSystemLayer>,
            IMissionSystem
        {
            public class Mission
            {
                public string Name { get; set; }

                public Func<bool> CheckComplete { get; set; }
            }

            private List<Mission> mMissions = new List<Mission>();

            public void Launch()
            {
                var countModel = this.GetModel<ICountModel>();

                mMissions.Add(new Mission()
                {
                    Name = "计数初学者", CheckComplete = () => countModel.Count.Value >= 3
                });
                
                mMissions.Add(new Mission()
                {
                    Name = "计数专家", CheckComplete = () => countModel.Count.Value >= 5
                });
                
                mMissions.Add(new Mission()
                {
                    Name = "计数达人", CheckComplete = () => countModel.Count.Value >= 10
                });

                // 监听变化
                countModel.Count.Bind(c =>
                {
                    // 获取所有完成任务的事件
                    var competedMissions = mMissions.Where(m => m.CheckComplete()).ToList();

                    foreach (var competedMission in competedMissions)
                    {
                        // 发送完成任务的事件
                        SendEvent(new OnMissionComplete()
                        {
                            MissionName = competedMission.Name
                        });

                        // 删除任务
                        mMissions.Remove(competedMission);
                    }
                });
            }
        }

        public interface ICountModel : ILModel
        {
            Property<int> Count { get; }
        }

        public class CountModel : ILModel<MVCWithSystemLayer>, ICountModel
        {
            public Property<int> Count { get; } = new Property<int>(0);
        }

        public class MVCWithSystemLayer : ILArchitecture<MVCWithSystemLayer>
        {
            protected override void OnSystemConfig(ILRuntimeIOCContainer systemLayer)
            {
                // 注册任务系统
                systemLayer.RegisterInstance<IMissionSystem>(new MissionSystem());
            }

            protected override void OnModelConfig(ILRuntimeIOCContainer modelLayer)
            {
                // 注册计数模型
                modelLayer.RegisterInstance<ICountModel>(new CountModel());
            }

            protected override void OnUtilityConfig(ILRuntimeIOCContainer utilityLayer)
            {
            }

            protected override void OnLaunch()
            {
                // 启动系统
                GetSystem<IMissionSystem>()
                    .Launch();
                
                Debug.Log("Launch");
                
            }
        }
    }
}