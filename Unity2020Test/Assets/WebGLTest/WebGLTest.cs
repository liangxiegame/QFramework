using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class WebGLTest : MonoBehaviour
    {
        private ResLoader mResLoader = ResLoader.Allocate();

        IEnumerator Start()
        {
            yield return ResKit.InitAsync();

            mResLoader.Add2Load<GameObject>("Image");

            mResLoader.Add2Load<Texture2D>("WebGLTestTexture");

            mResLoader.LoadAsync(() =>
            {
                var imagePrefab = mResLoader.LoadSync<GameObject>("Image");

                var image = imagePrefab.InstantiateWithParent(this.transform).GetComponent<Image>();

                var texture2D = mResLoader.LoadSync<Texture2D>("WebGLTestTexture");

                var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                    Vector2.one * 0.5f);

                image.sprite = sprite;
            });

            yield return new WaitForSeconds(3.0f);

            // 申请一个 ResLoader
            var resLoader = ResLoader.Allocate();
            
            // 提前加载 HelloWebGL
            resLoader.Add2Load<GameObject>("HelloWebGLPanel");
            
            // 异步加载
            resLoader.LoadAsync(() =>
            {
                // 打开界面
                UIKit.OpenPanel<HelloWebGLPanel>()
                    .OnClosed(() =>
                    {
                        // 关闭时卸载 resLoader
                        resLoader.Recycle2Cache();
                        resLoader = null;
                    }); 
                
            });
        }

        private void OnDestroy()
        {
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }
    }
}