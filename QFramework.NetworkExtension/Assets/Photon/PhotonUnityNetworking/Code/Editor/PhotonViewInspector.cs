// ----------------------------------------------------------------------------
// <copyright file="PhotonViewInspector.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Custom inspector for the PhotonView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

using Photon.Realtime;

namespace Photon.Pun
{
    [CustomEditor(typeof(PhotonView))]
    [CanEditMultipleObjects]
    internal class PhotonViewInspector : Editor
    {
        private PhotonView m_Target;

        private static GUIContent ownerTransferGuiContent = new GUIContent("Ownership Transfer", "Determines how ownership changes may be initiated.");
        private static GUIContent syncronizationGuiContent = new GUIContent("Synchronization", "Determines how sync updates are culled and sent.");
        private static GUIContent observableSearchGuiContent = new GUIContent("Observable Search", "When set to Auto, On Awake, Observables on this GameObject (and child GameObjects) will be found and populate the Observables List." +
                "\n\nNested PhotonViews (children with a PhotonView) and their children will not be included in the search.");

        public void OnEnable()
        {
            this.m_Target = (PhotonView)this.target;

            if (!Application.isPlaying)
                m_Target.FindObservables();
        }
        public override void OnInspectorGUI()
        {


            this.m_Target = (PhotonView)this.target;
            bool isProjectPrefab = PhotonEditorUtils.IsPrefab(this.m_Target.gameObject);
            bool multiSelected = Selection.gameObjects.Length > 1;

            if (this.m_Target.ObservedComponents == null)
            {
                this.m_Target.ObservedComponents = new System.Collections.Generic.List<Component>();
            }

            if (this.m_Target.ObservedComponents.Count == 0)
            {
                this.m_Target.ObservedComponents.Add(null);
            }

            GUILayout.Space(5);

            EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");
            // View ID - Hide if we are multi-selected
            if (!multiSelected)
            {
                if (isProjectPrefab)
                {
                    EditorGUILayout.LabelField("View ID", "<i>Set at runtime</i>", new GUIStyle("Label") { richText = true });
                }
                else if (EditorApplication.isPlaying)
                {
                    EditorGUILayout.LabelField("View ID", this.m_Target.ViewID.ToString());
                }
                else
                {
                    // this is an object in a scene, modified at edit-time. we can store this as sceneViewId
                    int idValue = EditorGUILayout.IntField("View ID [1.." + (PhotonNetwork.MAX_VIEW_IDS - 1) + "]", this.m_Target.sceneViewId);
                    if (this.m_Target.sceneViewId != idValue)
                    {
                        Undo.RecordObject(this.m_Target, "Change PhotonView viewID");
                        this.m_Target.sceneViewId = idValue;
                    }
                }
            }

            // Locally Controlled
            if (EditorApplication.isPlaying)
            {
                string masterClientHint = PhotonNetwork.IsMasterClient ? " (master)" : "";
                EditorGUILayout.LabelField("IsMine:", this.m_Target.IsMine.ToString() + masterClientHint);
                Room room = PhotonNetwork.CurrentRoom;
                int cretrId = this.m_Target.CreatorActorNr;
                Player cretr = (room != null) ? room.GetPlayer(cretrId) : null;
                Player owner = this.m_Target.Owner;
                Player ctrlr = this.m_Target.Controller;
                EditorGUILayout.LabelField("Controller:", (ctrlr != null ? ("[" + ctrlr.ActorNumber + "] '" + ctrlr.NickName + "' " + (ctrlr.IsMasterClient ? " (master)" : "")) : "[0] <null>"));
                EditorGUILayout.LabelField("Owner:", (owner != null ? ("[" + owner.ActorNumber + "] '" + owner.NickName + "' " + (owner.IsMasterClient ? " (master)" : "")) : "[0] <null>"));
                EditorGUILayout.LabelField("Creator:", (cretr != null ? ("[" +cretrId + "] '" + cretr.NickName + "' " + (cretr.IsMasterClient ? " (master)" : "")) : "[0] <null>"));

            }

            EditorGUILayout.EndVertical();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            GUILayout.Space(5);

            // Ownership section

            EditorGUILayout.LabelField("Ownership", (GUIStyle)"BoldLabel");

            OwnershipOption own = (OwnershipOption)EditorGUILayout.EnumPopup(ownerTransferGuiContent, this.m_Target.OwnershipTransfer/*, GUILayout.MaxWidth(68), GUILayout.MinWidth(68)*/);
            if (own != this.m_Target.OwnershipTransfer)
            {
                // jf: fixed 5 and up prefab not accepting changes if you quit Unity straight after change.
                // not touching the define nor the rest of the code to avoid bringing more problem than solving.
                EditorUtility.SetDirty(this.m_Target);

                Undo.RecordObject(this.m_Target, "Change PhotonView Ownership Transfer");
                this.m_Target.OwnershipTransfer = own;
            }

            
            GUILayout.Space(5);

            // Observables section

            EditorGUILayout.LabelField("Observables", (GUIStyle)"BoldLabel");

            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Synchronization"), syncronizationGuiContent);

            if (this.m_Target.Synchronization == ViewSynchronization.Off)
            {
                // Show warning if there are any observables. The null check is because the list allows nulls.
                var observed = m_Target.ObservedComponents;
                if (observed.Count > 0)
                {
                    for (int i = 0, cnt = observed.Count; i < cnt; ++i)
                        if (observed[i] != null)
                        {
                            EditorGUILayout.HelpBox("Synchronization is set to Off. Select a Synchronization setting in order to sync the listed Observables.", MessageType.Warning);
                            break;
                        }
                }
             }


            PhotonView.ObservableSearch autoFindObservables = (PhotonView.ObservableSearch)EditorGUILayout.EnumPopup(observableSearchGuiContent, m_Target.observableSearch);

            if (m_Target.observableSearch != autoFindObservables)
            {
                Undo.RecordObject(this.m_Target, "Change Auto Find Observables Toggle");
                m_Target.observableSearch = autoFindObservables;
            }

            m_Target.FindObservables();

            if (!multiSelected)
            {
                bool disableList = Application.isPlaying || autoFindObservables != PhotonView.ObservableSearch.Manual;

                if (disableList)
                    EditorGUI.BeginDisabledGroup(true);

                this.DrawObservedComponentsList(disableList);

                if (disableList)
                    EditorGUI.EndDisabledGroup();
            }

            // Cleanup: save and fix look
            if (GUI.changed)
            {
                PhotonViewHandler.OnHierarchyChanged(); // TODO: check if needed
            }

            EditorGUI.EndDisabledGroup();
        }



        private int GetObservedComponentsCount()
        {
            int count = 0;

            for (int i = 0; i < this.m_Target.ObservedComponents.Count; ++i)
            {
                if (this.m_Target.ObservedComponents[i] != null)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Find Observables, and then baking them into the serialized object.
        /// </summary>
        private void EditorFindObservables()
        {
            Undo.RecordObject(serializedObject.targetObject, "Find Observables");
            var property = serializedObject.FindProperty("ObservedComponents");
            
            // Just doing a Find updates the Observables list, but Unity fails to save that change.
            // Instead we do the find, and then iterate the found objects into the serialize property, then apply that.
            property.ClearArray();
            m_Target.FindObservables(true);
            for(int i = 0; i <  m_Target.ObservedComponents.Count; ++i)
            {
                property.InsertArrayElementAtIndex(i);
                property.GetArrayElementAtIndex(i).objectReferenceValue = m_Target.ObservedComponents[i];
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawObservedComponentsList(bool disabled = false)
        {
            SerializedProperty listProperty = this.serializedObject.FindProperty("ObservedComponents");

            if (listProperty == null)
            {
                return;
            }

            float containerElementHeight = 22;
            float containerHeight = listProperty.arraySize * containerElementHeight;

            string foldoutLabel = "Observed Components (" + this.GetObservedComponentsCount() + ")";
            bool isOpen = PhotonGUI.ContainerHeaderFoldout(foldoutLabel, this.serializedObject.FindProperty("ObservedComponentsFoldoutOpen").boolValue, () => EditorFindObservables(), "Find");
            this.serializedObject.FindProperty("ObservedComponentsFoldoutOpen").boolValue = isOpen;

            if (isOpen == false)
            {
                containerHeight = 0;
            }

            //Texture2D statsIcon = AssetDatabase.LoadAssetAtPath( "Assets/Photon Unity Networking/Editor/PhotonNetwork/PhotonViewStats.png", typeof( Texture2D ) ) as Texture2D;

            Rect containerRect = PhotonGUI.ContainerBody(containerHeight);


            bool wasObservedComponentsEmpty = this.m_Target.ObservedComponents.FindAll(item => item != null).Count == 0;
            if (isOpen == true)
            {
                for (int i = 0; i < listProperty.arraySize; ++i)
                {
                    Rect elementRect = new Rect(containerRect.xMin, containerRect.yMin + containerElementHeight * i, containerRect.width, containerElementHeight);
                    {
                        Rect texturePosition = new Rect(elementRect.xMin + 6, elementRect.yMin + elementRect.height / 2f - 1, 9, 5);
                        ReorderableListResources.DrawTexture(texturePosition, ReorderableListResources.texGrabHandle);

                        Rect propertyPosition = new Rect(elementRect.xMin + 20, elementRect.yMin + 3, elementRect.width - 45, 16);

                        // keep track of old type to catch when a new type is observed
                        Type _oldType = listProperty.GetArrayElementAtIndex(i).objectReferenceValue != null ? listProperty.GetArrayElementAtIndex(i).objectReferenceValue.GetType() : null;

                        EditorGUI.PropertyField(propertyPosition, listProperty.GetArrayElementAtIndex(i), new GUIContent());

                        // new type, could be different from old type
                        Type _newType = listProperty.GetArrayElementAtIndex(i).objectReferenceValue != null ? listProperty.GetArrayElementAtIndex(i).objectReferenceValue.GetType() : null;

                        // the user dropped a Transform, we must change it by adding a PhotonTransformView and observe that instead
                        if (_oldType != _newType)
                        {
                            if (_newType == typeof(PhotonView))
                            {
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = null;
                                Debug.LogError("PhotonView Detected you dropped a PhotonView, this is not allowed. \n It's been removed from observed field.");

                            }
                            else if (_newType == typeof(Transform))
                            {

                                // try to get an existing PhotonTransformView ( we don't want any duplicates...)
                                PhotonTransformView _ptv = this.m_Target.gameObject.GetComponent<PhotonTransformView>();
                                if (_ptv == null)
                                {
                                    // no ptv yet, we create one and enable position and rotation, no scaling, as it's too rarely needed to take bandwidth for nothing
                                    _ptv = Undo.AddComponent<PhotonTransformView>(this.m_Target.gameObject);
                                }
                                // switch observe from transform to _ptv
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = _ptv;
                                Debug.Log("PhotonView has detected you dropped a Transform. Instead it's better to observe a PhotonTransformView for better control and performances");
                            }
                            else if (_newType == typeof(Rigidbody))
                            {

                                Rigidbody _rb = listProperty.GetArrayElementAtIndex(i).objectReferenceValue as Rigidbody;

                                // try to get an existing PhotonRigidbodyView ( we don't want any duplicates...)
                                PhotonRigidbodyView _prbv = _rb.gameObject.GetComponent<PhotonRigidbodyView>();
                                if (_prbv == null)
                                {
                                    // no _prbv yet, we create one
                                    _prbv = Undo.AddComponent<PhotonRigidbodyView>(_rb.gameObject);
                                }
                                // switch observe from transform to _prbv
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = _prbv;
                                Debug.Log("PhotonView has detected you dropped a RigidBody. Instead it's better to observe a PhotonRigidbodyView for better control and performances");
                            }
                            else if (_newType == typeof(Rigidbody2D))
                            {

                                // try to get an existing PhotonRigidbody2DView ( we don't want any duplicates...)
                                PhotonRigidbody2DView _prb2dv = this.m_Target.gameObject.GetComponent<PhotonRigidbody2DView>();
                                if (_prb2dv == null)
                                {
                                    // no _prb2dv yet, we create one
                                    _prb2dv = Undo.AddComponent<PhotonRigidbody2DView>(this.m_Target.gameObject);
                                }
                                // switch observe from transform to _prb2dv
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = _prb2dv;
                                Debug.Log("PhotonView has detected you dropped a Rigidbody2D. Instead it's better to observe a PhotonRigidbody2DView for better control and performances");
                            }
                            else if (_newType == typeof(Animator))
                            {

                                // try to get an existing PhotonAnimatorView ( we don't want any duplicates...)
                                PhotonAnimatorView _pav = this.m_Target.gameObject.GetComponent<PhotonAnimatorView>();
                                if (_pav == null)
                                {
                                    // no _pav yet, we create one
                                    _pav = Undo.AddComponent<PhotonAnimatorView>(this.m_Target.gameObject);
                                }
                                // switch observe from transform to _prb2dv
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = _pav;
                                Debug.Log("PhotonView has detected you dropped a Animator, so we switched to PhotonAnimatorView so that you can serialized the Animator variables");
                            }
                            else if (!typeof(IPunObservable).IsAssignableFrom(_newType))
                            {
                                bool _ignore = false;
#if PLAYMAKER
                                _ignore = _newType == typeof(PlayMakerFSM);// Photon Integration for PlayMaker will swap at runtime to a proxy using iPunObservable.
#endif

                                if (_newType == null || _newType == typeof(Rigidbody) || _newType == typeof(Rigidbody2D))
                                {
                                    _ignore = true;
                                }

                                if (!_ignore)
                                {
                                    listProperty.GetArrayElementAtIndex(i).objectReferenceValue = null;
                                    Debug.LogError("PhotonView Detected you dropped a Component missing IPunObservable Interface,\n You dropped a <" + _newType + "> instead. It's been removed from observed field.");
                                }
                            }
                        }

                        //Debug.Log( listProperty.GetArrayElementAtIndex( i ).objectReferenceValue.GetType() );
                        //Rect statsPosition = new Rect( propertyPosition.xMax + 7, propertyPosition.yMin, statsIcon.width, statsIcon.height );
                        //ReorderableListResources.DrawTexture( statsPosition, statsIcon );

                        Rect removeButtonRect = new Rect(elementRect.xMax - PhotonGUI.DefaultRemoveButtonStyle.fixedWidth,
                                                         elementRect.yMin + 2,
                                                         PhotonGUI.DefaultRemoveButtonStyle.fixedWidth,
                                                         PhotonGUI.DefaultRemoveButtonStyle.fixedHeight);

                        GUI.enabled = !disabled && listProperty.arraySize > 1;
                        if (GUI.Button(removeButtonRect, new GUIContent(ReorderableListResources.texRemoveButton), PhotonGUI.DefaultRemoveButtonStyle))
                        {
                            listProperty.DeleteArrayElementAtIndex(i);
                        }
                        GUI.enabled = !disabled;

                        if (i < listProperty.arraySize - 1)
                        {
                            texturePosition = new Rect(elementRect.xMin + 2, elementRect.yMax, elementRect.width - 4, 1);
                            PhotonGUI.DrawSplitter(texturePosition);
                        }
                    }
                }
            }

            if (PhotonGUI.AddButton())
            {
                listProperty.InsertArrayElementAtIndex(Mathf.Max(0, listProperty.arraySize - 1));
            }

            this.serializedObject.ApplyModifiedProperties();

            bool isObservedComponentsEmpty = this.m_Target.ObservedComponents.FindAll(item => item != null).Count == 0;

            if (wasObservedComponentsEmpty == true && isObservedComponentsEmpty == false && this.m_Target.Synchronization == ViewSynchronization.Off)
            {
                Undo.RecordObject(this.m_Target, "Change PhotonView");
                this.m_Target.Synchronization = ViewSynchronization.UnreliableOnChange;
                this.serializedObject.Update();
            }

            if (wasObservedComponentsEmpty == false && isObservedComponentsEmpty == true)
            {
                Undo.RecordObject(this.m_Target, "Change PhotonView");
                this.m_Target.Synchronization = ViewSynchronization.Off;
                this.serializedObject.Update();
            }
        }
    }
}