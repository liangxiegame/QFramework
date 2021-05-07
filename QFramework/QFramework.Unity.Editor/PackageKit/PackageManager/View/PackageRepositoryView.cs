/****************************************************************************
 * Copyright (c) 2021.3 liangxie
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

using UnityEngine;

namespace QFramework
{
    public enum PackageRepositoryActionState
    {
        Uninstall,
        HasNewVersion,
        Installed,
    }

    public class PackageRepositoryView : HorizontalLayout, IController
    {
        public PackageRepositoryView(PackageRepository packageRepository)
        {
            // EasyIMGUI.Label()
            //     .Text(installedPackage != null ? installedPackage.Version : " ")
            //     .TextMiddleCenter()
            //     .Width(PackageListHeaderView.LocalVersionWidth)
            //     .Parent(this);
            //
            //
            // var actionState = PackageRepositoryActionState.Uninstall;
            //
            //
            // if (installedPackage == null)
            // {
            //     actionState = PackageRepositoryActionState.Uninstall;
            //
            //     new BoxView(LocaleText.Uninstall)
            //         .TextMiddleCenter()
            //         .Width(PackageListHeaderView.ActionStateWidth)
            //         .Parent(this)
            //         .FontColor(Color.white)
            //         .BackgroundColor = Color.black;
            // }
            // else if (packageRepository.VersionNumber > installedPackage.VersionNumber)
            // {
            //     actionState = PackageRepositoryActionState.HasNewVersion;
            //
            //     new BoxView(LocaleText.HasNewVersion)
            //         .TextMiddleCenter()
            //         .Width(PackageListHeaderView.ActionStateWidth)
            //         .Parent(this)
            //         .FontColor(Color.white)
            //         .BackgroundColor = Color.yellow;
            // }
            // else if (packageRepository.VersionNumber == installedPackage.VersionNumber)
            // {
            //     actionState = PackageRepositoryActionState.Installed;
            //
            //     new BoxView(LocaleText.Installed)
            //         .TextMiddleCenter()
            //         .Width(PackageListHeaderView.ActionStateWidth)
            //         .Parent(this)
            //         .FontColor(Color.white)
            //         .BackgroundColor = Color.green;
            // }
            //
            // EasyIMGUI.Label()
            //     .Text(packageRepository.accessRight)
            //     .TextMiddleCenter()
            //     .Width(PackageListHeaderView.AccessRightWidth)
            //     .Parent(this);
            //
            // EasyIMGUI.Label()
            //     .Text(packageRepository.author)
            //     .TextMiddleCenter()
            //     .FontBold().Width(PackageListHeaderView.AuthorWidth)
            //     .Parent(this);
            //
            //
            // EasyIMGUI.Button()
            //     .Text(LocaleText.Action)
            //     .Width(PackageListHeaderView.ActionWidth)
            //     .Parent(this)
            //     .OnClick(() =>
            //     {
            //         var menuView = GenericMenuView.Create();
            //
            //         if (actionState == PackageRepositoryActionState.Uninstall)
            //         {
            //             menuView.AddMenu(LocaleText.Import,
            //                 () => {
            // this.SendCommand(new ImportPackageCommand(packageRepository)); });
            //         }
            //         else if (actionState == PackageRepositoryActionState.HasNewVersion)
            //         {
            //             menuView.AddMenu(LocaleText.Update,
            //                 () => { this.SendCommand(new UpdatePackageCommand(packageRepository)); });
            //         }
            //         else if (actionState == PackageRepositoryActionState.Installed)
            //         {
            //             menuView.AddMenu(LocaleText.Reimport,
            //                 () => { this.SendCommand(new ReimportPackageCommand(packageRepository)); });
            //         }
            //

            //
            //         menuView.Show();
            //     });
        }

        protected override void OnDisposed()
        {
        }
        

        public IArchitecture GetArchitecture()
        {
            return PackageKit.Interface;
        }
    }
}