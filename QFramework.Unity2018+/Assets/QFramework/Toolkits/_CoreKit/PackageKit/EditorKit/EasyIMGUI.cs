/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using UnityEditor;

namespace QFramework
{
    [InitializeOnLoad]
    public sealed class EasyIMGUI
    {
        public static IMGUILabel Label() => new IMGUILabelView();

        public static IMGUIButton Button() => new IMGUIButtonView();

        public static IMGUISpace Space() => new IMGUISpaceView();

        public static IMGUIFlexibleSpace FlexibleSpace() => new IMGUIFlexibleSpaceView();

        public static IMGUITextField TextField() => new IMGUITextFieldView();

        public static IMGUITextArea TextArea() => new IMGUITextAreaView();

        public static IMGUICustom Custom() => new IMGUICustomView();

        public static IMGUIToggle Toggle() => new IMGUIIMGUIToggleView();

        public static IMGUIBox Box() => new IMGUIBoxView();

        public static IMGUIToolbar Toolbar() => new IMGUIIMGUIToolbarView();

        public static IMGUIVerticalLayout Vertical() => new VerticalLayout();

        public static IMGUIHorizontalLayout Horizontal() => new HorizontalLayout();

        public static IMGUIScrollLayout Scroll() => new IMGUIScrollLayoutView();

        public static IMGUIAreaLayout Area() => new IMGUIAreaLayoutView();
        
        public static IXMLView XMLView() => new XMLView();

        public static IMGUIRectLabel LabelWithRect() => new IMGUIRectLabelView();

        public static IMGUIRectBox BoxWithRect() => new IMGUIRectBoxView();

        public static IMGUIEnumPopup EnumPopup(Enum initValue) => new IMGUIEnumPopupView(initValue);

        static EasyIMGUI()
        {
            XMLKit.Get.SystemLayer.Get<IXMLToObjectConvertSystem>()
                .AddModule("EasyIMGUI", new EasyIMGUIXMLModule());
        }
    }
}
#endif