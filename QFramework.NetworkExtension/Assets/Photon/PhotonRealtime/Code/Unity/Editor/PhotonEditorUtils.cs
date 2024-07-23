// ----------------------------------------------------------------------------
// <copyright file="PhotonEditorUtils.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Unity Editor Utils
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 618 // Deprecation warnings


#if UNITY_2017_4_OR_NEWER
#define SUPPORTED_UNITY
#endif


#if UNITY_EDITOR

namespace Photon.Realtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEditor;
    using UnityEngine;

    using System.IO;
    using System.Text;
    using UnityEngine.Networking;


    [InitializeOnLoad]
    public static class PhotonEditorUtils
    {
        /// <summary>Stores a flag which tells Editor scripts if the PhotonEditor.OnProjectChanged got called since initialization.</summary>
        /// <remarks>If not, the AssetDatabase is likely not usable yet and instances of ScriptableObject can't be loaded.</remarks>
        public static bool ProjectChangedWasCalled;


        /// <summary>True if the ChatClient of the Photon Chat API is available. If so, the editor may (e.g.) show additional options in settings.</summary>
        public static bool HasChat;

        /// <summary>True if the VoiceClient of the Photon Voice API is available. If so, the editor may (e.g.) show additional options in settings.</summary>
        public static bool HasVoice;

        /// <summary>True if PUN is in the project.</summary>
        public static bool HasPun;

        /// <summary>True if Photon Fusion is available in the project (and enabled).</summary>
        public static bool HasFusion;

        /// <summary>True if the PhotonEditorUtils checked the available products / APIs. If so, the editor may (e.g.) show additional options in settings.</summary>
        public static bool HasCheckedProducts;

        static PhotonEditorUtils()
        {
            HasVoice = Type.GetType("Photon.Voice.VoiceClient, Assembly-CSharp") != null || Type.GetType("Photon.Voice.VoiceClient, Assembly-CSharp-firstpass") != null || Type.GetType("Photon.Voice.VoiceClient, PhotonVoice.API") != null;
            HasChat = Type.GetType("Photon.Chat.ChatClient, Assembly-CSharp") != null || Type.GetType("Photon.Chat.ChatClient, Assembly-CSharp-firstpass") != null || Type.GetType("Photon.Chat.ChatClient, PhotonChat") != null;
            HasPun = Type.GetType("Photon.Pun.PhotonNetwork, Assembly-CSharp") != null || Type.GetType("Photon.Pun.PhotonNetwork, Assembly-CSharp-firstpass") != null || Type.GetType("Photon.Pun.PhotonNetwork, PhotonUnityNetworking") != null;
            #if FUSION_WEAVER
            HasFusion = true;
            #endif
            PhotonEditorUtils.HasCheckedProducts = true;

            if (EditorPrefs.HasKey("DisablePun") && EditorPrefs.GetBool("DisablePun"))
            {
                HasPun = false;
            }

            if (HasPun)
            {
                // MOUNTING SYMBOLS
                #if !PHOTON_UNITY_NETWORKING
                AddScriptingDefineSymbolToAllBuildTargetGroups("PHOTON_UNITY_NETWORKING");
                #endif

                #if !PUN_2_0_OR_NEWER
                AddScriptingDefineSymbolToAllBuildTargetGroups("PUN_2_0_OR_NEWER");
                #endif

                #if !PUN_2_OR_NEWER
                AddScriptingDefineSymbolToAllBuildTargetGroups("PUN_2_OR_NEWER");
                #endif

                #if !PUN_2_19_OR_NEWER
                AddScriptingDefineSymbolToAllBuildTargetGroups("PUN_2_19_OR_NEWER");
                #endif
            }
        }

        /// <summary>
        /// Adds a given scripting define symbol to all build target groups
        /// You can see all scripting define symbols ( not the internal ones, only the one for this project), in the PlayerSettings inspector
        /// </summary>
        /// <param name="defineSymbol">Define symbol.</param>
        public static void AddScriptingDefineSymbolToAllBuildTargetGroups(string defineSymbol)
        {
            foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim()).ToList();

                if (!defineSymbols.Contains(defineSymbol))
                {
                    defineSymbols.Add(defineSymbol);

                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defineSymbols.ToArray()));
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Could not set Photon " + defineSymbol + " defines for build target: " + target + " group: " + group + " " + e);
                    }
                }
            }
        }


        /// <summary>
        /// Removes PUN2's Script Define Symbols from project
        /// </summary>
        public static void CleanUpPunDefineSymbols()
        {
            foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
                    .Split(';')
                    .Select(d => d.Trim())
                    .ToList();

                List<string> newDefineSymbols = new List<string>();
                foreach (var symbol in defineSymbols)
                {
                    if ("PHOTON_UNITY_NETWORKING".Equals(symbol) || symbol.StartsWith("PUN_2_"))
                    {
                        continue;
                    }

                    newDefineSymbols.Add(symbol);
                }

                try
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", newDefineSymbols.ToArray()));
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Could not set clean up PUN2's define symbols for build target: {0} group: {1}, {2}", target, group, e);
                }
            }
        }


        /// <summary>
        /// Gets the parent directory of a path. Recursive Function, will return null if parentName not found
        /// </summary>
        /// <returns>The parent directory</returns>
        /// <param name="path">Path.</param>
        /// <param name="parentName">Parent name.</param>
        public static string GetParent(string path, string parentName)
        {
            var dir = new DirectoryInfo(path);

            if (dir.Parent == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(parentName))
            {
                return dir.Parent.FullName;
            }

            if (dir.Parent.Name == parentName)
            {
                return dir.Parent.FullName;
            }

            return GetParent(dir.Parent.FullName, parentName);
        }

		/// <summary>
		/// Check if a GameObject is a prefab asset or part of a prefab asset, as opposed to an instance in the scene hierarchy
		/// </summary>
		/// <returns><c>true</c>, if a prefab asset or part of it, <c>false</c> otherwise.</returns>
		/// <param name="go">The GameObject to check</param>
		public static bool IsPrefab(GameObject go)
		{
            #if UNITY_2021_2_OR_NEWER
            return UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(go) != null || EditorUtility.IsPersistent(go);
            #elif UNITY_2018_3_OR_NEWER
            return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(go) != null || EditorUtility.IsPersistent(go);
            #else
            return EditorUtility.IsPersistent(go);
			#endif
		}

        //https://forum.unity.com/threads/using-unitywebrequest-in-editor-tools.397466/#post-4485181
        public static void StartCoroutine(System.Collections.IEnumerator update)
        {
            EditorApplication.CallbackFunction closureCallback = null;

            closureCallback = () =>
            {
                try
                {
                    if (update.MoveNext() == false)
                    {
                        EditorApplication.update -= closureCallback;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    EditorApplication.update -= closureCallback;
                }
            };

            EditorApplication.update += closureCallback;
        }

        public static System.Collections.IEnumerator HttpPost(string url, Dictionary<string, string> headers, byte[] payload, Action<string> successCallback, Action<string> errorCallback)
        {
            using (UnityWebRequest w = new UnityWebRequest(url, "POST"))
            {
                if (payload != null)
                {
                    w.uploadHandler = new UploadHandlerRaw(payload);
                }
                w.downloadHandler = new DownloadHandlerBuffer();
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        w.SetRequestHeader(header.Key, header.Value);
                    }
                }

                #if UNITY_2017_2_OR_NEWER
                yield return w.SendWebRequest();
                #else
                yield return w.Send();
                #endif

                while (w.isDone == false)
                    yield return null;

                #if UNITY_2020_2_OR_NEWER
                if (w.result == UnityWebRequest.Result.ProtocolError || w.result == UnityWebRequest.Result.ConnectionError || w.result == UnityWebRequest.Result.DataProcessingError)
                #elif UNITY_2017_1_OR_NEWER
                if (w.isNetworkError || w.isHttpError)
                #endif
                {
                    if (errorCallback != null)
                    {
                        errorCallback(w.error);
                    }
                }
                else
                {
                    if (successCallback != null)
                    {
                        successCallback(w.downloadHandler.text);
                    }
                }
            }
        }
        /// <summary>
        /// Creates a Foldout using a toggle with (GUIStyle)"Foldout") and a separate label. This is a workaround for 2019.3 foldout arrows not working.
        /// </summary>
        /// <param name="isExpanded"></param>
        /// <param name="label"></param>
        /// <returns>Returns the new isExpanded value.</returns>
        public static bool Foldout(this SerializedProperty isExpanded, GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect();
            bool newvalue = EditorGUI.Toggle(new Rect(rect) { xMin = rect.xMin + 2 }, GUIContent.none, isExpanded.boolValue, (GUIStyle)"Foldout");
            EditorGUI.LabelField(new Rect(rect) { xMin = rect.xMin + 15 }, label);
            if (newvalue != isExpanded.boolValue)
            {
                isExpanded.boolValue = newvalue;
                isExpanded.serializedObject.ApplyModifiedProperties();
            }
            return newvalue;
        }

        /// <summary>
        /// Creates a Foldout using a toggle with (GUIStyle)"Foldout") and a separate label. This is a workaround for 2019.3 foldout arrows not working.
        /// </summary>
        /// <param name="isExpanded"></param>
        /// <param name="label"></param>
        /// <returns>Returns the new isExpanded value.</returns>
        public static bool Foldout(this bool isExpanded, GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect();
            bool newvalue = EditorGUI.Toggle(new Rect(rect) { xMin = rect.xMin + 2 }, GUIContent.none, isExpanded, (GUIStyle)"Foldout");
            EditorGUI.LabelField(new Rect(rect) { xMin = rect.xMin + 15 }, label);
            return newvalue;
        }
    }


    public class CleanUpDefinesOnPunDelete : UnityEditor.AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions rao)
        {
            if ("Assets/Photon/PhotonUnityNetworking".Equals(assetPath))
            {
                PhotonEditorUtils.CleanUpPunDefineSymbols();
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
#endif