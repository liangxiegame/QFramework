/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace QFramework.Experimental
{
    //Display all GameEvents in the current scene - show which objects are using them
    //Include a description of what the Event does
    [ExecuteInEditMode]
    public class EventManagerWindow : EditorWindow
    {

        //Dictionary of all objects that are using the GameEventListener
        private Dictionary<SOGameEvent, List<GameObject>> gameEventDictionary = new Dictionary<SOGameEvent, List<GameObject>>();

        //List of all eventListenrs that isn't assigned
        private List<GameObject> unassignedEventListeners = new List<GameObject>();

        //All the objects with the gameEventListener
        private GameEventListener[] eventObjects;

        //Values for scrollPos
        private Vector2 scrollPos;

        //Toolbar variables
        private int toolbarInt = 0;
        private string[] toolbarStrings = { "Active Scene Events", "Unassigned Event Listeners" };

        [MenuItem("Window/EventManager")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one does not exist, make one
            EditorWindow.GetWindow(typeof(EventManagerWindow));
        }

        private void OnEnable()
        {
            //Register for both scene and hierarchy changes
            SceneManager.activeSceneChanged += SceneUpdated;
            EditorApplication.hierarchyChanged += HierarchyChanged;

            GetAllScriptableEventsInScene();
        }

        private void OnDisable()
        {
            //UnRegister for both scene and hierarchy changes
            EditorSceneManager.activeSceneChangedInEditMode -= SceneUpdated;
            SceneManager.activeSceneChanged -= SceneUpdated;

            EditorApplication.hierarchyChanged -= HierarchyChanged;
        }

        private void OnGUI()
        {          
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

            //Switch based on the current selected toolbar button
            switch(toolbarInt)
            {
                case 0:
                    DrawActiveSceneEvents();
                    break;
                case 1:
                    DrawUnassignedEvents();
                    break;
            }
        }

        /// <summary>
        /// Draw editor window showing all the active events being used in the scene
        /// </summary>
        private void DrawActiveSceneEvents()
        {
            Rect r = position; //position of the window - can get the height and width from here
            float windowWidth = position.width / 4; //Width of each section

            //Style for label - center position + bold
            GUIStyle centerLabelStyle = new GUIStyle();
            centerLabelStyle.alignment = TextAnchor.MiddleLeft;
            centerLabelStyle.fontStyle = FontStyle.Bold;

            //Style for label - center position + word wrap
            GUIStyle midLeftAlign = new GUIStyle();
            midLeftAlign.alignment = TextAnchor.MiddleLeft;
            midLeftAlign.wordWrap = true;

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Show all events being used in the current scene. \nWhile the scene is playing you can also raise any of the events.", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox); //All the Event names
            GUILayout.Label("Event Name", centerLabelStyle, GUILayout.Width(windowWidth));
            GUILayout.Label("Description", centerLabelStyle, GUILayout.Width(windowWidth));
            GUILayout.Label("GameObject", centerLabelStyle, GUILayout.Width(windowWidth));
            GUILayout.Label("Raise Event", centerLabelStyle, GUILayout.Width(windowWidth));

            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            //Print out a list of all the Events and each GameObject thats part of the event
            foreach (SOGameEvent key in gameEventDictionary.Keys)
            {
                //***********************Event Name***********************
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox); //All the Event names
                GUILayout.Label(key.name, midLeftAlign, GUILayout.Width(windowWidth));

                //***********************Event Description***********************
                EditorGUILayout.BeginVertical(GUILayout.MinWidth(windowWidth));
                GUI.skin.label.wordWrap = true; //Wrap the label text
                GUILayout.Label(key.eventDescription, midLeftAlign, GUILayout.Width(windowWidth));
                EditorGUILayout.EndVertical();
                //***************************************************************

                //***********************Event GameObjects***********************
                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true)); //All objects that use the Event
                foreach (GameObject value in gameEventDictionary[key])
                {
                    GUILayout.Label(value.name, midLeftAlign, GUILayout.Width(windowWidth)); //Gameobject names
                    EditorGUILayout.ObjectField(value, typeof(object), true, GUILayout.Width(windowWidth - 10));
                }
                EditorGUILayout.EndVertical();
                //***************************************************************

                //***********************Event Raise***********************
                EditorGUILayout.BeginVertical(); //Button to trigger all the events 
                if (EditorApplication.isPlaying) //If the application is playing - draw the raise buttons
                {
                    if (GUILayout.Button("Raise Event"))
                    {
                        key.Raise(); //Raise the events
                    }
                }
                EditorGUILayout.EndVertical();
                //********************************************************

                EditorGUILayout.EndHorizontal(); //End of the Event names
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draw editor window showing all the unassigned GameEvent Listener components
        /// </summary>
        private void DrawUnassignedEvents()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Show all GameObjects that have the GameEvent Listener component, but haven't assigned a GameEvent to it.", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true)); //All objects that use the Event
            foreach (GameObject value in unassignedEventListeners)
            {
                GUILayout.Label(value.name); //Gameobject names
                EditorGUILayout.ObjectField(value, typeof(object), true);
            }
            EditorGUILayout.EndVertical();

            Repaint(); //Repaint incase the GameEventListener is assigned
        }

        /// <summary>
        /// If the scene is changed then call the GetAllScriptableEventsInScene
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        private void SceneUpdated(Scene current, Scene next)
        {
            GetAllScriptableEventsInScene();
        }

        /// <summary>
        /// If the Hierarchy has changed then call the GetAllScriptableEventsInScene
        /// </summary>
        private void HierarchyChanged()
        {
            GetAllScriptableEventsInScene();
        }

        /// <summary>
        /// Get all the scriptable events being used in the scene and store them in a dictionary
        /// </summary>
        private void GetAllScriptableEventsInScene()
        {
            //Clear the dictionary and list
            ClearCollections();

            //Get all the eventObjects in the scene
            eventObjects = FindObjectsOfType<GameEventListener>();

            foreach (GameEventListener listener in eventObjects)
            {
                if (listener.Event != null) //Check to make sure there is an event (there won't be when the component is first added)
                {
                    //Check to see if the key does not already exist in the dictionary
                    if (!gameEventDictionary.ContainsKey(listener.Event))
                    {
                        List<GameObject> eventObjectList = new List<GameObject>();

                        eventObjectList.Add(listener.gameObject);

                        gameEventDictionary.Add(listener.Event, eventObjectList); //Add the event and the gameObject list

                    }
                    else
                    {
                        List<GameObject> copyList = gameEventDictionary[listener.Event]; //Get a copy of the list associated with listener event
                        copyList.Add(listener.gameObject);

                        gameEventDictionary[listener.Event] = copyList;
                    }
                }
                else //Add to the unassignedObject list
                {
                    unassignedEventListeners.Add(listener.gameObject);
                }
            }
        }

        /// <summary>
        /// Clear both the EventDictionary and UnassignedEventListeners
        /// </summary>
        private void ClearCollections()
        {
            //Empty the Dictionary
            gameEventDictionary.Clear();

            //Empty the List
            unassignedEventListeners.Clear();
        }
    }
}
#endif