using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace UI.Xml
{
    internal interface IXmlElementReference
    {
        void ClearElement();
    }

    public class XmlElementReference<T> : IXmlElementReference
        where T : MonoBehaviour
    {
        private T _element;
        public T element
        {
            get
            {
                if (_element == null)
                {
                    _element = xmlLayout.GetElementById<T>(id);
                }
                return _element;
            }

            protected set
            {
                _element = value;
            }
        }

        private XmlLayout xmlLayout;
        private string id;

        public XmlElementReference(XmlLayout xmlLayout, string id)
        {
            this.xmlLayout = xmlLayout;
            this.id = id;
        }

        static public implicit operator T(XmlElementReference<T> getXmlElement)
        {
            return getXmlElement.element;
        }

        public void ClearElement()
        {
            _element = null;
        }
    }
}
