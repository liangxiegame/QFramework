/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;

namespace QFramework
{
    internal class PublishPackageCommand : AbstractCommand
    {
        private readonly PackageVersion mPackageVersion;

        public PublishPackageCommand(PackageVersion packageVersion)
        {
            mPackageVersion = packageVersion;
        }

        protected override void OnExecute()
        {
            if (mPackageVersion.Readme.content.Length < 2)
            {
                IMGUIHelper.ShowEditorDialogWithErrorMsg("请输入版本修改说明");
                return;
            }

            if (!IsVersionValid(mPackageVersion.Version))
            {
                IMGUIHelper.ShowEditorDialogWithErrorMsg("请输入正确的版本号 格式:vX.Y.Z");
                return;
            }

            mPackageVersion.DocUrl = "https://liangxiegame.com";

            mPackageVersion.Readme = new ReleaseItem(mPackageVersion.Version, mPackageVersion.Readme.content,
                User.Username.Value,
                DateTime.Now);

            mPackageVersion.Save();

            AssetDatabase.Refresh();

            RenderEndCommandExecutor.PushCommand(() => { PublishPackage(mPackageVersion, false); });
        }

        public void PublishPackage(PackageVersion packageVersion, bool deleteLocal)
        {
            PackageMakerEditor.NoticeMessage.Value = "插件上传中,请稍后...";

            PackageMakerEditor.InUploadingView.Value = true;
            PackageMakerEditor.InEditorView.Value = false;
            PackageMakerEditor.InFinishView.Value = false;

            UploadPackage.DoUpload(packageVersion, () =>
            {
                if (deleteLocal)
                {
                    Directory.Delete(packageVersion.InstallPath, true);
                    AssetDatabase.Refresh();
                }

                PackageMakerEditor.UpdateResult.Value = "上传成功";

                PackageMakerEditor.InEditorView.Value = false;
                PackageMakerEditor.InUploadingView.Value = false;
                PackageMakerEditor.InFinishView.Value = true;

                if (EditorUtility.DisplayDialog("上传结果", PackageMakerEditor.UpdateResult.Value, "OK"))
                {
                    AssetDatabase.Refresh();

                    EditorWindow.GetWindow<PackageMakerEditor>().Close();
                }
            });
        }

        public static bool IsVersionValid(string version)
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
#endif