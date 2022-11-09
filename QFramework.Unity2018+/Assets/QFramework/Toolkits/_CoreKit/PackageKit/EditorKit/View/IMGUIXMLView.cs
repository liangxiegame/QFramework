/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace QFramework
{
 // ReSharper disable once InconsistentNaming
    public class EasyIMGUIXMLModule : IConvertModule
    {
        private Dictionary<string, IXMLToObjectConverter> mConverters = new Dictionary<string, IXMLToObjectConverter>();

        public EasyIMGUIXMLModule()
        {
            mConverters.Add("IButton", new IMGUIButtonView());
            mConverters.Add("ILabel", new IMGUILabelView());
            mConverters.Add("IFlexibleSpace", new IMGUIFlexibleSpaceView());
            mConverters.Add("IHorizontal", new HorizontalLayout());
            mConverters.Add("IVertical", new VerticalLayout());
            mConverters.Add("ICustom", new IMGUICustomView());
            mConverters.Add("ISpace", new IMGUISpaceView());
            mConverters.Add("ITextField", new IMGUITextFieldView());
            mConverters.Add("ITextArea", new IMGUITextAreaView());
        }

        public IXMLToObjectConverter GetConverter(string name)
        {
            if (mConverters.ContainsKey(name))
            {
                return mConverters[name];
            }
            else
            {
                foreach (var xmlToObjectConverter in mConverters)
                {
                    Debug.Log(xmlToObjectConverter.Key);
                }
                
                Debug.Log(name);

                throw new Exception("不存在");
            }
        }

        public void RegisterConverter(string name, IXMLToObjectConverter converter)
        {
            if (mConverters.ContainsKey(name))
            {
                mConverters[name] = converter;
            }
            else
            {
                mConverters.Add(name, converter);
            }
        }
    }

    public interface IXMLView : IMGUIVerticalLayout
    {
        T GetById<T>(string id) where T : class, IMGUIView;

        IXMLView LoadXML(string xmlFilePath);

        IXMLView LoadXMLContent(string xmlContent);
    }

    internal class XMLView : VerticalLayout, IXMLView
    {
        private readonly Dictionary<string, IMGUIView> mIdIndex = new Dictionary<string, IMGUIView>();

        public T GetById<T>(string id) where T : class, IMGUIView
        {
            return mIdIndex[id] as T;
        }

        public IXMLView LoadXML(string xmlFilePath)
        {
            var content = File.ReadAllText(xmlFilePath);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);

            var node = xmlDocument.FirstChild;
            GenView(this, node);

            return this;
        }

        public IXMLView LoadXMLContent(string xmlContent)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlContent);

            var node = xmlDocument.FirstChild;
            GenView(this, node);

            return this;
        }

        void GenView(IMGUIView parentLayout, XmlNode parentNode)
        {
            foreach (XmlNode childNode in parentNode)
            {
                var converter = mConverter.Value.GetConverter(childNode.Name);

                var view = converter.Convert<IMGUIView>(childNode);

                var layout = parentLayout as IMGUILayout;

                if (layout != null)
                {
                    layout.AddChild(view);
                }
                else
                {
                    break;
                }

                if (!string.IsNullOrEmpty(view.Id))
                {
                    mIdIndex.Add(view.Id, view);
                }

                GenView(view, childNode);
            }
        }

        private Lazy<IConvertModule> mConverter = new Lazy<IConvertModule>(() => XMLKit.Get.SystemLayer.Get<IXMLToObjectConvertSystem>()
            .GetConvertModule("EasyIMGUI"));
    }

    public interface IConvertModule
    {
        IXMLToObjectConverter GetConverter(string name);

        void RegisterConverter(string name, IXMLToObjectConverter converter);
    }

    public interface IXMLToObjectConverter
    {
        T Convert<T>(XmlNode node) where T : class;
    }

    public class CustomXMLToObjectConverter<T> : IXMLToObjectConverter
    {
        private readonly Func<XmlNode, T> mConverter;

        public CustomXMLToObjectConverter(Func<XmlNode, T> converter)
        {
            mConverter = converter;
        }


        public T1 Convert<T1>(XmlNode node) where T1 : class
        {
            return mConverter(node) as T1;
        }
    }

    public interface IXMLToObjectConvertSystem
    {
        IConvertModule GetConvertModule(string moduleName);
        void AddModule(string key, IConvertModule module);

        bool ContainsModule(string key);
    }

    internal class XMLToObjectConvertSystem : IXMLToObjectConvertSystem
    {
        private Dictionary<string, IConvertModule> mModules =
            new Dictionary<string, IConvertModule>();

        public IConvertModule GetConvertModule(string moduleName)
        {
            return mModules[moduleName];
        }

        public void AddModule(string key, IConvertModule module)
        {
            if (mModules.ContainsKey(key))
            {
                mModules[key] = module;
            }
            else
            {
                mModules.Add(key, module);
            }
        }

        public bool ContainsModule(string key)
        {
            return mModules.ContainsKey(key);
        }
    }

    public class XMLKit
    {
        private static Lazy<XMLKit> mInstance = new Lazy<XMLKit>(() =>
        {
            var xmlKit = new XMLKit();
            xmlKit.Init();
            return xmlKit;
        });

        public static XMLKit Get => mInstance.Value;

        public readonly IOCContainer SystemLayer = new IOCContainer();

        private void Init()
        {
            SystemLayer.Register<IXMLToObjectConvertSystem>(new XMLToObjectConvertSystem());
        }
    }
}
#endif