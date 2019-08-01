using UnityEngine;

namespace Unidux.Experimental.Editor
{
    public class StateJsonFileWrapper : ScriptableObject
    {
        [System.NonSerialized] public string FileName; // path is relative to Assets/
    }
}