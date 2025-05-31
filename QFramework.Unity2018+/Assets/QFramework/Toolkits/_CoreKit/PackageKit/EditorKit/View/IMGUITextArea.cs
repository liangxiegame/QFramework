/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Xml;
using UnityEngine;

namespace QFramework
{
    public interface IMGUITextArea : IMGUIView, IHasText<IMGUITextArea>, IXMLToObjectConverter
    {
        BindableProperty<string> Content { get; }
    }

    internal class IMGUITextAreaView : IMGUIAbstractView, IMGUITextArea
    {
        public IMGUITextAreaView()
        {
            Content = new BindableProperty<string>(string.Empty);
            mStyle = new FluentGUIStyle(() => GUI.skin.textArea);
        }

        public BindableProperty<string> Content { get; private set; }

        protected override void OnGUI()
        {
            Content.Value = CrossPlatformGUILayout.TextArea(Content.Value, mStyle.Value, LayoutStyles);
        }

        public IMGUITextArea Text(string labelText)
        {
            Content.Value = labelText;
            return this;
        }


        public T Convert<T>(XmlNode node) where T : class
        {
            var textArea = EasyIMGUI.TextArea();

            foreach (XmlAttribute nodeAttribute in node.Attributes)
            {
                if (nodeAttribute.Name == "Id")
                {
                    textArea.Id = nodeAttribute.Value;
                }
                else if (nodeAttribute.Name == "Text")
                {
                    textArea.Text(nodeAttribute.Value);
                }
            }

            return textArea as T;
        }
    }
}
#endif