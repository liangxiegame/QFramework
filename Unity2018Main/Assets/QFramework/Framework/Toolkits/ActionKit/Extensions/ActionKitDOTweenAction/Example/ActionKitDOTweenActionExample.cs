/****************************************************************************
 * Copyright (c) 2018.12 ~ 2020.5 liangxie MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using DG.Tweening;
using UnityEngine;

namespace QFramework.Example
{
    public class ActionKitDOTweenActionExample : MonoBehaviour
    {
        void Start()
        {
            this.Sequence()
                .DOTween(() => Camera.main.DOColor(Color.cyan, 0.5f))
                .DOTween(() => Camera.main.DOColor(Color.blue, 0.5f))
                .Append(DoTweenAction.Allocate(() => Camera.main.DOColor(Color.black, 0.5f)))
                .Begin();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.Sequence()
                    .DOTween(() => Camera.main.DOColor(Color.cyan, 0.5f))
                    .DOTween(() => Camera.main.DOColor(Color.blue, 0.5f))
                    .Append(DoTweenAction.Allocate(() => Camera.main.DOColor(Color.black, 0.5f)))
                    .Begin();
            }
            
        }
    }
}