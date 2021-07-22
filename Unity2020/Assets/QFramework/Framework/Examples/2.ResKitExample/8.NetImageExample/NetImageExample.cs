using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class NetImageExample : MonoBehaviour
    {
        ResLoader mResLoader = ResLoader.Allocate();

        // Use this for initialization
        void Start()
        {
            var image = transform.Find("Image").GetComponent<Image>();


            mResLoader.Add2Load<Texture2D>(
                "netimage:" +
                "https://file.liangxiegame.com/8eb58e6e-c6ac-40fe-a4a0-17b59fb6f5ed.png",
                (b, res) =>
                {
                    if (b)
                    {
                        var texture = res.Asset as Texture2D;
                        var sprite = texture.CreateSprite();
                        image.sprite = sprite;
                        mResLoader.AddObjectForDestroyWhenRecycle2Cache(sprite);
                    }
                });


            mResLoader.LoadAsync();
        }


        private void OnDestroy()
        {
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}