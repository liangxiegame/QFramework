/****************************************************************************
 * Copyright (c) 2018.8 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QFramework
{   
    [Serializable]
    public class PackageData
    {
        public string Name    = "";

        public string Version
        {
            get { return PackageVersions.First().Version; }
        }

        public string DownloadUrl
        {
            get { return PackageVersions.First().DownloadUrl; }
        }

		public string InstallPath
		{
			get { return PackageVersions.First ().InstallPath; }
		}

        public string DocUrl
        {
            get { return PackageVersions.First().DocUrl; }
        }

        public PackageType Type
        {
            get { return PackageVersions.First().Type; }
        }

        public PackageAccessRight AccessRight
        {
            get { return PackageVersions.First().AccessRight; }
        }

        public Readme readme;
        
        public List<PackageVersion> PackageVersions = new List<PackageVersion>();

        public PackageData()
        {
            readme = new Readme();
        }

        public int VersionNumber
        {
            get
            {
                var numbersStr = Version.RemoveString("v").Split('.');

                var retNumber = numbersStr[2].ToInt();
                retNumber += numbersStr[1].ToInt() * 100;
                retNumber += numbersStr[0].ToInt() * 10000;
                return retNumber;
            }
        }

        public bool Installed
        {
            get { return Directory.Exists(InstallPath); }
        }

        public void SaveVersionFile()
        {
            PackageVersions.First().Save();
        }
    }
    
    public enum PackageType
    {
        FrameworkModule, //fm
        Shader, //s
        UIKitComponent, //uc
        Plugin, // p
        AppOrGameDemoOrTemplate, //agt
        DocumentsOrTutorial, //doc
    }

    public enum PackageAccessRight
    {
        Public,
        Private
    }

    [Serializable]
    public class PackageVersion
    {
        
        public string Id { get; set; }
        
        public string Name
        {
            get { return InstallPath.GetLastDirName(); }
        }
        
        public string Version = "v0.0.0";

        public PackageType Type;

        public PackageAccessRight AccessRight;
        
        public int VersionNumber
        {
            get
            {
                var numbersStr = Version.RemoveString("v").Split('.');

                var retNumber = numbersStr[2].ToInt();
                retNumber += numbersStr[1].ToInt() * 100;
                retNumber += numbersStr[0].ToInt() * 10000;
                
                return retNumber;
            }
        }

        public string DownloadUrl;

		public string InstallPath = "Assets/QFramework/Framework/";

        public string FileName
        {
            get { return Name + "_" + Version + ".unitypackage"; }
        }

        public string DocUrl;

        public ReleaseItem Readme = new ReleaseItem();

        public void Save()
        {
            this.SaveJson(InstallPath.CreateDirIfNotExists() + "/PackageVersion.json");
        }

        public static PackageVersion Load(string filePath)
        {
            if (filePath.EndsWith("/"))
            {
                filePath += "PackageVersion.json";
            }
            else if (!filePath.EndsWith("PackageVersion.json"))
            {
                filePath += "/PackageVersion.json";
            }

            return SerializeHelper.LoadJson<PackageVersion>(filePath);
        }
    }
}