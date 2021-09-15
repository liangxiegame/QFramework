/****************************************************************************
 * Copyright (c) 2021.1 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace QFramework
{
    public interface IXMLView : IVerticalLayout
    {
        T GetById<T>(string id) where T : class, IMGUIView;

        IXMLView LoadXML(string xmlFilePath);
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

        void GenView(IMGUIView parentLayout, XmlNode parentNode)
        {
            foreach (XmlNode childNode in parentNode)
            {
                var converter = Converter.GetConverter(childNode.Name);
                    
                var view = converter.Convert<IMGUIView>(childNode);
                    
                var layout =  parentLayout as IMGUILayout;

                if (layout != null)
                {
                    layout.AddChild(view);
                }
                else
                {
                    break;
                }

                if (view.Id.IsNotNullAndEmpty())
                {
                    mIdIndex.Add(view.Id, view);
                }
                
                GenView(view, childNode);
            }
        }

        IConvertModule Converter
        {
            get
            {
                return mConverter ?? (mConverter = XMLKit.Interface.GetSystem<IXMLToObjectConvertSystem>()
                    .GetConvertModule("EasyIMGUI"));
            }
        }

        private IConvertModule mConverter = null;
    }
}