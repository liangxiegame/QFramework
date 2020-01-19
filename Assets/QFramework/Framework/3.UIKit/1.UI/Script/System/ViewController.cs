using UnityEngine;

namespace QFramework
{
    public class ViewController : MonoBehaviour
    {
        [HideInInspector] public string ScriptName;
        
        [HideInInspector]
        public string ScriptsFolder = string.Empty;

        [HideInInspector]
        public bool GeneratePrefab = false;
        
        
        [HideInInspector]
        public string PrefabFolder = string.Empty;
    }
}