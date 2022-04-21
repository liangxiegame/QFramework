// ----------------------------------------------------------------------------
// <copyright file="PhotonViewHandler.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   This is a Editor script to initialize PhotonView components.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
	using System.Collections.Generic;
    using Realtime;
    using UnityEditor;
	using UnityEngine;
    using Debug = UnityEngine.Debug;


    [InitializeOnLoad]
	public class PhotonViewHandler : EditorWindow
	{
		static PhotonViewHandler()
		{
            // called once per change (per key-press in inspectors) and once after play-mode ends.
			#if (UNITY_2018 || UNITY_2018_1_OR_NEWER)
			EditorApplication.hierarchyChanged += OnHierarchyChanged;
			#else
			EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
			#endif
		}


		internal static void OnHierarchyChanged()
        {
            // set prefabs to viewID 0 if needed
            // organize resource PVs in a list per viewID

            // process the lists: if more than one photonView is in a list, we have to resolve the clash
            // check if only one view had the viewId earlier
            // apply a new viewID to the others

            // update the cached list of instances and their viewID


            //Debug.LogWarning("OnHierarchyChanged(). isPlaying: " + Application.isPlaying);
            if (Application.isPlaying)
            {
                return;
            }


            PhotonView[] photonViewResources = Resources.FindObjectsOfTypeAll<PhotonView>();
            List<PhotonView> photonViewInstances = new List<PhotonView>();
            Dictionary<int, List<PhotonView>> viewInstancesPerViewId = new Dictionary<int, List<PhotonView>>();
            List<PhotonView> photonViewsToReassign = new List<PhotonView>();

            foreach (PhotonView view in photonViewResources)
            {
                if (PhotonEditorUtils.IsPrefab(view.gameObject))
                {
                    // prefabs should use 0 as ViewID and sceneViewId
                    if (view.ViewID != 0 || view.sceneViewId != 0)
                    {
                        view.ViewID = 0;
                        view.sceneViewId = 0;
                        EditorUtility.SetDirty(view);
                    }

                    continue;   // skip prefabs in further processing
                }

                photonViewInstances.Add(view);


                // assign a new viewID if the viewId is lower than the minimum for this scene
                if (!IsViewIdOkForScene(view))
                {
                    photonViewsToReassign.Add(view);
                    continue;   // this view definitely gets cleaned up, so it does not count versus duplicates, checked below
                }


                // organize the viewInstances into lists per viewID, so we know duplicate usage
                if (!viewInstancesPerViewId.ContainsKey(view.sceneViewId))
                {
                    viewInstancesPerViewId[view.sceneViewId] = new List<PhotonView>();
                }
                viewInstancesPerViewId[view.sceneViewId].Add(view);
            }

            //Debug.Log("PreviousAssignments: "+PunSceneViews.Instance.Views.Count);

            foreach (List<PhotonView> list in viewInstancesPerViewId.Values)
            {
                if (list.Count <= 1)
                {
                    continue;   // skip lists with just one entry (the viewID is unique)
                }


                PhotonView previousAssignment = null;
                bool wasAssigned = PunSceneViews.Instance.Views.TryGetValue(list[0].sceneViewId, out previousAssignment);

                foreach (PhotonView view in list)
                {
                    if (wasAssigned && view.Equals(previousAssignment))
                    {
                        // previously, we cached the used viewID as assigned to the current view. we don't change this.
                        continue;
                    }

                    //Debug.LogWarning("View to reassign due to viewID: "+view, view.gameObject);
                    photonViewsToReassign.Add(view);
                }
            }

            int i;
            foreach (PhotonView view in photonViewsToReassign)
            {
                i = MinSceneViewId(view);
                while (viewInstancesPerViewId.ContainsKey(i))
                {
                    i++;
                }
                view.sceneViewId = i;
                viewInstancesPerViewId.Add(i, null);    // we don't need the lists anymore but we care about getting the viewIDs listed
                EditorUtility.SetDirty(view);
            }


            // update the "semi persistent" list of viewIDs and their PhotonViews
            PunSceneViews.Instance.Views.Clear();
            foreach (PhotonView view in photonViewInstances)
            {
                if (PunSceneViews.Instance.Views.ContainsKey(view.sceneViewId))
                {
                    Debug.LogError("ViewIDs should no longer have duplicates! "+view.sceneViewId, view);  
                    continue;
                }

                PunSceneViews.Instance.Views[view.sceneViewId] = view;
            }

            //Debug.Log("photonViewsToReassign.Count: "+photonViewsToReassign.Count + " count of viewIDs in use: "+viewInstancesPerViewId.Values.Count);
            //Debug.Log("PreviousAssignments now counts: "+PunSceneViews.Instance.Views.Count);
        }


        private static int MinSceneViewId(PhotonView view)
        {
            int result = PunSceneSettings.MinViewIdForScene(view.gameObject.scene.name);
            return result;
        }

        private static bool IsViewIdOkForScene(PhotonView view)
        {
            return view.sceneViewId >= MinSceneViewId(view);
        }
	}

    /// <summary>
    /// Stores a PhotonView instances per viewId (key). Instance is used as cache storage in-Editor.
    /// </summary>
    public class PunSceneViews : ScriptableObject
    {
        [SerializeField]
        public Dictionary<int, PhotonView> Views = new Dictionary<int, PhotonView>();

        private static PunSceneViews instanceField;
        public static PunSceneViews Instance
        {
            get
            {
                if (instanceField != null)
                {
                    return instanceField;
                }

                instanceField = GameObject.FindObjectOfType<PunSceneViews>();
                if (instanceField == null)
                {
                    instanceField = ScriptableObject.CreateInstance<PunSceneViews>();
                }

                return instanceField;
            }
        }
    }
}