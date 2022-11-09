using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace QFramework.PointGame
{
    public interface IAchievementSystem : ISystem
    {
    }

    public class AchievementItem
    {
        public string Name { get; set; }

        public Func<bool> CheckComplete { get; set; }

        public bool Unlocked { get; set; }
    }


    public class AchievementSystem : AbstractSystem, IAchievementSystem
    {
        private List<AchievementItem> mItems = new List<AchievementItem>();

        private bool mMissed = false;
        protected override void OnInit()
        {
            this.RegisterEvent<OnMissEvent>(e =>
            {
                mMissed = true;
            });

            this.RegisterEvent<GameStartEvent>(e =>
            {
                mMissed = false;
            });

            mItems.Add(new AchievementItem()
            {
                Name = "百分成就",
                CheckComplete = () => this.GetModel<IGameModel>().BestScore.Value > 100
            });
          
            mItems.Add(new AchievementItem()
            {
                Name = "手残",
                CheckComplete = () => this.GetModel<IGameModel>().Score.Value < 0
            });

            mItems.Add(new AchievementItem()
            {
                Name = "零失误成就",
                CheckComplete = () => !mMissed
            });
          
            mItems.Add(new AchievementItem()
            {
                Name = "零失误成就",
                CheckComplete = () => mItems.Count(item => item.Unlocked) >= 3
            });
          
            // 成就系统一般是持久化的，所以如果需要持久化也是在这个时机进行，可以让 Unlocked 变成 BindableProperty

            this.RegisterEvent<GamePassEvent>(async e =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
              
                foreach (var achievementItem in mItems)
                {
                    if (!achievementItem.Unlocked && achievementItem.CheckComplete())
                    {
                        achievementItem.Unlocked = true;

                        Debug.Log("解锁 成就:" + achievementItem.Name);
                    }
                }
            });
        }
    }
}