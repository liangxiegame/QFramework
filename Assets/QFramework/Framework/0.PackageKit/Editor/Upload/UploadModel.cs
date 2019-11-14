using System;
using System.IO;
using QF.DVA;
using Unidux;
using UniRx;
using UnityEditor;

namespace QF.PackageKit.Upload
{
    public class UploadProgress
    {
        public const byte STATE_GENERATE_INIT      = 0;
        public const byte STATE_GENERATE_UPLOADING = 2;
        public const byte STATE_GENERATE_COMPLETE  = 3;

    }
    
    [Serializable]
    public class State : DvaState
    {
        public string UpdateResult = "";
        public string NoticeMessage = "";
        public byte Progress = UploadProgress.STATE_GENERATE_INIT;
    }
    
    public class UploadModel : DvaModelEditor<UploadModel,State>
    {
        private string _ns = "upload";
        private State _initialState = new State();

        public class Effects
        {
            public static void Publish(PackageVersion packageVersion,bool deleteLocal)
            {
                Dispatch("setNotice", "插件导出中,请稍后...");

                Observable.NextFrame().Subscribe(_ =>
                {
                    Dispatch("setNotice", "插件上传中,请稍后...");
                    Dispatch("setProgress", UploadProgress.STATE_GENERATE_UPLOADING);
						
                    UploadPackage.DoUpload(packageVersion, () =>
                    {
                        if (deleteLocal)
                        {
                            Directory.Delete(packageVersion.InstallPath, true);
                            AssetDatabase.Refresh();
                        }
                        Dispatch("setResult","上传成功");
                        Dispatch("setProgress", UploadProgress.STATE_GENERATE_COMPLETE);
                    });
                });
            }
        }
        
        
        public override State Reduce(State state, DvaAction action)
        {
            if (action.Type == "setProgress")
            {
                state.Progress = (byte)action.Payload;
            }
            else if (action.Type == "setNotice")
            {
                state.NoticeMessage = (string) action.Payload;
            } else if (action.Type == "setResult")
            {
                state.UpdateResult = (string) action.Payload;
            }
            
            return state;
        }

        protected override string Namespace
        {
            get { return _ns; }
        }

        protected override State InitialState
        {
            get { return _initialState; }
        }
    }
}