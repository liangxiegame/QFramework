using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace QFramework
{
    public class UIEffectHelper
    {
        public static void AddUIEffect(Transform parent, GameObject effectRoot, int offsetOrder = 1)
        {
            if (parent == null || effectRoot == null)
            {
                return;
            }

            int sortingOrder = offsetOrder;
            Canvas parentCanvas = parent.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                sortingOrder += parentCanvas.sortingOrder;
            }

            effectRoot.transform.SetParent(parent, true);

            Renderer[] childs = effectRoot.GetComponentsInChildren<Renderer>(true);
            if (childs != null)
            {
                for (int i = 0; i < childs.Length; ++i)
                {
                    childs[i].sortingOrder = sortingOrder;
                }
            }
        }

        public static int GetLayerOffset(Transform parent, int offsetOrder = -1)
        {
            if (parent == null)
            {
                return offsetOrder;
            }

            int sortingOrder = offsetOrder;
            Canvas parentCanvas = parent.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                sortingOrder += parentCanvas.sortingOrder;
            }
            return sortingOrder;
        }
    }
}
