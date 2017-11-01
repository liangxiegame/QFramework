/****************************************************************************
 * Copyright (c) 2017 liangxieq
 * 
 * https://github.com/UniPM/UniPM
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace UniPM
{
    using QFramework;
    using System.Linq;
    using UnityEditor;
    using Path = System.IO.Path;
    
    [System.Serializable]
    public class PackageConfig
    {
        public string Name = "TestCompress";

        public string CopyRight = "UniPM";

        public string Version;

        public string PackagePath;

        public string ConfigFilePath
        {
            get { return PackagePath.CombinePath("Package.json"); }
        }

        public string ReleaseNote;

        public string DownloadURL;

        public PackageConfig(string packagePath)
        {
            Name = packagePath.Split(Path.DirectorySeparatorChar).Last();
            PackagePath = packagePath;
            Version = "0.0.0";
            DownloadURL = EditorPrefs.GetString(PackageListConfig.GitUrl)
                .AppendFormat("/raw/master/{0}/{0}.zip", Name).ToString();
        }

        void UpdateView()
        {
            PackagePath.RemoveAllLabels();
            ConfigFilePath.RemoveAllLabels();
            
            UpdateVersionView();
            
            UpdateLabelView(Name);
            
            UpdateLabelView("UniPM");

            AssetDatabase.Refresh();
        }


        #region 这个操作应该都在面板上做
        void UpdateLabelView(string name)
        {
            PackagePath.AddLabel(name);
            ConfigFilePath.AddLabel(name);
        }

        void UpdateVersionView()
        {
            int tmpInt = 0;
            PackagePath.RemoveLabelsWhere(label =>
                label.StartsWith("v") && int.TryParse(label.Last().ToString(), out tmpInt) &&
                int.TryParse(label[1].ToString(), out tmpInt));
            ConfigFilePath.RemoveLabelsWhere(label =>
                label.StartsWith("v") && int.TryParse(label.Last().ToString(), out tmpInt) &&
                int.TryParse(label[1].ToString(), out tmpInt));

            UpdateLabelView(string.Format("v{0}", Version));
        }

        public void UpdateVersion(int pos)
        {
            var versionNumbers = Version.Split('.');
            int versionNumber = int.Parse(versionNumbers[pos]);
            versionNumber++;
            versionNumbers[pos] = versionNumber.ToString();
            Version = string.Join(".", versionNumbers);
            UpdateView();
        }
        
        public void ResetVersion(int pos)
        {
            var versionNumbers = Version.Split('.');
            int versionNumber = int.Parse(versionNumbers[pos]);
            versionNumber = 0;
            versionNumbers[pos] = versionNumber.ToString();
            Version = string.Join(".", versionNumbers);
            UpdateView();
        }

        #endregion

        
        public static PackageConfig LoadFromPath(string configFilePath)
        {
            return SerializeHelper.LoadJson<PackageConfig>(configFilePath);
        }
        
        public void SaveLocal()
        {
            this.SaveJson(ConfigFilePath);
            AssetDatabase.Refresh();
            UpdateView();
        }

        public void SaveExport()
        {
            this.SaveJson(ConfigFilePath);
        }
    }
}