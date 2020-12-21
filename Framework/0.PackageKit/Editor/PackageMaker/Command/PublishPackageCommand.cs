/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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
using System.IO;
using UnityEditor;

namespace QFramework
{
    public class PublishPackageCommand : Command<PackageMaker>
    {
        private readonly PackageVersion mPackageVersion;

        public PublishPackageCommand(PackageVersion packageVersion)
        {
            mPackageVersion = packageVersion;
        }

        public override void Execute()
        {
            if (mPackageVersion.Readme.content.Length < 2)
            {
                (GetConfig<PackageKit>() as IArchitecture)
                    .GetUtility<IEditorDialogUtility>().ShowErrorMsg("请输入版本修改说明");
                return;
            }

            if (!IsVersionValide(mPackageVersion.Version))
            {
                (GetConfig<PackageKit>() as IArchitecture)
                    .GetUtility<IEditorDialogUtility>().ShowErrorMsg("请输入正确的版本号 格式:vX.Y.Z");
                return;
            }

            mPackageVersion.DocUrl = "http://lianxiegame.com";

            mPackageVersion.Readme = new ReleaseItem(mPackageVersion.Version, mPackageVersion.Readme.content,
                User.Username.Value,
                DateTime.Now);

            mPackageVersion.Save();

            AssetDatabase.Refresh();

            RenderEndCommandExecuter.PushCommand(() => { PublishPackage(mPackageVersion, false); });
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