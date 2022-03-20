/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal class LabelViewWithRect : IMGUIAbstractView
    {
        public LabelViewWithRect(string content = "", float x = 100, float y = 200, float width = 200,
            float height = 200)
        {
            Content = new BindableProperty<string>(content);

            Rect = new Rect(x, y, width, height);
        }

        public Rect Rect { get; set; }

        public BindableProperty<string> Content { get; private set; }

        protected override void OnGUI()
        {
            EditorGUI.LabelField(Rect, Content.Value, EditorStyles.boldLabel);
        }
    }
}
#endif