using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;

namespace UI.Xml
{
    public class XmlLayoutMenuItems
    {
        [MenuItem("Assets/Create/XmlLayout/New XmlLayout Xml File")]
        private static void NewXmlLayoutXmlFileMenuItem()
        {
            NewXmlLayoutXmlFile();
        }

        public static string NewXmlLayoutXmlFile(string fileName = "NewXmlLayoutFile", bool selectAsset = true)
        {
            return CreateAsset(
                fileName, "xml",
                @"
<XmlLayout xmlns=""http://www.w3schools.com""
           xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
           xsi:noNamespaceSchemaLocation=""xmlLayoutXSDPath"">


</XmlLayout>
                ", selectAsset);
        }

        [MenuItem("Assets/Create/XmlLayout/New XmlLayout Controller")]
        private static void NewXmlLayoutControllerMenuItem()
        {
            NewXmlLayoutController();
        }
        
        public static string NewXmlLayoutController(string filename = "NewXmlLayoutController", bool selectAsset = true)
        {
            return CreateAsset(
                filename, "cs",
                @"
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Xml;

class ClassName : XmlLayoutController
{    
    public override void LayoutRebuilt(ParseXmlResult parseResult)
    {
        // ParseXmlResult.Changed   => The Xml was parsed and the layout changed as a result
        // ParseXmlResult.Unchanged => The Xml was unchanged, so no the layout remained unchanged
        // ParseXmlResult.Failed    => The Xml failed validation

        // Called whenever the XmlLayout finishes rebuilding the layout
        // Use this function to make any dynamic changes (e.g. create dynamic lists, menus, etc.) or dynamically load values/selections for elements such as DropDown
    }
}
            ", selectAsset);
        }

        [MenuItem("Assets/Create/XmlLayout/New Custom Attribute")]
        private static void NewCustomAttributeMenuItem()
        {
            CreateAsset(
                "NewCustomXmlAttribute", "cs",
                @"
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Xml;

public class ClassName : CustomXmlAttribute
{
    public override AttributeDictionary Convert(string value, AttributeDictionary elementAttributes, XmlElement xmlElement)
    {
        // In this function you can create new attribute/value pairs
        // to translate an attribute into one or more new ones
        // e.g. UI.Xml.CustomAttributes.ImageAttribute => converts the 'image' attribute to 'sprite'
        return base.Convert(value, elementAttributes, xmlElement);
    }

    public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
    {
        // In this function you can apply changes to the xmlElement and/or its component MonoBehaviours directly
        // e.g. UI.Xml.CustomAttributes.ActiveAttribute => Enables / Disables GameObjects based on 'value'
        base.Apply(xmlElement, value, elementAttributes);
    }
}
            ");
        }

        [MenuItem("Assets/Create/XmlLayout/New XmlLayout Element Tag Handler")]
        private static void NewXmlLayoutElementTagHandlerMenuItem()
        {
            CreateAsset(
                "NewTagHandler", "cs",
                @"
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Xml;

// The Tag name will be determined as the name of the class with 'TagHandler' removed,
// e.g. TextTagHandler -> <Text>
public class ClassName : ElementTagHandler
{    
    public override MonoBehaviour primaryComponent
    {
        get
        {
            if (currentInstanceTransform == null) return null;

            // Replace <Text> with the MonoBehaviour that you feel best fits as the 'primary' component of this Xml Tag
            // e.g. <Text> -> Text, <Button> -> Button
            return currentInstanceTransform.GetComponent<Text>();
        }
    }

    public override string prefabPath
    {
        get
        {
            // by default, XmlLayout will use the following path: Resources/XmlLayout Prefabs/{Element Name}TagHandler
            // if you wish to use this path, you can safely remove this property
            // otherwise, you may define a new path here - please note that the Prefab path must be located within a Resources directory
            return base.prefabPath;
        }
    }

    public override bool ParseChildElements(System.Xml.XmlNode xmlNode)
    {
        // If this function returns true, then XmlLayout will NOT attempt to process any nested child elements further
        // This is used to process elements such as Text or DropDown which have special handling and do not supported regular nested elements


        // if no special child handling is required, then this method can safely be removed

        return base.ParseChildElements(xmlNode);            
    }

    public override void ApplyAttributes(AttributeDictionary attributes)
    {
        base.ApplyAttributes(attributes);

        // Add any special attribute handling here, e.g.
        /*
            var textComponent = primaryComponent as Text;
            if(attributes.ContainsKey(""text"")) textComponent.text = attributes[""text""];
        */
        // A large number of string extension functions have been provided in the UI.Xml namespace, e.g. ToColor(), ToColorBlock(), ToVector2(), ToRectOffset(), ToSprite(), ToFont(), etc.
        // The full list is available in the UI.Xml.ConversionExtensions class
        // Please note that ALL attribute names within the AttributeDictionary be in lower case within this method

        // if no special attribute handling is required, this method can safely be removed
    }

    protected override void HandleEventAttribute(string eventName, string eventValue)
    {
        // Add any special event handling here
        // for examples on how to do this, see the source code of:
        // - ToggleTagHandler
        // - ToggleGroupTagHandler

        // Please note that this function will ONLY be called for event attributes which have been defined
        // by the propery 'EventAttributeNames' which can be overriden in this class to define custom names
        // The default EventAttributeNames are:
        // -onClick
        // -onMouseEnter
        // -onMouseExit

            
        base.HandleEventAttribute(eventName, eventValue);
    }    

    public override bool isCustomElement
    {
        get
        {
            return true;
        }
    }
}
            ");
        }

        private static string CreateAsset(string className, string type, string template, bool selectAsset = true)
        {
            var filePath = GetSelectedPath();                         

            filePath = AssetDatabase.GenerateUniqueAssetPath(String.Format("{0}/{1}", filePath, className)) + "." + type;

            int i = 1;
            while (File.Exists(String.Format("{0}/{1}", Application.dataPath, filePath.Substring("Assets/".Length))))
            {
                if (i == 1)
                {
                    className += "_1";
                    filePath = filePath.Replace("." + type, "_1." + type);                                        
                }
                else
                {
                    var oldClassName = className;
                    
                    className = className.Replace(String.Format("_{0}", (i - 1)), String.Format("_{0}", i));
                    
                    filePath = filePath.Replace(oldClassName, className);                    
                }

                i++;
            }

            var fileContents = template.Replace("ClassName", className).Trim();
            var fullFilePath = String.Format("{0}/{1}", Application.dataPath, filePath.Substring("Assets/".Length));

            if (type == "xml")
            {
                var xsdPath = Application.dataPath + "/" + AssetDatabase.GetAssetPath(XmlLayoutUtilities.XmlLayoutConfiguration.XSDFile).Substring("Assets/".Length);
                var relativePath = new Uri(fullFilePath).MakeRelativeUri(new Uri(xsdPath)).ToString();
                fileContents = fileContents.Replace("xmlLayoutXSDPath", relativePath);
            }

            File.WriteAllText(fullFilePath, fileContents);

            AssetDatabase.Refresh();

            if (selectAsset)
            {
                // we're actually hijacking this window for it's Update() function
                // which will be called before and after the editor has finished compiling
                // (tried to use EditorApplication.delayCall, but it didn't get called once compilation was complete)
                var window = EditorWindow.GetWindow<XmlLayoutSelectNewAssetWindow>();
                window.SelectAsset(filePath);
            }

            return filePath;
        }

        private static string GetSelectedPath()
        {
            string path = "Assets";
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }

            return path;
        }
    }

    public class XmlLayoutSelectNewAssetWindow : EditorWindow
    {
        string m_path = null;
        bool m_waitingForCompilation = false;


        public void SelectAsset(string path)
        {
            m_path = path;
            m_waitingForCompilation = true;
                        
            this.minSize = this.maxSize = new Vector2(300, 32);
            
            var pos = this.position;
            pos.x = (Screen.width - pos.width) * 0.5f;
            pos.y = (Screen.height - pos.height) * 0.5f;                        

            this.position = pos;
        }

        void OnGUI()
        {
            GUILayout.Label("Please wait - waiting for Unity to finish compiling...");
        }

        void Update()
        {
            if (!m_waitingForCompilation) return;            

            if (!EditorApplication.isCompiling)
            {                
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(m_path);

                EditorUtility.FocusProjectWindow();                
                ProjectWindowUtil.ShowCreatedAsset(asset);

                var projectWindow = EditorWindow.focusedWindow;
                
                EditorApplication.delayCall += () =>
                {
                    Selection.activeInstanceID = asset.GetInstanceID();
                    EditorApplication.delayCall += () => projectWindow.SendEvent(new Event { keyCode = KeyCode.F2, type = EventType.KeyDown });
                };
                
                m_waitingForCompilation = false;
                m_path = null;

                this.Close();
            }
        }

        [MenuItem("Assets/XmlLayout/Configuration")]
        public static void OpenXmlLayoutConfigurationMenuItem()
        {
            var configFile = XmlLayoutUtilities.XmlLayoutConfiguration;                        
            Selection.activeObject = configFile;
            EditorGUIUtility.PingObject(configFile);
        }
    }    
}
