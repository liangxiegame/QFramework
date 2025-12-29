#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal static class UploadPackage
    {
        private static string UploadURL => "https://api.liangxiegame.com/qf/v4/package/add";

        internal static void DoUpload(PackageVersion packageVersion, System.Action succeed)
        {
            EditorUtility.DisplayProgressBar("插件上传", "打包中...", 0.1f);

            var fileName = packageVersion.Name + "_" + packageVersion.Version + ".unitypackage";
            var fullPath = ExportPaths(fileName, packageVersion.IncludeFileOrFolders.ToArray());
            var file = File.ReadAllBytes(fullPath);

            var form = new WWWForm();
            form.AddField("username", User.Username.Value);
            form.AddField("password", User.Password.Value);
            form.AddField("name", packageVersion.Name);
            form.AddField("version", packageVersion.Version);
            form.AddBinaryData("file", file);
            form.AddField("releaseNote", packageVersion.Readme.content);
            form.AddField("installPath", packageVersion.InstallPath);

            form.AddField("accessRight", packageVersion.AccessRight.ToString().ToLower());
            form.AddField("docUrl", packageVersion.DocUrl);
            form.AddField("status",(int)packageVersion.Status);

            if (packageVersion.Type == PackageType.FrameworkModule)
            {
                form.AddField("type", "fm");
            }
            else if (packageVersion.Type == PackageType.Shader)
            {
                form.AddField("type", "s");
            }
            else if (packageVersion.Type == PackageType.AppOrGameDemoOrTemplate)
            {
                form.AddField("type", "agt");
            }
            else if (packageVersion.Type == PackageType.Plugin)
            {
                form.AddField("type", "p");
            }

            Debug.Log(fullPath);

            EditorUtility.DisplayProgressBar("插件上传", "上传中...", 0.2f);

            EditorHttp.Post(UploadURL, form, (response) =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    EditorUtility.ClearProgressBar();
                    Debug.Log(response.Text);
                    if (succeed != null)
                    {
                        succeed();
                    }

                    File.Delete(fullPath);
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("插件上传", string.Format("上传失败!{0}", response.Error), "确定");
                    File.Delete(fullPath);
                }
            });
        }

        private static readonly string mExportRootDir = Path.Combine(Application.dataPath, "../");

        internal static string ExportPaths(string exportPackageName, params string[] paths)
        {
            var filePath = Path.Combine(mExportRootDir, exportPackageName);

            AssetDatabase.ExportPackage(paths, filePath, ExportPackageOptions.Recurse);
            AssetDatabase.Refresh();
            return filePath;
        }
    }
}
#endif