using System;
using System.IO;
using BindKit.Commands;
using BindKit.ViewModels;
using UnityEditor;
using UnityEngine;

namespace QFramework.PackageKit
{
    public class PackageMakerViewModel : ViewModelBase
    {
        private readonly PackageVersion mPackageVersion;

        public PackageMakerViewModel(PackageVersion packageVersion)
        {
            mPackageVersion = packageVersion;

            mReleaseNote = mPackageVersion.Readme.content;
        }

        public string Version
        {
            get { return mPackageVersion.Version; }
            set { this.Set(ref mPackageVersion.Version, value, "Version"); }
        }

        private Enum mAccessRight = PackageAccessRight.Private;

        public Enum AccessRight
        {
            get { return mPackageVersion.AccessRight; }
            set
            {
                RaisePropertyChanged("AccessRight");
                mPackageVersion.AccessRight = (PackageAccessRight) value;
            }
        }

        private string mReleaseNote = "";

        public string ReleaseNote
        {
            get { return mReleaseNote; }
            set { this.Set(ref mReleaseNote, value, "ReleaseNote"); }
        }


        public Enum Type
        {
            get { return mPackageVersion.Type; }
            set
            {
                RaisePropertyChanged("Type");
                mPackageVersion.Type = (PackageType) value;
            }
        }

        private string mDocUrl = "http://lianxiegame.com";

        public string DocUrl
        {
            get { return mDocUrl; }
            set { this.Set(ref mDocUrl, value, "DocUrl"); }
        }


        private bool mInEditorView = true;

        public bool InEditorView
        {
            get { return mInEditorView; }
            set { this.Set(ref mInEditorView, value, "InEditorView"); }
        }

        private bool mInUploadingView = false;

        public bool InUploadingView
        {
            get { return mInUploadingView; }
            set { this.Set(ref mInUploadingView, value, "InUploadingView"); }
        }

        private bool mInFinishView = false;

        public bool InFinishView
        {
            get { return mInFinishView; }
            set { this.Set(ref mInFinishView, value, "InFinishView"); }
        }

        public void Paste()
        {
            DocUrl = GUIUtility.systemCopyBuffer;
        }

        public SimpleCommand<PackageVersion> Publish
        {
            get
            {
                return new SimpleCommand<PackageVersion>((version =>
                {
                    if (mReleaseNote.Length < 2)
                    {
                        DialogUtils.ShowErrorMsg("请输入版本修改说明");
                        return;
                    }

                    if (!IsVersionValide(Version))
                    {
                        DialogUtils.ShowErrorMsg("请输入正确的版本号 格式:vX.Y.Z");
                        return;
                    }

                    version.DocUrl = DocUrl;

                    version.Readme = new ReleaseItem(Version, mReleaseNote,
                        User.Username.Value,
                        DateTime.Now);

                    version.Save();

                    AssetDatabase.Refresh();

                    RenderEndCommandExecuter.PushCommand(() => { PublishPackage(version, false); });
                }));
            }
        }

        private string mNoticeMessage = "";

        public string NoticeMessage
        {
            get { return mNoticeMessage; }
            set { this.Set(ref mNoticeMessage, value, "NoticeMessage"); }
        }

        private string mUpdateResult = "";

        public string UpdateResult
        {
            get { return mUpdateResult; }
            set { this.Set(ref mUpdateResult, value, "UpdateResult"); }
        }

        public void PublishPackage(PackageVersion packageVersion, bool deleteLocal)
        {
            NoticeMessage = "插件上传中,请稍后...";

            InUploadingView = true;
            InEditorView = false;
            InFinishView = false;

            UploadPackage.DoUpload(packageVersion, () =>
            {
                if (deleteLocal)
                {
                    Directory.Delete(packageVersion.InstallPath, true);
                    AssetDatabase.Refresh();
                }

                UpdateResult = "上传成功";

                InEditorView = false;
                InUploadingView = false;
                InFinishView = true;

                if (EditorUtility.DisplayDialog("上传结果", UpdateResult, "OK"))
                {

                    AssetDatabase.Refresh();

                    Window.focusedWindow.Close();
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