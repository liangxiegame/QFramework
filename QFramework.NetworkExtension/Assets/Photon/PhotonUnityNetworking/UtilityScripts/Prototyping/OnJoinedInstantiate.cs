// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnJoinedInstantiate.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  This component will instantiate a network GameObject when a room is joined
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using Photon.Realtime;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Photon.Pun.UtilityScripts
{

    /// <summary>
    /// This component will instantiate a network GameObject when a room is joined
    /// </summary>
    public class OnJoinedInstantiate : MonoBehaviour
        , IMatchmakingCallbacks
    {
        public enum SpawnSequence { Connection, Random, RoundRobin }

        #region Inspector Items

        // Old field, only here for backwards compat. Value copies over to SpawnPoints in OnValidate
        [HideInInspector] private Transform SpawnPosition;

        [HideInInspector] public SpawnSequence Sequence = SpawnSequence.Connection;

        [HideInInspector] public List<Transform> SpawnPoints = new List<Transform>(1) { null };

        [Tooltip("Add a random variance to a spawn point position. GetRandomOffset() can be overridden with your own method for producing offsets.")]
        [HideInInspector] public bool UseRandomOffset = true;

        [Tooltip("Radius of the RandomOffset.")]
        [FormerlySerializedAs("PositionOffset")]
        [HideInInspector] public float RandomOffset = 2.0f;

        [Tooltip("Disables the Y axis of RandomOffset. The Y value of the spawn point will be used.")]
        [HideInInspector] public bool ClampY = true;

        [HideInInspector] public List<GameObject> PrefabsToInstantiate = new List<GameObject>(1) { null }; // set in inspector

        [FormerlySerializedAs("autoSpawnObjects")]
        [HideInInspector] public bool AutoSpawnObjects = true;

        #endregion

        // Record of spawned objects, used for Despawn All
        public Stack<GameObject> SpawnedObjects = new Stack<GameObject>();
        protected int spawnedAsActorId;



#if UNITY_EDITOR

        protected void OnValidate()
        {
            /// Check the prefab to make sure it is the actual resource, and not a scene object or other instance.
            if (PrefabsToInstantiate != null)
                for (int i = 0; i < PrefabsToInstantiate.Count; ++i)
                {
                    var prefab = PrefabsToInstantiate[i];
                    if (prefab)
                        PrefabsToInstantiate[i] = ValidatePrefab(prefab);
                }

            /// Move any values from old SpawnPosition field to new SpawnPoints
            if (SpawnPosition)
            {
                if (SpawnPoints == null)
                    SpawnPoints = new List<Transform>();

                SpawnPoints.Add(SpawnPosition);
                SpawnPosition = null;
            }
        }

        /// <summary>
        /// Validate, and if valid add this prefab to the first null element of the list, or create a new element. Returns true if the object was added.
        /// </summary>
        /// <param name="prefab"></param>
        public bool AddPrefabToList(GameObject prefab)
        {
            var validated = ValidatePrefab(prefab);
            if (validated)
            {
                // Don't add to list if this prefab already is on the list
                if (PrefabsToInstantiate.Contains(validated))
                    return false;

                // First try to use any null array slots to keep things tidy
                if (PrefabsToInstantiate.Contains(null))
                    PrefabsToInstantiate[PrefabsToInstantiate.IndexOf(null)] = validated;
                // Otherwise, just add this prefab.
                else
                    PrefabsToInstantiate.Add(validated);
                return true;
            }

            return false;

        }

        /// <summary>
        /// Determines if the supplied GameObject is an instance of a prefab, or the actual source Asset, 
        /// and returns a best guess at the actual resource the dev intended to use.
        /// </summary>
        /// <returns></returns>
        protected static GameObject ValidatePrefab(GameObject unvalidated)
        {
            if (unvalidated == null)
                return null;

            if (!unvalidated.GetComponent<PhotonView>())
                return null;

#if UNITY_2018_3_OR_NEWER

			GameObject validated = null;

			if (unvalidated != null)
			{

				if (PrefabUtility.IsPartOfPrefabAsset(unvalidated))
					return unvalidated;

				var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(unvalidated);
				var isValidPrefab = prefabStatus == PrefabInstanceStatus.Connected || prefabStatus == PrefabInstanceStatus.Disconnected;

				if (isValidPrefab)
					validated = PrefabUtility.GetCorrespondingObjectFromSource(unvalidated) as GameObject;
				else
					return null;

				if (!PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(validated).Contains("/Resources"))
					Debug.LogWarning("Player Prefab needs to be a Prefab in a Resource folder.");
			}
#else
            GameObject validated = unvalidated;

            if (unvalidated != null && PrefabUtility.GetPrefabType(unvalidated) != PrefabType.Prefab)
                validated = PrefabUtility.GetPrefabParent(unvalidated) as GameObject;
#endif
            return validated;
        }

#endif


        public virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }


        public virtual void OnJoinedRoom()
        {
            // Only AutoSpawn if we are a new ActorId. Rejoining should reproduce the objects by server instantiation.
            if (AutoSpawnObjects && !PhotonNetwork.LocalPlayer.HasRejoined)
            {
                SpawnObjects();
            }
        }

        public virtual void SpawnObjects()
        {
            if (this.PrefabsToInstantiate != null)
            {
                foreach (GameObject o in this.PrefabsToInstantiate)
                {
                    if (o == null)
                        continue;
#if UNITY_EDITOR
                    Debug.Log("Auto-Instantiating: " + o.name);
#endif
                    Vector3 spawnPos; Quaternion spawnRot;
                    GetSpawnPoint(out spawnPos, out spawnRot);


                    var newobj = PhotonNetwork.Instantiate(o.name, spawnPos, spawnRot, 0);
                    SpawnedObjects.Push(newobj);
                }
            }
        }

        /// <summary>
        /// Destroy all objects that have been spawned by this component for this client.
        /// </summary>
        /// <param name="localOnly">Use Object.Destroy rather than PhotonNetwork.Destroy.</param>
        public virtual void DespawnObjects(bool localOnly)
        {

            while (SpawnedObjects.Count > 0)
            {
                var go = SpawnedObjects.Pop();
                if (go)
                {
                    if (localOnly)
                        Object.Destroy(go);
                    else
                        PhotonNetwork.Destroy(go);

                }
            }
        }

        public virtual void OnFriendListUpdate(List<FriendInfo> friendList) { }
        public virtual void OnCreatedRoom() { }
        public virtual void OnCreateRoomFailed(short returnCode, string message) { }
        public virtual void OnJoinRoomFailed(short returnCode, string message) { }
        public virtual void OnJoinRandomFailed(short returnCode, string message) { }
        public virtual void OnLeftRoom() { }

        protected int lastUsedSpawnPointIndex = -1;

        /// <summary>
        /// Gets the next SpawnPoint from the list using the SpawnSequence, and applies RandomOffset (if used) to the transform matrix.
        /// Override this method with any custom code for coming up with a spawn location. This method is used by AutoSpawn.
        /// </summary>
        public virtual void GetSpawnPoint(out Vector3 spawnPos, out Quaternion spawnRot)
        {

            // Fetch a point using the Sequence method indicated
            Transform point = GetSpawnPoint();

            if (point != null)
            {
                spawnPos = point.position;
                spawnRot = point.rotation;
            }
            else
            {
                spawnPos = new Vector3(0, 0, 0);
                spawnRot = new Quaternion(0, 0, 0, 1);
            }
            
            if (UseRandomOffset)
            {
                Random.InitState((int)(Time.time * 10000));
                spawnPos += GetRandomOffset();
            }
        }
        

        /// <summary>
        /// Get the transform of the next SpawnPoint from the list, selected using the SpawnSequence setting. 
        /// RandomOffset is not applied, only the transform of the SpawnPoint is returned.
        /// Override this method to change how Spawn Point transform is selected. Return the transform you want to use as a spawn point.
        /// </summary>
        /// <returns></returns>
        protected virtual Transform GetSpawnPoint()
        {
            // Fetch a point using the Sequence method indicated
            if (SpawnPoints == null || SpawnPoints.Count == 0)
            {
                return null;
            }
            else
            {
                switch (Sequence)
                {
                    case SpawnSequence.Connection:
                        {
                            int id = PhotonNetwork.LocalPlayer.ActorNumber;
                            return SpawnPoints[(id == -1) ? 0 : id % SpawnPoints.Count];
                        }

                    case SpawnSequence.RoundRobin:
                        {
                            lastUsedSpawnPointIndex++;
                            if (lastUsedSpawnPointIndex >= SpawnPoints.Count)
                                lastUsedSpawnPointIndex = 0;

                            /// Use Vector.Zero and Quaternion.Identity if we are dealing with no or a null spawnpoint.
                            return SpawnPoints == null || SpawnPoints.Count == 0 ? null : SpawnPoints[lastUsedSpawnPointIndex];
                        }

                    case SpawnSequence.Random:
                        {
                            return SpawnPoints[Random.Range(0, SpawnPoints.Count)];
                        }

                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// When UseRandomeOffset is enabled, this method is called to produce a Vector3 offset. The default implementation clamps the Y value to zero. You may override this with your own implementation.
        /// </summary>
        protected virtual Vector3 GetRandomOffset()
        {
            Vector3 random = Random.insideUnitSphere;
            if (ClampY)
                random.y = 0;
            return RandomOffset * random.normalized;
        }

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(OnJoinedInstantiate), true)]
    [CanEditMultipleObjects]
    public class OnJoinedInstantiateEditor : Editor
    {

        SerializedProperty SpawnPoints, PrefabsToInstantiate, UseRandomOffset, ClampY, RandomOffset, Sequence, autoSpawnObjects;
        GUIStyle fieldBox;

        private void OnEnable()
        {
            SpawnPoints = serializedObject.FindProperty("SpawnPoints");
            PrefabsToInstantiate = serializedObject.FindProperty("PrefabsToInstantiate");
            UseRandomOffset = serializedObject.FindProperty("UseRandomOffset");
            ClampY = serializedObject.FindProperty("ClampY");
            RandomOffset = serializedObject.FindProperty("RandomOffset");
            Sequence = serializedObject.FindProperty("Sequence");

            autoSpawnObjects = serializedObject.FindProperty("AutoSpawnObjects");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            const int PAD = 6;

            if (fieldBox == null)
                fieldBox = new GUIStyle("HelpBox") { padding = new RectOffset(PAD, PAD, PAD, PAD) };

            EditorGUI.BeginChangeCheck();

            EditableReferenceList(PrefabsToInstantiate, new GUIContent(PrefabsToInstantiate.displayName, PrefabsToInstantiate.tooltip), fieldBox);

            EditableReferenceList(SpawnPoints, new GUIContent(SpawnPoints.displayName, SpawnPoints.tooltip), fieldBox);

            /// Spawn Pattern
            EditorGUILayout.BeginVertical(fieldBox);
            EditorGUILayout.PropertyField(Sequence);
            EditorGUILayout.PropertyField(UseRandomOffset);
            if (UseRandomOffset.boolValue)
            {
                EditorGUILayout.PropertyField(RandomOffset);
                EditorGUILayout.PropertyField(ClampY);
            }
            EditorGUILayout.EndVertical();

            /// Auto/Manual Spawn
            EditorGUILayout.BeginVertical(fieldBox);
            EditorGUILayout.PropertyField(autoSpawnObjects);
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Create a basic rendered list of objects from a SerializedProperty list or array, with Add/Destroy buttons.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="gc"></param>
        public void EditableReferenceList(SerializedProperty list, GUIContent gc, GUIStyle style = null)
        {
            EditorGUILayout.LabelField(gc);

            if (style == null)
                style = new GUIStyle("HelpBox") { padding = new RectOffset(6, 6, 6, 6) };

            EditorGUILayout.BeginVertical(style);

            int count = list.arraySize;

            if (count == 0)
            {
                if (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.MaxWidth(20)), "+", (GUIStyle)"minibutton"))
                {
                    int newindex = list.arraySize;
                    list.InsertArrayElementAtIndex(0);
                    list.GetArrayElementAtIndex(0).objectReferenceValue = null;
                }
            }
            else
            {
                // List Elements and Delete buttons
                for (int i = 0; i < count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    bool add = (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.MaxWidth(20)), "+", (GUIStyle)"minibutton"));
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
                    bool remove = (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.MaxWidth(20)), "x", (GUIStyle)"minibutton"));

                    EditorGUILayout.EndHorizontal();

                    if (add)
                    {
                        Add(list, i);
                        break;
                    }

                    if (remove)
                    {
                        list.DeleteArrayElementAtIndex(i);
                        //EditorGUILayout.EndHorizontal();
                        break;
                    }
                }

                EditorGUILayout.GetControlRect(false, 4);
                
                if (GUI.Button(EditorGUILayout.GetControlRect(), "Add", (GUIStyle)"minibutton"))
                    Add(list, count);

            }
               

            EditorGUILayout.EndVertical();
        }

        private void Add(SerializedProperty list, int i)
        {
            {
                int newindex = list.arraySize;
                list.InsertArrayElementAtIndex(i);
                list.GetArrayElementAtIndex(i).objectReferenceValue = null;
            }
        }
    }
   

#endif
}
