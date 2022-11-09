/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class LocaleKitEditor
    {
        /// <summary>
        /// 快捷访问 减少代码量
        /// </summary>
        private static Lazy<EditorPrefsBoolProperty> mIsCN =
            new Lazy<EditorPrefsBoolProperty>(() => new EditorPrefsBoolProperty("EDITOR_CN", true));

        public static IBindableProperty<bool> IsCN => mIsCN.Value;

        private static IMGUILabel mENLabel;

        private static Lazy<IMGUIView> mSwitchToggleView = new Lazy<IMGUIView>(() =>
        {
            var languageToggle = EasyIMGUI.Toggle()
                .IsOn(() => !IsCN.Value);
            languageToggle.ValueProperty.Register(b => { IsCN.Value = !b; });

            mENLabel = EasyIMGUI.Label()
                .Text("EN");

            return EasyIMGUI.Custom().OnGUI(() =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                mENLabel.DrawGUI();
                languageToggle.DrawGUI();
                GUILayout.EndHorizontal();
            });
        });

        public static void DrawSwitchToggle(Color fontColor)
        {
            mENLabel?.ChangeFontColor(fontColor);
            mSwitchToggleView.Value.DrawGUI();
        }
    }
}
#endif