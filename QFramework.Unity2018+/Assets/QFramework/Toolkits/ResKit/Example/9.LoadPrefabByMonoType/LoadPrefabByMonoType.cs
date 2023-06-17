using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{

    public class LoadPrefabByMonoType : MonoBehaviour
    {
        private ResLoader mResLoader;
        private void Awake()
        {
            ResKit.Init();
            mResLoader = ResLoader.Allocate();
        }

        private void Start()
        {
            mResLoader.LoadSync<SquareA>("SquareA")
                .InstantiateWithParent(transform)
                .LocalIdentity();
            
            mResLoader.Add2Load<SquareA>("SquareA",(b, res) =>
            {
                if (b)
                {
                    (res.Asset as GameObject).GetComponent<SquareA>()
                        .InstantiateWithParent(transform)
                        .LocalRotation(Quaternion.Euler(new Vector3(0, 0, 45)))
                        .GetComponent<SpriteRenderer>()
                        .color = Color.blue;
                }
            });
            
            mResLoader.LoadAsync();
        }

        private void OnDestroy()
        {
            mResLoader.Recycle2Cache();
            mResLoader = null;
        }
    }
    
    
}