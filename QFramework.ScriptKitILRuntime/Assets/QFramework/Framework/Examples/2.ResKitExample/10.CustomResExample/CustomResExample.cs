using UnityEngine;

namespace QFramework
{
    public class CustomResExample : MonoBehaviour
    {
        // 自定义的 Res
        public class MyRes : Res
        {
            public MyRes(string name)
            {
                mAssetName = name;
            }

            // 同步加载（自己实现）
            public override bool LoadSync()
            {
                // Asset = 加载的结果给 Asset 赋值 
                State = ResState.Ready;
                return true;
            }

            // 异步加载(自己实现)
            public override void LoadAsync()
            {
                // Asset = 加载的结果给 Asset 赋值 
                State = ResState.Ready;
            }
            

            // 释放资源（自己实现)
            protected override void OnReleaseRes()
            {
                // 卸载操作
                // Asset = null
                State = ResState.Waiting;
            }
        }

        // 自定义的 Res 创建器（包含识别功能）
        public class MyResCreator : IResCreator
        {
            public bool Match(ResSearchKeys resSearchKeys)
            {
                return resSearchKeys.AssetName.StartsWith("myres://");
            }

            public IRes Create(ResSearchKeys resSearchKeys)
            {
                return new MyRes(resSearchKeys.AssetName);
            }
        }

        // Use this for initialization
        void Start()
        {
            // 添加创建器
            ResFactory.AddResCreator<MyResCreator>();

            var resLoader = ResLoader.Allocate();

            var resSearchKeys = ResSearchKeys.Allocate("myres://hello_world");
            
            var myRes =  resLoader.LoadResSync(resSearchKeys);
            
            resSearchKeys.Recycle2Cache();
            
            Debug.Log(myRes.AssetName);
            Debug.Log(myRes.State);
        }
    }
}