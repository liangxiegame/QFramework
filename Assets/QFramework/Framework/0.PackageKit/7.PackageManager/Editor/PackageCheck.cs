/****************************************************************************
 * Copyright (c) 2018.7 ~ 11 liangxie
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

using System.Collections.Generic;
using QF.Action;
using QF.Extensions;
using QF.PackageKit;
using UnityEditor;

namespace QF.Editor
{
	[InitializeOnLoad]
	public class PackageCheck
	{
		enum CheckStatus
		{
			WAIT,
			COMPARE,
			NONE
		}

		private CheckStatus mCheckStatus;

		private double mNextCheckTime = 0;

		private double mCheckInterval = 60;


		static PackageCheck()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode && Network.IsReachable)
			{
				PackageCheck packageCheck = new PackageCheck()
				{
					mCheckStatus = CheckStatus.WAIT,
					mNextCheckTime = EditorApplication.timeSinceStartup,
				};

				EditorApplication.update = packageCheck.CustomUpdate;

			}
		}

		private void CustomUpdate()
		{
			// 添加网络判断
			if (!Network.IsReachable) return;
			
			switch (mCheckStatus)
			{
				case CheckStatus.WAIT:
					if (EditorApplication.timeSinceStartup >= mNextCheckTime)
					{
						mCheckStatus = CheckStatus.COMPARE;
					}

					break;

				case CheckStatus.COMPARE:

					ProcessCompare();

					break;
			}
		}


		private void GoToWait()
		{
			mCheckStatus = CheckStatus.WAIT;

			mNextCheckTime = EditorApplication.timeSinceStartup + mCheckInterval;
		}

		private bool ReCheckConfigDatas()
		{
			mCheckInterval = 60;

			return true;
		}

		private void ProcessCompare()
		{
            if (Network.IsReachable)
            {
                EditorActionKit.ExecuteNode(new GetAllRemotePackageInfo(packageDatas =>
                {
                    if (packageDatas == null)
                    {
                        return;
                    }

                    if (PackageKitModel.State.VersionCheck)
                    {
	                    CheckNewVersionDialog(packageDatas, PackageInfosRequestCache.Get().PackageDatas);
                    }
                }));
            }
			
			ReCheckConfigDatas();
			GoToWait();
		}

		private static bool CheckNewVersionDialog(List<PackageData> requestPackageDatas,List<PackageData> cachedPackageDatas)
		{
			foreach (var requestPackageData in requestPackageDatas)
			{
				var cachedPacakgeData =
					cachedPackageDatas.Find(packageData => packageData.Name == requestPackageData.Name);

				var installedPackageVersion = InstalledPackageVersions.Get()
					.Find(packageVersion => packageVersion.Name == requestPackageData.Name);

				if (installedPackageVersion == null)
				{
				}
				else if (cachedPacakgeData == null &&
				         requestPackageData.VersionNumber > installedPackageVersion.VersionNumber ||
				         cachedPacakgeData != null && requestPackageData.Installed &&
				         requestPackageData.VersionNumber > cachedPacakgeData.VersionNumber &&
				         requestPackageData.VersionNumber > installedPackageVersion.VersionNumber)
				{

					ShowDisplayDialog(requestPackageData.Name);
					return false;
				}
			}

			return true;
		}


		private static void ShowDisplayDialog(string packageName)
		{
			var result = EditorUtility.DisplayDialog("PackageManager",
				"{0} 有新版本更新,请前往查看(如需不再提示请点击前往查看，并取消勾选 Version Check)".FillFormat(packageName),
				"前往查看", "稍后查看");

			if (result)
			{
				EditorApplication.ExecuteMenuItem(FrameworkMenuItems.Preferences);
			}
		}
	}
}