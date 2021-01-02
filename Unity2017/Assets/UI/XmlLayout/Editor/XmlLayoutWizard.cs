using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

namespace UI.Xml
{
    public class XmlLayoutWizard : EditorWindow
    {
        string xmlFileName = "None";
        string controllerName = "None";

        string newXmlFileName = "NewXmlFile";
        string newControllerName = "NewXmlLayoutController";

        List<string> xmlFileNames;
        List<string> controllerNames;

        [MenuItem("GameObject/UI/XmlLayout/Add New XmlLayout")]
        static void CreateNewXmlLayoutMenuItem()
        {
            var window = EditorWindow.GetWindow<XmlLayoutWizard>();
            window.Show();

            var width = 600f;
            var height = 230f;

            window.titleContent = new GUIContent("Add New XmlLayout");
            window.position = new Rect((Screen.currentResolution.width - width) / 2f,
                                        (Screen.currentResolution.height - height) / 2f,
                                        width,
                                        height);  
        }

        void OnEnable()
        {
            controllerNames = XmlLayoutUtilities.GetXmlLayoutControllerNames();
            controllerNames.Insert(0, "Create New");
            controllerNames.Insert(0, "None");

            xmlFileNames = GetAssetsOfType<TextAsset>(".xml").Select(x => AssetDatabase.GetAssetPath(x).Substring("Assets/".Length)).ToList();
            xmlFileNames.Insert(0, "Create New");
            xmlFileNames.Insert(0, "None");            
        }

        void OnGUI()
        {
            float spaceRemaining = 74;

            var style = new GUIStyle();
            style.margin = new RectOffset(10, 10, 10, 10);

            GUILayout.BeginVertical(style);
            
            // a) Choose whether or not to use an Xml file            
            //      i) Create a new one
            //      ii) Use an existing one

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Xml File", GUILayout.Width(200));            
            int xmlIndex = EditorGUILayout.Popup(xmlFileNames.IndexOf(xmlFileName), xmlFileNames.ToArray());            
            xmlFileName = xmlFileNames[xmlIndex];            
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            if (xmlFileName == "Create New")
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("New Xml File Name", GUILayout.Width(200));
                newXmlFileName = EditorGUILayout.TextField(newXmlFileName);
                GUILayout.EndHorizontal();
                GUILayout.Space(4);

                spaceRemaining -= 20;
            }

            // b) Choose whether to use an XmlLayoutController
            //      i)  Create a new one
            //      ii) Use an existing one
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Xml Layout Controller", GUILayout.Width(200));
            int index = EditorGUILayout.Popup(controllerNames.IndexOf(controllerName), controllerNames.ToArray());
            controllerName = controllerNames[index];
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            if (controllerName == "Create New")
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("New Xml Layout Controller Name", GUILayout.Width(200));
                newControllerName = EditorGUILayout.TextField(newControllerName);
                GUILayout.EndHorizontal();
                GUILayout.Space(4);

                spaceRemaining -= 20;
            }

            EditorGUILayout.HelpBox("Both the 'Xml File' and 'Xml Layout Controller' fields are optional.", MessageType.Info);

            if (xmlFileName == "Create New" || controllerName == "Create New")
            {
                EditorGUILayout.HelpBox("Please be advised that the file(s) will be created in the Assets/ folder and will need to be manually moved to the folder(s) of your choice.", MessageType.Info);
                spaceRemaining -= 32;
            }

            // Buttons
            GUILayout.Space(spaceRemaining);

            if (GUILayout.Button("Add Xml Layout"))
            {
                if (xmlFileName == "Create New")
                {
                    // Create the new xml file
                    xmlFileName = XmlLayoutMenuItems.NewXmlLayoutXmlFile(newXmlFileName, false).Substring("Assets/".Length);                                        
                    
                    AssetDatabase.Refresh();
                }

                bool newController = false;
                if (controllerName == "Create New")
                {
                    // Create the new controller file
                    XmlLayoutMenuItems.NewXmlLayoutController(newControllerName, false).Substring("Assets/".Length);
                    
                    newController = true;
                }

                var xmlLayout = InstantiatePrefab("XmlLayout Prefabs/XmlLayout").GetComponent<XmlLayout>();

                if (xmlFileName == "None")
                {
                    // expand the xml editor by default
                    xmlLayout.editor_showXml = true;
                }

                xmlLayout.name = "XmlLayout";

                Selection.activeGameObject = xmlLayout.gameObject;

                if (xmlFileName != "None")
                {
                    var xmlFile = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + xmlFileName);
                    xmlLayout.XmlFile = xmlFile;                    

                    xmlLayout.ReloadXmlFile();
                }

                if (controllerName != "None")
                {                    
                    if(!newController)
                    {
                        var controllerType = XmlLayoutUtilities.GetXmlLayoutControllerType(controllerName);

                        if (controllerType != null)
                        {
                            xmlLayout.gameObject.AddComponent(controllerType);
                        }                        
                    } 
                    else 
                    {
                        AssetDatabase.Refresh();

                        var window = EditorWindow.GetWindow<XmlLayoutSetControllerWindow>();
                        window.SetAction(xmlLayout.gameObject.GetInstanceID(), newControllerName);
                    }
                }                

                this.Close();
            }

            if (GUILayout.Button("Cancel")) this.Close();

            GUILayout.EndVertical();
        }

        // http://answers.unity3d.com/questions/486545/getting-all-assets-of-the-specified-type.html
        public static T[] GetAssetsOfType<T>(string fileExtension) where T : UnityEngine.Object
        {
            List<T> tempObjects = new List<T>();
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            FileInfo[] goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);

            int i = 0; int goFileInfoLength = goFileInfo.Length;
            FileInfo tempGoFileInfo; string tempFilePath;
            T tempGO;
            for (; i < goFileInfoLength; i++)
            {
                tempGoFileInfo = goFileInfo[i];
                if (tempGoFileInfo == null)
                    continue;

                tempFilePath = tempGoFileInfo.FullName;
                tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                tempGO = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(T)) as T;
                if (tempGO == null)
                {
                    continue;
                }
                else if (!(tempGO is T))
                {
                    continue;
                }

                tempObjects.Add(tempGO);
            }

            return tempObjects.ToArray();
        }

        public static void FixInstanceTransform(RectTransform baseTransform, RectTransform instanceTransform)
        {
            instanceTransform.localPosition = baseTransform.localPosition;
            instanceTransform.position = baseTransform.position;
            instanceTransform.rotation = baseTransform.rotation;
            instanceTransform.localScale = baseTransform.localScale;
            instanceTransform.anchoredPosition = baseTransform.anchoredPosition;
            instanceTransform.sizeDelta = baseTransform.sizeDelta;
        }

        public static GameObject InstantiatePrefab(string name, bool playMode = false, bool generateUndo = true)
        {
            var prefab = XmlLayoutUtilities.LoadResource<GameObject>(name);

            if (prefab == null)
            {
                throw new UnityException(String.Format("Could not find prefab '{0}'!", name));
            }

            Transform parent = null;

#if UNITY_EDITOR
            if (!playMode) parent = UnityEditor.Selection.activeTransform;
#endif
            var gameObject = GameObject.Instantiate(prefab) as GameObject;
            gameObject.name = name;

            if (parent == null || !(parent is RectTransform))
            {
                parent = GetCanvasTransform();
            }

            gameObject.transform.SetParent(parent);

            var transform = (RectTransform)gameObject.transform;
            var prefabTransform = (RectTransform)prefab.transform;

            FixInstanceTransform(prefabTransform, transform);

#if UNITY_EDITOR
            if (generateUndo)
            {
                UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "Created " + name);
            }
#endif

            return gameObject;
        }

        public static Transform GetCanvasTransform()
        {
            Canvas canvas = null;
#if UNITY_EDITOR
            // Attempt to locate a canvas object parented to the currently selected object
            if (!Application.isPlaying && UnityEditor.Selection.activeGameObject != null)
            {
                canvas = FindParentOfType<Canvas>(UnityEditor.Selection.activeGameObject);
                //canvas = UnityEditor.Selection.activeTransform.GetComponentInParent<Canvas>();                
            }
#endif

            if (canvas == null)
            {
                // Attempt to find a canvas anywhere
                canvas = UnityEngine.Object.FindObjectOfType<Canvas>();

                if (canvas != null) return canvas.transform;
            }

            // if we reach this point, we haven't been able to locate a canvas
            // ...So I guess we'd better create one

            GameObject canvasGameObject = new GameObject("Canvas");
            canvasGameObject.layer = LayerMask.NameToLayer("UI");
            canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGameObject.AddComponent<CanvasScaler>();
            canvasGameObject.AddComponent<GraphicRaycaster>();

#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(canvasGameObject, "Create Canvas");
#endif

            var eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();

            if (eventSystem == null)
            {
                GameObject eventSystemGameObject = new GameObject("EventSystem");
                eventSystem = eventSystemGameObject.AddComponent<EventSystem>();
                eventSystemGameObject.AddComponent<StandaloneInputModule>();

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                eventSystemGameObject.AddComponent<TouchInputModule>();
#endif

#if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo(eventSystemGameObject, "Create EventSystem");
#endif
            }

            return canvas.transform;
        }

        public static T FindParentOfType<T>(GameObject childObject)
            where T : UnityEngine.Object
        {
            Transform t = childObject.transform;
            while (t.parent != null)
            {
                var component = t.parent.GetComponent<T>();

                if (component != null) return component;

                t = t.parent.transform;
            }

            // We didn't find anything
            return null;
        }
    }

    public class XmlLayoutSetControllerWindow : EditorWindow
    {
        int m_xmlLayoutInstanceId;
        string m_xmlLayoutControllerName;
        bool m_waitingForCompilation = false;

        public void SetAction(int xmlLayoutInstanceId, string xmlLayoutControllerName)
        {
            m_xmlLayoutInstanceId = xmlLayoutInstanceId;
            m_xmlLayoutControllerName = xmlLayoutControllerName;

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
                var gameObject = (GameObject)EditorUtility.InstanceIDToObject(m_xmlLayoutInstanceId);
                var type = XmlLayoutUtilities.GetXmlLayoutControllerType(m_xmlLayoutControllerName);

                gameObject.AddComponent(type);

                m_waitingForCompilation = false;
                this.Close();
            }            
        }        
    }
}
