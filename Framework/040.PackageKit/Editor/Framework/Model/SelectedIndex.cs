using UnityEditor;

namespace QFramework.PackageKit.Model
{
    public class PackageKitSelectedIndexChangedEvent
    {
        
    }

    public interface IPackageKitSelectIndexModel
    {
        int SelectedIndex { get; set; }
    }
    
    public class PackageKitSelectedIndexModel
    {
        public int SelectedIndex
        {
            get
            {
                return EditorPrefs.GetInt("PACKAGE_KIT_SELECTED_INDEX", 0);
            }
            set
            {
                if (EditorPrefs.GetInt("PACKAGE_KIT_SELECTED_INDEX", 0) != value)
                {
                    EditorPrefs.SetInt("PACKAGE_KIT_SELECTED_INDEX", value);
                    
                }
            }
            
            
        }
        
    }
}