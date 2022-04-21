// -----------------------------------------------------------------------
// <copyright file="PhotonAppSettings.cs" company="Exit Games GmbH">
// </copyright>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_2017_4_OR_NEWER
#define SUPPORTED_UNITY
#endif


#if !PHOTON_UNITY_NETWORKING

namespace Photon.Realtime
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    /// <summary>
    /// Collection of connection-relevant settings, used internally by PhotonNetwork.ConnectUsingSettings.
    /// </summary>
    /// <remarks>
    /// Includes the AppSettings class from the Realtime APIs plus some other, PUN-relevant, settings.</remarks>
    [Serializable]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/getting-started/initial-setup")]
    public class PhotonAppSettings : ScriptableObject
    {
        [Tooltip("Core Photon Server/Cloud settings.")]
        public AppSettings AppSettings;

        #if UNITY_EDITOR
        [HideInInspector]
        public bool DisableAutoOpenWizard;
        //public bool ShowSettings;
        //public bool DevRegionSetOnce;
        #endif

        private static PhotonAppSettings instance;

        /// <summary>Serialized server settings, written by the Setup Wizard for use in ConnectUsingSettings.</summary>
        public static PhotonAppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    LoadOrCreateSettings();
                }

                return instance;
            }

            private set { instance = value; }
        }



        public static void LoadOrCreateSettings()
        {
            if (instance != null)
            {
                Debug.LogWarning("Instance is not null. Will not LoadOrCreateSettings().");
                return;
            }


            #if UNITY_EDITOR
            // let's check if the AssetDatabase finds the file; aimed to avoid multiple files being created, potentially a futile step
            AssetDatabase.Refresh();
            #endif

            // try to load the resource / asset (ServerSettings a.k.a. PhotonServerSettings)
            instance = (PhotonAppSettings)Resources.Load(typeof(PhotonAppSettings).Name, typeof(PhotonAppSettings));
            if (instance != null)
            {
                //Debug.LogWarning("Settings from Resources."); // DEBUG
                return;
            }


            // create it if not loaded
            if (instance == null)
            {
                instance = (PhotonAppSettings)CreateInstance(typeof(PhotonAppSettings));
                if (instance == null)
                {
                    Debug.LogError("Failed to create ServerSettings. PUN is unable to run this way. If you deleted it from the project, reload the Editor.");
                    return;
                }

                //Debug.LogWarning("Settings created!"); // DEBUG
            }

            // in the editor, store the settings file as it's not loaded
            #if UNITY_EDITOR
            string punResourcesDirectory = "Assets/Photon/Resources/";
            string serverSettingsAssetPath = punResourcesDirectory + typeof(PhotonAppSettings).Name + ".asset";
            string serverSettingsDirectory = Path.GetDirectoryName(serverSettingsAssetPath);

            if (!Directory.Exists(serverSettingsDirectory))
            {
                Directory.CreateDirectory(serverSettingsDirectory);
                AssetDatabase.ImportAsset(serverSettingsDirectory);
            }

            AssetDatabase.CreateAsset(instance, serverSettingsAssetPath);
            AssetDatabase.SaveAssets();


            //Debug.Log("Settings stored to DB."); // DEBUG
            #endif
        }
    }
}
#endif