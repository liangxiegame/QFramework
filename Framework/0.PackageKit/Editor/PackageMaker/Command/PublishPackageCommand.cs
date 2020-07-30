using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework.PackageKit
{
    public class PublishPackageCommand : IPackageMakerCommand
    {
        private PackageVersion mPackageVersion;
        public PublishPackageCommand(PackageVersion packageVersion)
        {
            mPackageVersion = packageVersion;
        }

        public void Execute()
        {
            if (mPackageVersion.Readme.content.Length < 2)
            {
                DialogUtils.ShowErrorMsg("请输入版本修改说明");
                return;
            }

            if (!IsVersionValide(mPackageVersion.Version))
            {
                DialogUtils.ShowErrorMsg("请输入正确的版本号 格式:vX.Y.Z");
                return;
            }

            mPackageVersion.DocUrl = "http://lianxiegame.com";

            mPackageVersion.Readme = new ReleaseItem(mPackageVersion.Version, mPackageVersion.Readme.content,
                User.Username.Value,
                DateTime.Now);

            mPackageVersion.Save();

            AssetDatabase.Refresh();

            RenderEndCommandExecuter.PushCommand(() =>
            {
                PublishPackage(mPackageVersion, false);
            });
        }
        
        public void PublishPackage(PackageVersion packageVersion, bool deleteLocal)
        {
            PackageMakerState.NoticeMessage.Value = "插件上传中,请稍后...";

            PackageMakerState.InUploadingView.Value = true;
            PackageMakerState.InEditorView.Value = false;
            PackageMakerState.InFinishView.Value = false;

            UploadPackage.DoUpload(packageVersion, () =>
            {
                if (deleteLocal)
                {
                    Directory.Delete(packageVersion.InstallPath, true);
                    AssetDatabase.Refresh();
                }

                PackageMakerState.UpdateResult.Value = "上传成功";
                
                PackageMakerState.InEditorView.Value = false;
                PackageMakerState.InUploadingView.Value = false;
                PackageMakerState.InFinishView.Value = true;

                if (EditorUtility.DisplayDialog("上传结果", PackageMakerState.UpdateResult.Value, "OK"))
                {

                    AssetDatabase.Refresh();

                    EditorWindow.focusedWindow.Close();
                }
            });
        }
        public static bool IsVersionValide(string version)
        {
            if (version == null)
            {
                return false;
            }

            var t = version.Split('.');
            return t.Length == 3 && version.StartsWith("v");
        }
    }
}