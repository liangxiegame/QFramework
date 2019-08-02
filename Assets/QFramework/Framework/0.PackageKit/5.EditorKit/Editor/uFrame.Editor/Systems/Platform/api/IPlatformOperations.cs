using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QF.GraphDesigner
{
    public interface IPlatformOperations
    {
        void OpenScriptFile(string filePath);
        void OpenLink(string link);
        string GetAssetPath(object graphData);
        bool MessageBox(string title, string message, string ok);
        bool MessageBox(string title, string message, string ok, string cancel);
        void SaveAssets();
        void RefreshAssets();
        void Progress(float progress, string message);
    }
}
