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
        public static IMGUILabel Label()
        {
            return new IMGUILabelView();
        }
        
        public static IMGUIButton Button()
        {
            return new IMGUIButtonView();
        }

        public static IMGUISpace Space()
        {
            return new IMGUISpaceView();
        }

        public static IMGUIFlexibleSpace FlexibleSpace()
        {
            return new IMGUIFlexibleSpaceView();
        }

        public static IMGUITextField TextField()
        {
            return new IMGUITextFieldView();
        }

        public static IMGUITextArea TextArea()
        {
            return new IMGUITextAreaView();
        }

        public static IMGUICustom Custom()
        {
            return new IMGUICustomView();
        }

        public static IMGUIToggle Toggle()
        {
            return new IMGUIIMGUIToggleView();
        }

        public static IMGUIBox Box()
        {
            return new IMGUIBoxView();
        }

        public static IMGUIToolbar Toolbar()
        {
            return new IMGUIIMGUIToolbarView();
        }

        public static IMGUIVerticalLayout Vertical()
        {
            return new VerticalLayout();
        }

        public static IMGUIHorizontalLayout Horizontal()
        {
            return new HorizontalLayout();
        }

        public static IMGUIScrollLayout Scroll()
        {
            return new IMGUIScrollLayoutView();
        }

        public static IMGUIAreaLayout Area()
        {
            return new IMGUIAreaLayoutView();
        }


        public static IXMLView XMLView()
        {
            return new XMLView();
        }

        public static IMGUIRectLabel LabelWithRect()
        {
            return new IMGUIRectLabelView();
        }

        public static IMGUIRectBox BoxWithRect()
        {
            return new IMGUIRectBoxView();
        }

        public static IMGUIEnumPopup EnumPopup(Enum initValue)
        {
            return new IMGUIEnumPopupView(initValue);
        }

        static EasyIMGUI()
        {
            XMLKit.Get.SystemLayer.Get<IXMLToObjectConvertSystem>()
                .AddModule("EasyIMGUI", new EasyIMGUIXMLModule());
        }
    }
}
#endif