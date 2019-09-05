using UnityEditor;

namespace QF.GraphDesigner.Unity
{
    public class UnityPlatformPreferences : IPlatformPreferences
    {
        public bool GetBool(string name, bool def)
        {
            return EditorPrefs.GetBool(name, def);
        }

        public string GetString(string name, string def)
        {
            return EditorPrefs.GetString(name, def);
        }

        public float GetFloat(string name, float def)
        {
            return EditorPrefs.GetFloat(name, def);
        }

        public float GetInt(string name, int def)
        {

            return EditorPrefs.GetInt(name, def);
        }
        public void SetBool(string name, bool value)
        {
            EditorPrefs.SetBool(name, value);
        }

        public void SetString(string name, string value)
        {
            EditorPrefs.SetString(name, value);
        }

        public void SetFloat(string name, float value)
        {
            EditorPrefs.SetFloat(name, value);
        }

        public void SetInt(string name, int value)
        {

            EditorPrefs.SetInt(name, value);
        }

        public bool HasKey(string name)
        {
            return EditorPrefs.HasKey(name);
        }
        public void DeleteKey(string name)
        {
            EditorPrefs.DeleteKey(name);
        }
        public void DeleteAll()
        {
            EditorPrefs.DeleteAll();
        }

    }
}