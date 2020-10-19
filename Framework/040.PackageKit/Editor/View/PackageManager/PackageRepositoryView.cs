/****************************************************************************
 * Copyright (c) 2020.10 liangxie
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

namespace QFramework
{
    public class PackageRepositoryView : HorizontalLayout
    {
        ControllerNode<PackageKit> mControllerNode = ControllerNode<PackageKit>.Allocate();

        public PackageRepositoryView(PackageRepository packageRepository)
        {
            EasyIMGUI.Space().Pixel(2).AddTo(this);

            EasyIMGUI.Label().Text(packageRepository.name).FontBold().Width(200).AddTo(this);

            EasyIMGUI.Label().Text(packageRepository.latestVersion).TextMiddleCenter().Width(80).AddTo(this);

            var installedPackage = mControllerNode.GetModel<ILocalPackageVersionModel>()
                .GetByName(packageRepository.name);

            EasyIMGUI.Label().Text(installedPackage != null ? installedPackage.Version : " ").TextMiddleCenter().Width(100)
                .AddTo(this);

            EasyIMGUI.Label().Text(packageRepository.accessRight).TextMiddleLeft().Width(50).AddTo(this);


            if (installedPackage == null)
            {
                EasyIMGUI.Button()
                    .Text(LocaleText.Import)
                    .Width(90)
                    .AddTo(this)
                    .OnClick(() => { mControllerNode.SendCommand(new ImportPackageCommand(packageRepository)); });
            }
            else if (packageRepository.VersionNumber > installedPackage.VersionNumber)
            {
                EasyIMGUI.Button()
                    .Text(LocaleText.Update)
                    .Width(90)
                    .OnClick(() => { mControllerNode.SendCommand(new UpdatePackageCommand(packageRepository)); })
                    .AddTo(this);
            }
            else if (packageRepository.VersionNumber == installedPackage.VersionNumber)
            {
                EasyIMGUI.Button()
                    .Text(LocaleText.Reimport)
                    .Width(90)
                    .OnClick(() => { mControllerNode.SendCommand(new ReimportPackageCommand(packageRepository)); })
                    .AddTo(this);
            }
            else if (packageRepository.VersionNumber < installedPackage.VersionNumber)
            {
                EasyIMGUI.Space().Pixel(94).AddTo(this);
            }

            EasyIMGUI.Button()
                .Text(LocaleText.ReleaseNotes)
                .OnClick(() => { mControllerNode.SendCommand(new OpenDetailCommand(packageRepository)); })
                .Width(100)
                .AddTo(this);


            EasyIMGUI.Label().Text(packageRepository.author)
                .TextMiddleLeft()
                .FontBold().Width(100)
                .AddTo(this);
        }

        protected override void OnDisposed()
        {
            mControllerNode.Recycle2Cache();
            mControllerNode = null;
        }

        class LocaleText
        {
            public static string Doc
            {
                get { return Language.IsChinese ? "文档" : "Doc"; }
            }

            public static string Import
            {
                get { return Language.IsChinese ? "导入" : "Import"; }
            }

            public static string Update
            {
                get { return Language.IsChinese ? "更新" : "Update"; }
            }

            public static string Reimport
            {
                get { return Language.IsChinese ? "再次导入" : "Reimport"; }
            }

            public static string ReleaseNotes
            {
                get { return Language.IsChinese ? "详情" : "Detail"; }
            }
        }
    }
}