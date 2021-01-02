using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;

namespace UI.Xml
{
    [ExecuteInEditMode]
    public partial class XmlLayout : MonoBehaviour
    {
        [Tooltip("If this is set to true, then XmlLayout will preload some of its functionality in advance. This will mean that there will be a slight performance hit the first time an XmlLayout is loaded. Without the preload, there will be a minor performance hit each time a new Xml Tag type is parsed.")]
        public bool PreloadXmlLayoutCache = true;

        public TextAsset XmlFile;

        [Tooltip("Automatically reload Xml file if it is changed? Note: This will override the Xml property, and it will only work in the Unity Editor.")]
        public bool AutomaticallyReloadXmlFileIfItChanges = true;

        [Tooltip("If set to true, this XmlLayout will automatically rebuild when Awake() is called. This should always be set if this XmlLayout loads data dynamically.")]
        public bool ForceRebuildOnAwake = true;

        [Tooltip("If set to true, this XmlLayout will automatically reload the Xml from the XmlFile when Awake() is called.")]
        public bool ForceReloadXmlFileOnAwake = false;

        [TextArea]
        public string Xml = "<XmlLayout>\r\n</XmlLayout>";

        [Tooltip("An optional list of Xml files which contain default values (such as element styles).")]
        public List<TextAsset> DefaultsFiles;

        /// <summary>
        /// Used by the editor to determine whether or not to trigger an update if a file which has been included (via the <Include> tag) has been updated
        /// </summary>
        [SerializeField, HideInInspector]
        public List<string> IncludedFiles = new List<string>();

        /// <summary>
        /// Used by the editor to determine whether or not to show the XML code
        /// </summary>
        public bool editor_showXml = false;
        public Vector2 editor_xmlScrollPosition = new Vector2();

        [SerializeField]
        private string previousXml = "";

        [SerializeField]
        public ElementDictionary ElementsById = new ElementDictionary();

        private XmlLayoutController _xmlLayoutController;
        public XmlLayoutController XmlLayoutController
        {
            get
            {
                if (_xmlLayoutController == null)
                {
                    _xmlLayoutController = this.GetComponent<XmlLayoutController>();
                }

                return _xmlLayoutController;
            }
        }

        private XmlElement m_XmlElement;
        public XmlElement XmlElement
        {
            get
            {
                if (m_XmlElement == null)
                {
                    m_XmlElement = GetComponent<XmlElement>();
                }

                if (m_XmlElement == null)
                {
                    m_XmlElement = gameObject.AddComponent<XmlElement>();
                    m_XmlElement.Initialise(this, this.transform as RectTransform, XmlLayoutUtilities.GetXmlTagHandler("XmlLayout"));
                }

                return m_XmlElement;
            }
        }

        [SerializeField]
        public DefaultAttributeValueDictionary defaultAttributeValues = new DefaultAttributeValueDictionary();

        private bool m_awake = false;
        public bool IsReady { get; protected set; }

        protected XmlLayoutTooltip m_Tooltip;
        public XmlLayoutTooltip Tooltip
        {
            get
            {
                if (m_Tooltip == null) CreateTooltipObject();

                return m_Tooltip;
            }
        }

        // Used to store default state values
        [SerializeField]
        protected AttributeDictionary m_defaultTooltipAttributes = new AttributeDictionary();

        void Awake()
        {
            m_awake = true;

            if (Application.isPlaying)
            {
                if (PreloadXmlLayoutCache) HandlePreload();

                if (ForceRebuildOnAwake)
                {
                    if (XmlFile != null && ForceReloadXmlFileOnAwake) ReloadXmlFile();
                    else RebuildLayout(true);
                }
            }

            IsReady = true;
        }

#if UNITY_EDITOR
        void Update()
        {
            if (UnityEditor.EditorApplication.isCompiling && !Application.isPlaying) RebuildLayout(true);
        }
#endif

        public void ReloadXmlFile()
        {
            if (XmlFile != null)
            {
                Xml = XmlFile.text;
                RebuildLayout(true);
            }
        }

        void CreateTooltipObject()
        {            
            var prefab = XmlLayoutUtilities.LoadResource<GameObject>("XmlLayout Prefabs/Tooltip");
            m_Tooltip = ((GameObject)Instantiate(prefab)).GetComponent<XmlLayoutTooltip>();
            m_Tooltip.transform.SetParent(this.transform);
            m_Tooltip.transform.localPosition = Vector3.zero;
            m_Tooltip.transform.localScale = Vector3.one;
            m_Tooltip.name = "Tooltip";
            m_Tooltip.gameObject.SetActive(false);
        }

        void ClearContents()
        {
            if (this == null) return;

            for (var x = 0; x < transform.childCount; x++)
            {
                if (Application.isPlaying) Destroy(transform.GetChild(x).gameObject);
                else DestroyImmediate(transform.GetChild(x).gameObject);
            }

            IncludedFiles.Clear();
            ElementsById.Clear();
            //ElementDataSources.Clear();
            defaultAttributeValues.Clear();

            if (m_Tooltip != null) Destroy(m_Tooltip.gameObject);
        }

        /// <summary>
        /// Clear the contents of this XmlLayout and rebuild it (using the Xml value)
        /// Call this after changing the Xml value for changes to take effect
        /// </summary>
        public void RebuildLayout(bool forceEvenIfXmlUnchanged = false, bool throwExceptionIfXmlIsInvalid = false)
        {
            if (!forceEvenIfXmlUnchanged && (!this.gameObject.activeInHierarchy || !m_awake)) return;
            
            var parseResult = ParseXml(null, true, true, forceEvenIfXmlUnchanged, throwExceptionIfXmlIsInvalid);

            // Notify the XmlLayoutController that the Layout has been rebuilt
            if (XmlLayoutController != null)
            {
                XmlLayoutController.ViewModelUpdated();
                XmlLayoutController.NotifyXmlElementReferencesOfLayoutRebuild();
                XmlLayoutController.LayoutRebuilt(parseResult);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying && parseResult != ParseXmlResult.Unchanged)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(this.gameObject.scene);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlToParse">The xml to parse. If this argument is null, then this method will use this XmlLayout's 'Xml' property instead.</param>
        /// <param name="clearContents">Should the contents of this XmlLayout be cleared before parsing the xml?</param>
        /// <param name="loadDefaultsFiles">Should the defaults files (if any) be loaded before parsing the xml?</param>
        /// <param name="forceEvenIfXmlUnchanged">Should the xml be parsed if it hasn't changed? (only applicable if xmlToParse is null)</param>
        /// <param name="throwExceptionIfXmlIsInvalid">Should an exception be thrown if the Xml contains errors?</param>
        private ParseXmlResult ParseXml(string xmlToParse = null,
                                bool clearContents = true,
                                bool loadDefaultsFiles = true,
                                bool forceEvenIfXmlUnchanged = false,
                                bool throwExceptionIfXmlIsInvalid = false)
        {
            if (xmlToParse == null)
            {
                if (!forceEvenIfXmlUnchanged)
                {
                    // If our Xml hasn't changed, and we aren't required to force update, then don't continue
                    // Note: this only happens if xmlToParse is null (which means we are parsing the primary Xml of this XmlLayout)
                    if (previousXml.Equals(Xml)) return ParseXmlResult.Unchanged;
                }

                previousXml = Xml;

                xmlToParse = Xml;
            }

            if (XmlLayoutController != null)
            {
                xmlToParse = XmlLayoutController.ProcessViewModel(xmlToParse);
            }

            xmlToParse = HandleLocalization(xmlToParse);            

            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xmlToParse);
            }
            catch (XmlException e)
            {
                var message = String.Format("[XmlLayout][{0}] Error parsing XML data: {1}", this.name, e.Message);
                Debug.LogError(message);

                if (throwExceptionIfXmlIsInvalid)
                {
                    throw e;
                }

                return ParseXmlResult.Failed;
            }

            if (clearContents)
            {
                // For some reason, especially in edit mode, iterating through the child objects of this transform doesn't always iterate through all of the objects
                // repeating the action twice seems to be sufficient                
                for (var x = 0; x < 2; x++)
                {
                    ClearContents();
                }
            }

            if (loadDefaultsFiles && DefaultsFiles != null)
            {
                defaultAttributeValues.Clear();

                DefaultsFiles.ForEach(f =>
                {
                    if (f != null) ParseXml(f.text, false, false, true);
                });
            }

            var rectTransform = this.transform as RectTransform;

            ParseNode(xmlDoc.DocumentElement, rectTransform, rectTransform, true);

            return ParseXmlResult.Changed;
        }

        private void ParseNode(XmlNode xmlNode, RectTransform parent, RectTransform element = null, bool parseChildren = true, XmlElement parentXmlElement = null)
        {
            if (xmlNode.NodeType == XmlNodeType.Text || xmlNode.NodeType == XmlNodeType.Comment) return;

            var type = xmlNode.Name.ToLower();

            if (type == "include")
            {
                //LoadIncludeFile(xmlNode);
                LoadInlineIncludeFile(xmlNode, parent);
                return;
            }

            if (type == "defaults")
            {
                LoadDefaults(xmlNode);
                return;
            }

            var attributes = xmlNode.Attributes.ToAttributeDictionary();

            var tagHandler = XmlLayoutUtilities.GetXmlTagHandler(type);
            if (tagHandler == null) return;

            tagHandler.SetInstance(element, this);

            XmlElement xmlElement = null;
            if (element == null)
            {
                xmlElement = tagHandler.GetInstance(parent, this, attributes.GetValue("prefabPath"));

                if (parentXmlElement != null)
                {
                    parentXmlElement.AddChildElement(xmlElement, false);
                }
            }

            var tagTransform = element ?? xmlElement.rectTransform;

            tagHandler.SetInstance(tagTransform, this);
            if (xmlElement != null)
            {
                xmlElement.attributes = attributes;
                //xmlElement.DataSource = xmlElement.GetAttribute("vm-dataSource");
            }
            tagHandler.Open(attributes);

            // if the tag handler successfully parses the child nodes, then don't attempt to parse them here
            // (this is only used for specific elements, e.g. Dropdown)
            if (tagHandler.ParseChildElements(xmlNode)) parseChildren = false;

            if (parseChildren && xmlNode.HasChildNodes)
            {
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Text || childNode.NodeType == XmlNodeType.CDATA)
                    {
                        if (!attributes.ContainsKey("text")) attributes.Add("text", "");
                        var text = childNode.NodeType == XmlNodeType.Text ? childNode.ParentNode.InnerText.Trim() : childNode.InnerText.Trim();

                        // Strip out any instances of multiple spaces (as in HTML)
                        while (text.Contains("  "))
                        {
                            text = text.Replace("  ", " ");
                        }

                        while (text.Contains("\r\n "))
                        {
                            text = text.Replace("\r\n ", "\r\n");
                        }

                        attributes["text"] = text;
                    }
                    else
                    {
                        tagHandler.SetInstance(tagTransform, this);
                        ParseNode(childNode, tagHandler.transformToAddChildrenTo, null, true, parentXmlElement);
                    }
                }
            }

            tagHandler.SetInstance(tagTransform, this);

            if (attributes.ContainsKey("id"))
            {
                if (ElementsById.ContainsKey(attributes["id"]))
                {
                    Debug.LogError("[XmlLayout] Ignoring duplicate id value '" + attributes["id"] + ". Id values must be unique.");
                }
                else
                {
                    if (xmlElement != null)
                    {
                        ElementsById.Add(attributes["id"], xmlElement);
                    }
                }
            }            

            if (defaultAttributeValues.ContainsKey(type))
            {
                var defaultAttributesMerged = new AttributeDictionary();

                if (defaultAttributeValues[type].ContainsKey("all"))
                {
                    defaultAttributesMerged = defaultAttributeValues[type]["all"];
                }

                if (attributes.ContainsKey("class"))
                {
                    var classes = attributes["class"].Split(',', ' ').Select(s => s.Trim().ToLower()).ToList();

                    if (xmlElement != null) xmlElement.classes = classes;

                    foreach (var _class in classes)
                    {
                        if (defaultAttributeValues[type].ContainsKey(_class))
                        {
                            defaultAttributesMerged = XmlLayoutUtilities.MergeAttributes(defaultAttributesMerged, defaultAttributeValues[type][_class]);
                        }
                    }
                }

                attributes = XmlLayoutUtilities.MergeAttributes(defaultAttributesMerged, attributes);
            }

            if (xmlElement != null)
            {
                xmlElement.ApplyAttributes(attributes);
            }
            else
            {
                tagHandler.ApplyAttributes(attributes);
            }

            tagHandler.Close();

            if (!tagHandler.renderElement)
            {
                tagHandler.RemoveElement();
            }
        }

        void LoadDefaults(XmlNode node)
        {
            if (node.HasChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Text || childNode.NodeType == XmlNodeType.Comment) continue;

                    var type = childNode.Name.ToLower();

                    if (type == "tooltip")
                    {
                        HandleDefaultTooltipNode(childNode);
                        continue;
                    }

                    if (XmlLayoutUtilities.GetXmlTagHandler(type) == null)
                    {
                        continue;
                    }

                    var attributes = childNode.Attributes.ToAttributeDictionary();

                    var classes = attributes.ContainsKey("class") ? attributes["class"].Split(',', ' ').Select(s => s.Trim().ToLower()).ToList() : new List<string>() { "all" };

                    foreach (var _class in classes)
                    {
                        if (!defaultAttributeValues.ContainsKey(type))
                        {
                            defaultAttributeValues.Add(type, new ClassAttributeCollectionDictionary());
                        }

                        if (!defaultAttributeValues[type].ContainsKey(_class))
                        {
                            defaultAttributeValues[type].Add(_class, new AttributeDictionary());
                        }

                        defaultAttributeValues[type][_class] = XmlLayoutUtilities.MergeAttributes(defaultAttributeValues[type][_class], attributes);
                        defaultAttributeValues[type][_class].Remove("class");
                    }
                }
            }
        }

        void LoadIncludeFile(string path)
        {
            var xmlFile = Resources.Load(path) as TextAsset;
            if (xmlFile == null)
            {
                Debug.LogError("[XmlLayout] Unable to locate xml file using path '" + path + "'. Please ensure that the file is located within a Resources folder.");
                return;
            }

            ParseXml(xmlFile.text, false, false, true);

            if (!IncludedFiles.Contains(path))
            {
                IncludedFiles.Add(path);
            }
        }

        void LoadIncludeFile(XmlNode node)
        {
            var path = node.Attributes["path"].Value;

            // strip out the file extension, if provided
            path = path.Replace(".xml", "");

            LoadIncludeFile(path);
        }

        void LoadInlineIncludeFile(XmlNode node, RectTransform parent)
        {
            var path = node.Attributes["path"].Value;

            // strip out the file extension, if provided
            path = path.Replace(".xml", "");

            var xmlFile = Resources.Load<TextAsset>(path);

            if (xmlFile == null)
            {
                Debug.LogError(String.Format("[XmlLayout][{0}] Error locating include file : '{1}'.", this.name, path));
                return;
            }

            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xmlFile.text);
            }
            catch (XmlException e)
            {
                var message = String.Format("[XmlLayout][{0}] Error parsing XML data: {1}", this.name, e.Message);
                Debug.LogError(message);

                return;
            }

            if (!IncludedFiles.Contains(path))
            {
                IncludedFiles.Add(path);
            }

            var rootNode = xmlDoc.FirstChild;

            foreach (XmlNode childNode in rootNode)
            {
                ParseNode(childNode, parent);
            }
        }

        void HandleDefaultTooltipNode(XmlNode node)
        {
            m_defaultTooltipAttributes = node.Attributes.ToAttributeDictionary();
        }

        /// <summary>
        /// Return a specific element within this XmlLayout
        /// Note: the element must have the id attribute set in order to use this function
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public XmlElement GetElementById(string id)
        {
            if (ElementsById.ContainsKey(id))
            {
                return ElementsById[id];
            }

            return null;
        }

        /// <summary>
        /// Return the component of a specific element within this XmlLayout.
        /// Note   : the element must have the id attribute set in order to use this function
        /// Note 2 : the component must exist on the target element
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetElementById<T>(string id)
        {
            if (ElementsById.ContainsKey(id))
            {
                var t = ElementsById[id];
                var component = t.GetComponent<T>();

                if (component != null) return component;
            }

            return default(T);
        }

        /// <summary>
        /// Get the string id (if any) of a RectTRansform element in this XmlLayout
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string GetElementId(RectTransform element)
        {
            if (ElementsById.Any(e => e.Value.rectTransform == element))
            {
                return ElementsById.First(kvp => kvp.Value.rectTransform == element).Key;
            }

            return null;
        }

        /// <summary>
        /// Get the value of all child XmlElements by element id
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetFormData()
        {
            return XmlElement.GetFormData(UI.Xml.XmlElement.eLocateElementsBy.Id);
        }

        void OnRectTransformDimensionsChange()
        {
            if (!gameObject.activeInHierarchy) return;

            if (!XmlElement.IsAnimating)
            {
                RebuildLayout(true);
            }
        }

        public void Show()
        {
            XmlElement.Show();
        }

        public void Hide(Action onCompleteCallback = null)
        {
            XmlElement.Hide(false, onCompleteCallback);
        }

        void HandlePreload()
        {
            var preloader = this.GetComponent<XmlLayoutPreloader>();
            if (preloader == null) preloader = GameObject.FindObjectOfType<XmlLayoutPreloader>();
            if (preloader == null)
            {
                // Only call Preload if we actually had to create the preload instance
                preloader = this.gameObject.AddComponent<XmlLayoutPreloader>();
                preloader.Preload();
            }
        }


        protected XmlElement m_CurrentTooltipElement;
        public void ShowTooltip(XmlElement element, string tooltipContent)
        {
            m_CurrentTooltipElement = element;

            Tooltip.LoadAttributes(m_defaultTooltipAttributes);            
            
            Tooltip.gameObject.SetActive(true);

            Tooltip.SetText(tooltipContent);

            Tooltip.SetStylesFromXmlElement(element);            
           
            if (!Tooltip.followMouse)
            {                
                Tooltip.SetPositionAdjacentTo(element);

                // the size/etc. of the tooltip may change as a result of text and styles, but it appears that the rectTransform values will not be updated until the the end of the current frame
                // as such, we need to call SetTooltipPositionAdjacentTo again in one frame, just in case. This is primarily so that the tooltip will be clamped within the canvas area.            
                XmlLayoutTimer.DelayedCall(0, () => { Tooltip.SetPositionAdjacentTo(element);  }, this);
            }
        }        

        public void HideTooltip(XmlElement sourceElement)
        {
            if (sourceElement == m_CurrentTooltipElement)
            {
                Tooltip.gameObject.SetActive(false);
            }
        }
    }

    [Serializable]
    public class ElementDictionary : SerializableDictionary<string, XmlElement>
    {
        public ElementDictionary()
        {
            _Comparer = StringComparer.OrdinalIgnoreCase;
        }
    }

    [Serializable]
    public class AttributeDictionary : SerializableDictionary<string, string>
    {
        public AttributeDictionary(IDictionary<string, string> attributes = null)
        {
            _Comparer = StringComparer.OrdinalIgnoreCase;

            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    this.Add(attribute.Key, attribute.Value);
                }
            }
        }

        public AttributeDictionary Clone()
        {
            return new AttributeDictionary(this);
        }

        public string GetValue(string key)
        {
            if (this.ContainsKey(key)) return this[key];

            return null;
        }

        public T GetValue<T>(string key)
        {
            return GetValue(key).ChangeToType<T>();
        }

        public override string ToString()
        {
            var s = "AttributeDictionary Values:\n";

            foreach (var kvp in this)
            {
                s += String.Format("[{0}] => '{1}'\n", kvp.Key, kvp.Value);
            }

            return s;
        }
    }

    [Serializable]
    public class ClassAttributeCollectionDictionary : SerializableDictionary<string, AttributeDictionary>
    {
        public ClassAttributeCollectionDictionary()
        {
            _Comparer = StringComparer.OrdinalIgnoreCase;
        }
    }

    [Serializable]
    public class DefaultAttributeValueDictionary : SerializableDictionary<string, ClassAttributeCollectionDictionary>
    {
        public DefaultAttributeValueDictionary()
        {
            _Comparer = StringComparer.OrdinalIgnoreCase;
        }
    }
}