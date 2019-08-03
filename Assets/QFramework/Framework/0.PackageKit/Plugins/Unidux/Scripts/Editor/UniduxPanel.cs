using Unidux.Util;
using UnityEditor;
using UnityEngine;

namespace Unidux.Experimental.Editor
{
    public class UniduxPanel : EditorWindow
    {
        [MenuItem("Window/UniduxPanel")]
        static void Init()
        {
            var window = GetWindow<UniduxPanel>();
            window.Show();
        }

        private ISerializeFactory Serializer = UniduxSetting.Serializer;
        private GameObject _storeObject = null;
        private IStoreAccessor _store = null;
        private int _toolBarPosition = 0;
        private const string StoreObjectKey = "UniduxPanel.StoreObject";
        private UniduxPanelSettingTab _settingTab = new UniduxPanelSettingTab();
        private UniduxPanelStateTab _stateTab = new UniduxPanelStateTab();

        void OnGUI()
        {
            this.titleContent.text = "UniduxPanel";
            EditorGUILayout.LabelField("UniduxEditorWindow", EditorStyles.boldLabel);

            // XXX: ObjectField cannot restrict type selection of IStoreAccessor
            var selectObject =
                EditorGUILayout.ObjectField(
                    "IStoreAccessor",
                    this._storeObject,
                    typeof(GameObject),
                    true
                ) as GameObject;

            // Initial case, but it excepts the case of setting none object.
            if (selectObject == null && this._storeObject == null)
            {
                selectObject = this.LoadObject(StoreObjectKey);
            }

            this._store = selectObject != null ? selectObject.GetComponent<IStoreAccessor>() : null;
            if (this._store != null && this._storeObject != selectObject)
            {
                this.SaveObject(selectObject, StoreObjectKey);
                this._storeObject = selectObject;
            }
            else if (this._store == null && this._storeObject != null)
            {
                this._storeObject = null;
                this.ResetObject(StoreObjectKey);
            }

            // Show toolbar
            this._toolBarPosition = GUILayout.Toolbar(this._toolBarPosition, new[] {"save/load", "state"});

            switch (this._toolBarPosition)
            {
                case 0:
                    this._settingTab.Render(this._store, this.Serializer);
                    break;
                case 1:
                    this._stateTab.Render(this._store);
                    break;
            }
        }

        private void ResetObject(string key)
        {
            EditorPrefs.DeleteKey(key);
        }

        private int SaveObject(GameObject obj, string key)
        {
            if (obj != null)
            {
                int id = obj.GetInstanceID();
                EditorPrefs.SetInt(key, id);
                return id;
            }
            return -1;
        }

        private GameObject LoadObject(string key)
        {
            int id = EditorPrefs.GetInt(key, -1);
            if (id != -1)
            {
                return EditorUtility.InstanceIDToObject(id) as GameObject;
            }
            return null;
        }
    }
}