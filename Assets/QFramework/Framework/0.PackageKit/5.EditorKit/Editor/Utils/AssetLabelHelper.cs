

namespace QF
{
    using System;
    using System.Linq;    
    using UnityEditor;
        
    public static class AssetLabelHelper
    {

        public static void AddLabel(this string selfPath, params string[] labels)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(selfPath);

            AddLabel(obj, labels);
        }

        public static void AddLabel(this UnityEngine.Object obj, params string[] labels)
        {
            var existsLabels = AssetDatabase.GetLabels(obj).ToList();

            foreach (var label in labels)
            {
                if (!existsLabels.Any(existsLabel => existsLabel.Equals(label)))
                {
                    existsLabels.Add(label);
                }
            }
            
            AssetDatabase.SetLabels(obj, existsLabels.ToArray());
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        public static bool HasAssetLabel(this string selfPath, string label)
        {
            return HasAssetLabel(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(selfPath), label);
        }
        
        public static bool HasAssetLabel(this UnityEngine.Object obj, string label)
        {
			return AssetDatabase.GetLabels(obj).ToList().Any(labelName=>labelName.Equals(label));
        }

        public static void RemoveLabelsWhere(this string selfPath, Func<string, bool> matcher)
        {
            RemoveLabelsWhere(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(selfPath),matcher);
        }

        public static void RemoveLabelsWhere(this UnityEngine.Object obj, Func<string,bool> matcher)
        {
            var labels = AssetDatabase.GetLabels(obj).ToList();
            labels.RemoveAll(label => matcher(label));
            AssetDatabase.SetLabels(obj, labels.ToArray());
        }
        
        
        public static void RemoveAllLabels(this string selfPath)
        {
            RemoveAllLabels(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(selfPath));
        }
        
        public static void RemoveAllLabels(this UnityEngine.Object obj)
        {
            AssetDatabase.ClearLabels(obj);
        }
    }
}