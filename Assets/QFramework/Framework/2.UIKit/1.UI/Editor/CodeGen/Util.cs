using System.Text;
using UnityEngine;

namespace QFramework
{
    public class CodeGenUtil
    {
        public static string PathToParent(Transform trans, string parentName)
        {
            var retValue = new StringBuilder(trans.name);

            while (trans.parent != null)
            {
                if (trans.parent.name.Equals(parentName))
                {
                    break;
                }

                retValue = trans.parent.name.Append("/").Append(retValue);

                trans = trans.parent;
            }

            return retValue.ToString();
        }
        
        public static string GetLastDirName(string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var dirs = name.Split('/');

            return dirs[dirs.Length - 2];
        }
    }
}