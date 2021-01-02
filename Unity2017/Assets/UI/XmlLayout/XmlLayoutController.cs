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
    [RequireComponent(typeof(XmlLayout))]
    public partial class XmlLayoutController : MonoBehaviour
    {        
        private XmlLayout _xmlLayout;
        protected XmlLayout xmlLayout
        {
            get
            {
                if (_xmlLayout == null)
                {
                    _xmlLayout = this.GetComponent<XmlLayout>();
                }

                return _xmlLayout;
            }
        }

        public bool SuppressEventHandling = false;

        public virtual void ReceiveMessage(string methodName, string value, RectTransform source = null)
        {
            if (SuppressEventHandling) return;

            var type = this.GetType();            
            var method = type.GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);            

            if (method == null)
            {
                Debug.LogError("[XmlLayout][XmlLayoutController] No method named '" + methodName + "' has been defined in this XmlLayoutController!");
                return;
            }

            if (value == null || method.GetParameters().Count() == 0)
            {
                method.Invoke(this, null);
            }
            else
            {
                var parameters = method.GetParameters();                

                if (!parameters.Any())
                {
                    method.Invoke(this, null);
                    return;
                }

                var parameterInfo = parameters.FirstOrDefault();
                var parameterType = parameterInfo.ParameterType;

                if (value == "this" && source != null)
                {
                    object parameter = source;
                    
                    // if the parameter is a MonoBehaviour type, then attempt to find an object of that type from the source object and use it as the parameter
                    if(parameterType.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        parameter = source.GetComponent(parameterType);
                        if (parameter == null)
                        {
                            parameter = source.GetComponentInChildren(parameterType);                            
                        }
                    }

                    method.Invoke(this, new object[] { parameter });
                }
                else
                {                    
                    method.Invoke(this, new object[] { value.ChangeToType(parameterType) });
                }
            }            
        }

        public virtual void ReceiveElementDroppedMessage(string methodName, XmlElement item, XmlElement droppedOn)
        {
            if (SuppressEventHandling) return;

            var type = this.GetType();
            var method = type.GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (method == null)
            {
                Debug.LogError("[XmlLayout][XmlLayoutController] No method named '" + methodName + "' has been defined in this XmlLayoutController!");
                return;
            }

            method.Invoke(this, new object[] { item, droppedOn });
        }

        /// <summary>
        /// This function will be called whenever the layout is rebuilt - if you have any setup code which needs to be executed after the layout is rebuilt, override this function and implement it here.
        /// ParseXmlResult.Changed   => The Xml was parsed and the layout changed as a result
        /// ParseXmlResult.Unchanged => The Xml was unchanged, so no the layout remained unchanged
        /// ParseXmlResult.Failed    => The Xml failed validation
        /// </summary>
        public virtual void LayoutRebuilt(ParseXmlResult parseResult)
        {
        }

        internal virtual void ViewModelUpdated()
        {
        }

        internal virtual void ViewModelPropertyChanged(string propertyName)
        {
        }

        internal virtual string ProcessViewModel(string xml)
        {
            return xml;
        }

        public virtual void Show()
        {
            xmlLayout.Show();
        }
        
        public virtual void Hide(Action onCompleteCallback = null)
        {
            xmlLayout.Hide(onCompleteCallback);
        }
    }
}
