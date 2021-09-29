using System.IO;
using System.Text.RegularExpressions;
using QFramework;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void Awake()
    {
        // string str = "Game@hotfix.asmdef";
        // var mat = Regex.Match(str, @".*?@hotfix\.asmdef");
        // print(mat.Success);
        // return;
        DirectoryInfo root = new DirectoryInfo(Application.dataPath);
        root.ForeachFiles(info =>
        {
            print(info.Name);
            if (info.Name == "Game@hotfix.asmdef")
            {
                int a = 1;
            }
            var match = Regex.Match(info.Name, @".*?@hotfix\.asmdef");
            if (match.Success)
            {
                print(info.DirectoryName);
                return true;
            }
            return false;
        });
    }
}
