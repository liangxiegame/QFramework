using UnityEngine;
using UnityEngine.Internal;

namespace QFramework
{
    public class ViewController : MonoBehaviour
    {
        [HideInInspector] public string ScriptName;
        
        [HideInInspector]
        public string ScriptsFolder = "Assets/Scripts";

        [HideInInspector]
        public bool GeneratePrefab = false;
        
        
        [HideInInspector]
        public string PrefabFolder = "Assets/Prefabs";
    }
}