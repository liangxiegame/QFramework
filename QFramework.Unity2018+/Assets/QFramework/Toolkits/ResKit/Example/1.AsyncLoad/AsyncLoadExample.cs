using System.Collections;
using UnityEngine;

namespace QFramework.Example
{
    public class AsyncLoadExample : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return ResKit.InitAsync();
            
            var resLoader = ResLoader.Allocate();

            resLoader.Add2Load<GameObject>("AssetObj 1",(b, res) =>
            {
                if (b)
                {
                    res.Asset.As<GameObject>().Instantiate();
                }
            });

            // AssetBundleName + AssetName
            resLoader.Add2Load<GameObject>("assetobj 2_prefab","AssetObj 2",(b, res) =>
            {
                if (b)
                {
                    res.Asset.As<GameObject>().Instantiate();
                }
            });
            
            resLoader.Add2Load<GameObject>("AssetObj 3",(b, res) =>
            {
                if (b)
                {
                    res.Asset.As<GameObject>().Instantiate();
                }
            });

            resLoader.LoadAsync(() =>
            {
                // 加载成功 5 秒后回收
                ActionKit.Delay(5.0f, () =>
                {
                    resLoader.Recycle2Cache();

                }).Start(this);
            });
        }
    }
}