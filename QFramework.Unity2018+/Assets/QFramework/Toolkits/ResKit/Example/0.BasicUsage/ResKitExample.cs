using UnityEngine;

namespace QFramework.Example
{
    public class ResKitExample : MonoBehaviour
    {
        // 每个脚本都需要
        private ResLoader mResLoader = ResLoader.Allocate();

        private void Start()
        {
            // 项目启动只调用一次即可
            ResKit.Init();
            
            // 通过资源名 + 类型搜索并加载资源（更方便）
            var prefab = mResLoader.LoadSync<GameObject>("AssetObj");
            var gameObj = Instantiate(prefab);
            gameObj.name = "这是使用通过 AssetName 加载的对象";

            // 通过 AssetBundleName 和 资源名搜索并加载资源（更精确）
            prefab = mResLoader.LoadSync<GameObject>("assetobj_prefab", "AssetObj");
            gameObj = Instantiate(prefab);
            gameObj.name = "这是使用通过 AssetName  和 AssetBundle 加载的对象";
        }

        private void OnDestroy()
        {
            // 释放所有本脚本加载过的资源
            // 释放只是释放资源的引用
            // 当资源的引用数量为 0 时，会进行真正的资源卸载操作
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }
    }
}