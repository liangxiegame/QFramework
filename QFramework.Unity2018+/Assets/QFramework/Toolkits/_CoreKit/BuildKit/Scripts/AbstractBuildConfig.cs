using System.IO;
using UnityEngine;

namespace QFramework
{
    [System.Serializable]
    public class AbstractBuildConfig<T> where T : AbstractBuildConfig<T>,new()
    {
        public static string FileName => typeof(T).Name + "_config";

        
#if UNITY_EDITOR
        public const string FolderPath = "Assets/Art/Config/Resources/BuildConfig/";
        public static string FilePath => FolderPath.CreateDirIfNotExists() + FileName + ".json";
        
        public void Save()
        {
            var jsonContent = JsonUtility.ToJson(this, true);
            File.WriteAllText(FilePath,jsonContent);
        }
#endif

        public static T Default => mDefault = mDefault ?? Load();
        private static T mDefault = null;

        public static T Load()
        {
            try
            {
                var textAsset = Resources.Load<TextAsset>($"BuildConfig/{FileName}");
                return JsonUtility.FromJson<T>(textAsset.text);
            }
            catch
            {
                return new T();
            }
        }
    }
}