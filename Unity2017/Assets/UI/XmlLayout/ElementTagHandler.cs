using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Xml;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using UI.Xml.CustomAttributes;

namespace UI.Xml
{
    public abstract partial class ElementTagHandler
    {
        public virtual MonoBehaviour primaryComponent { get { return null; } }
        public RectTransform currentInstanceTransform { get; protected set; }
        public XmlLayout currentXmlLayoutInstance { get; protected set; }

        protected virtual Image imageComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<Image>();
            }
        }

        protected LayoutElement layoutElement
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<LayoutElement>();
            }
        }

        protected XmlElement currentXmlElement
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<XmlElement>();
            }
        }

        public virtual RectTransform transformToAddChildrenTo
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform;
            }
        }

        protected EventTrigger eventTrigger
        {
            get
            {
                return currentXmlElement.EventTrigger;
            }
        }

        private List<string> _eventAttributeNames = new List<string>()
        {
            "onClick",
            "onMouseEnter",
            "onMouseExit",
            "onElementDropped",
            "onBeginDrag",
            "onEndDrag",
            "onDrag"
        };

        protected virtual List<string> eventAttributeNames
        {
            get
            {
                return _eventAttributeNames;
            }
        }

        public virtual string prefabPath
        {
            get
            {
                return "XmlLayout Prefabs/" + this.GetType().Name.Replace("TagHandler", "");
            }
        }

        protected string _elementName;
        public string tagType
        {
            get
            {
                if (_elementName == null) _elementName = this.GetType().Name.Replace("TagHandler", "");

                return _elementName;
            }
        }

        /// <summary>
        /// This determines which elements this element may be a child of
        /// default - Default behaviour, may be a child of any other element (with a few exceptions)                
        /// V1.1 : This is now a string, so you can use whatever value you wish
        /// </summary>
        public virtual string elementGroup
        {
            get
            {
                return "default";
            }
        }

        /// <summary>
        /// This determines which element types may be a child of _this_ element
        /// default - Any element which is part of the 'Default' group (see ElementTagHandler.elementGroup) may be a child of this element        
        /// </summary>
        public virtual string elementChildType
        {
            get
            {
                return "default";
            }
        }

        public virtual bool isCustomElement
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// If this is set to false, then this element will not be rendered, but its functions will still be executed
        /// </summary>
        public virtual bool renderElement
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Used to add non-default attributes to this element in the XSD file   
        /// It is only necessary to override this if your custom tag handler provides new attributes and you wish for Visual Studio to autocomplete them for you.
        ///  
        /// The key is the name of the attribute, and the value is the datatype. 
        /// The datatype must be a valid datatype within XmlLayout.xsd or http://www.w3.org/2001/XMLSchema
        /// e.g. 
        /// xs:string -> string value (from http://www.w3.org/2001/XMLSchema)
        /// xs:integer -> integer value (from http://www.w3.org/2001/XMLSchema)
        /// xs:float -> float value (from http://www.w3.org/2001/XMLSchema)
        /// xmlLayout:color -> color in hex/rgb/rgba format
        /// xmlLayout:vector2 -> vector 2 in x y format
        /// xmlLayout:floatList -> list of floats e.g. "10 10 10"    
        /// </summary>
        public virtual Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>();
            }
        }

        public virtual string extension
        {
            get
            {
                return "base";
            }
        }

        /// <summary>
        /// Create an instance of this tag's prefab, and make it the current instance being worked on by this tag handler
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual XmlElement GetInstance(RectTransform parent, XmlLayout xmlLayout, string overridePrefabPath = null)
        {
            currentInstanceTransform = Instantiate(parent, overridePrefabPath ?? this.prefabPath);
            var xmlElement = currentInstanceTransform.gameObject.GetComponent<XmlElement>() ?? currentInstanceTransform.gameObject.AddComponent<XmlElement>();

            xmlElement.Initialise(xmlLayout, currentInstanceTransform, this);

            var parentXmlElement = parent.GetComponent<XmlElement>();
            if (parentXmlElement != null)
            {
                parentXmlElement.AddChildElement(xmlElement);
            }

            return xmlElement;
        }

        public void SetInstance(RectTransform instanceTransform, XmlLayout xmlLayout)
        {
            currentInstanceTransform = instanceTransform;
            currentXmlLayoutInstance = xmlLayout;

            var xmlElement = this.currentXmlElement;
            if (instanceTransform != null && xmlElement == null)
            {
                // Normally this won't be necessary, but sometimes we may be applying attribute values to child elements that aren't top-level XmlElements, so just in case
                xmlElement = currentInstanceTransform.gameObject.AddComponent<XmlElement>();
            }

            if (xmlElement != null)
            {
                xmlElement.Initialise(xmlLayout, instanceTransform, this);
            }
        }

        public virtual void ApplyAttributes(AttributeDictionary attributes)
        {
            if (currentInstanceTransform == null || currentXmlLayoutInstance == null)
            {
                Debug.LogWarning("[XmlLayout][Warning] Please call ElementTagHandler.SetInstance() before using XmlElement.ApplyAttributes()");
                return;
            }

            //var startTime = DateTime.Now;
            attributes = HandleCustomAttributes(attributes);
            var _primaryComponent = primaryComponent;

            foreach (var attribute in attributes)
            {
                string name = attribute.Key;
                string value = attribute.Value;

                if (eventAttributeNames.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    HandleEventAttribute(name, value);
                    continue;
                }

                var propertySetOnComponent = _primaryComponent != null ? SetPropertyValue(_primaryComponent, name, value) : false;

                // if we failed to set the property on the component, perhaps it is a transform value instead
                if (!propertySetOnComponent)
                {
                    var propertySetOnTransform = SetPropertyValue(currentInstanceTransform, name, value);

                    // perhaps it is a layout value
                    if (!propertySetOnTransform)
                    {
                        var propertySetOnLayoutComponent = SetPropertyValue(layoutElement, name, value);

                        // or, perhaps it is an image value
                        if (!propertySetOnLayoutComponent)
                        {
                            // lastly, check the XmlElement
                            var propertySetOnXmlElement = SetPropertyValue(currentXmlElement, name, value);

                            if (!propertySetOnXmlElement)
                            {
                                var _imageComponent = imageComponent;
                                if (_imageComponent != null)
                                {
                                    SetPropertyValue(imageComponent, name, value);
                                }
                            }
                        }
                    }
                }
            }

            /*if (attributes.ContainsKey("vm-dataSource"))
            {                
                currentXmlElement.DataSource = attributes["vm-dataSource"];
                HandleDataSourceAttribute(attributes["vm-dataSource"]);
            }*/

            if (attributes.Count > 0)
            {
                //var elapsedTime = DateTime.Now - startTime;
                //Debug.Log("Took " + elapsedTime.TotalMilliseconds + "ms to execute " + this.GetType().Name + ".ApplyAttributes() to " + currentXmlLayoutInstance + "->" + currentInstanceTransform.name + " (" + attributes.Count + " attributes)");
            }
        }

        private void HandleDataSourceAttribute(string dataSource)
        {
            // remove any pre-existing entries (as the dataSource string may have changed)            
            //currentXmlLayoutInstance.ElementDataSources.RemoveAll(ed => ed.XmlElement == currentXmlElement);            
            //currentXmlLayoutInstance.ElementDataSources.Add(new XmlElementDataSource(dataSource, currentXmlElement));            
        }

        protected bool SetPropertyValue(object o, string propertyName, string value)
        {
            if (o == null) return false;

            var bindingFlags = System.Reflection.BindingFlags.Public
                             | System.Reflection.BindingFlags.IgnoreCase
                             | System.Reflection.BindingFlags.Instance;            

            var type = o.GetType();

            //var fieldInfo = type.GetField(propertyName, bindingFlags);
            var fieldInfo = GetComponentXmlField(type, propertyName);

            try
            {
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(o, value.ChangeToType(fieldInfo.FieldType));
                    return true;
                }
                else
                {
                    var propertyInfo = type.GetProperty(propertyName, bindingFlags);

                    if (propertyInfo != null && propertyInfo.GetSetMethod(false) != null)
                    {
                        propertyInfo.SetValue(o, value.ChangeToType(propertyInfo.PropertyType), null);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[XmlLayout] " + e.Message + " (propertyName == '" + propertyName + "',value == '" + value + "')");
            }

            // We didn't find a property to set
            return false;
        }

        protected virtual void HandleEventAttribute(string eventName, string eventValue)
        {
            var layout = currentXmlLayoutInstance;

            if (layout.XmlLayoutController == null)
            {
                Debug.LogError("[XmlLayout] Attempted to process an event attribute for an XmlLayout with no XmlLayoutController attached.");
                return;
            }

            var eventData = eventValue.Trim(new Char[] { ')', ';' })
                                      .Split(',', '(');
            string value = null;
            if (eventData.Count() > 1)
            {
                value = eventData[1];
            }

            if (eventName.Equals("OnElementDropped", StringComparison.OrdinalIgnoreCase))
            {
                HandleOnDroppedEventAttribute(eventData[0]);
                return;
            }

            var transform = currentInstanceTransform;
            var _component = primaryComponent;
            var type = _component.GetType();
            var interactablePropertyInfo = type.GetProperty("interactable");

            var xmlElement = currentInstanceTransform.GetComponent<XmlElement>();

            Action action = () =>
            {
                bool interactable = true;

                if (interactablePropertyInfo != null)
                {
                    interactable = (bool)interactablePropertyInfo.GetValue(_component, null);
                }

                if (interactable)
                {
                    layout.XmlLayoutController.ReceiveMessage(eventData[0], value, transform);
                }
            };

            switch (eventName.ToLower())
            {
                case "onclick":
                    xmlElement.AddOnClickEvent(action, true);
                    break;
                case "onmouseenter":
                    xmlElement.AddOnMouseEnterEvent(action, true);
                    break;
                case "onmouseexit":
                    xmlElement.AddOnMouseExitEvent(action, true);
                    break;
                case "ondrag":
                    xmlElement.AddOnDragEvent(action, true);
                    break;
                case "onbegindrag":
                    xmlElement.AddOnBeginDragEvent(action, true);
                    break;
                case "onenddrag":
                    xmlElement.AddOnEndDragEvent(action, true);
                    break;
                default:
                    Debug.LogWarning("[XmlLayout] Unknown event type: '" + eventName + "'");
                    return;
            }            
        }

        protected void HandleOnDroppedEventAttribute(string value)
        {
            var xmlElement = currentInstanceTransform.GetComponent<XmlElement>();
            var layout = currentXmlLayoutInstance;

            Action<XmlElement, XmlElement> action = (item, droppedOn) =>
            {
                layout.XmlLayoutController.ReceiveElementDroppedMessage(value, item, droppedOn);
            };

            xmlElement.AddOnElementDroppedEvent(action);
        }

        /// <summary>
        /// Convert custom attributes (that aren't found via reflection, e.g. width/height) into useable values
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        protected AttributeDictionary HandleCustomAttributes(AttributeDictionary attributes)
        {
            var elementName = this.GetType().Name.Replace("TagHandler", "");
            var customAttributes = attributes.Where(k => XmlLayoutUtilities.IsCustomAttribute(k.Key))
                                             .ToList();

            foreach (var attribute in customAttributes)
            {
                var customAttribute = XmlLayoutUtilities.GetCustomAttribute(attribute.Key);

                if (customAttribute.RestrictToPermittedElementsOnly)
                {
                    if (!customAttribute.PermittedElements.Contains(elementName, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                attributes = XmlLayoutUtilities.MergeAttributes(
                                        attributes,
                                        customAttribute.Convert(attribute.Value, attributes.Clone(), this.currentXmlElement));

                customAttribute.Apply(currentXmlElement, attribute.Value, attributes.Clone());

                if (!customAttribute.KeepOriginalTag)
                {
                    attributes.Remove(attribute.Key);
                }
            }

            return attributes;
        }

        protected RectTransform Instantiate(RectTransform parent, string name = "")
        {
            var prefab = XmlLayoutUtilities.LoadResource<GameObject>(name);
            GameObject gameObject = null;
            RectTransform transform = null;

            if (prefab != null)
            {
                gameObject = GameObject.Instantiate<GameObject>(prefab);
                transform = gameObject.GetComponent<RectTransform>();

                transform.SetParent(parent);

                FixInstanceTransform(prefab.transform as RectTransform, transform);
            }
            else
            {
                if (!String.IsNullOrEmpty(name))
                {
                    Debug.Log("Warning: prefab '" + name + "' not found.");
                }
                gameObject = new GameObject(name);
            }

            if (transform == null)
            {
                transform = gameObject.AddComponent<RectTransform>();
            }

            if (name != null && name.Contains("/") && !name.EndsWith("/"))
            {
                name = name.Substring(name.LastIndexOf("/") + 1);
            }

            gameObject.name = name ?? "Xml Element";

            if (transform.parent != parent)
            {
                transform.SetParent(parent);
            }

            return transform;
        }

        protected static void FixInstanceTransform(RectTransform baseTransform, RectTransform instanceTransform)
        {
            instanceTransform.localPosition = baseTransform.localPosition;
            instanceTransform.position = baseTransform.position;
            instanceTransform.rotation = baseTransform.rotation;
            instanceTransform.localScale = baseTransform.localScale;
            instanceTransform.anchoredPosition = baseTransform.anchoredPosition;
            instanceTransform.sizeDelta = baseTransform.sizeDelta;

            instanceTransform.position = new Vector3(instanceTransform.position.x, instanceTransform.position.y, 0);
            instanceTransform.anchoredPosition3D = new Vector3(baseTransform.anchoredPosition3D.x, baseTransform.anchoredPosition3D.y, 0);
        }

        /// <summary>
        /// Called when the tag is opened
        /// </summary>
        public virtual void Open(AttributeDictionary attributes)
        {
        }

        /// <summary>
        /// Called when the tag is closed
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// If this function returns true, then XmlLayout will consider this element to be completely parsed and will not try to parse the child nodes normally.
        /// Only overriden for a select few element tag handlers, e.g. Dropdown.
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public virtual bool ParseChildElements(XmlNode xmlNode)
        {
            return false;
        }

        public void RemoveElement()
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(currentXmlElement.gameObject);
            }
            else
            {
                GameObject.DestroyImmediate(currentXmlElement.gameObject);
            }
        }

        public virtual void SetValue(string newValue)
        {
            // default behaviour: set text attribute
            ApplyAttributes(
                new AttributeDictionary()
                {
                    {"text", newValue}
                });
        }
    }
}