using System;
using System.Collections.Generic;
using QF.Action;
using QF.DVA;
using QFramework;
using Unidux;
using UnityEditor;

namespace QF.PackageKit
{
    /// <summary>
    /// 状态
    /// </summary>
    [Serializable]
    public class State : DvaState
    {
        public List<PackageData> PackageDatas = new List<PackageData>();

        public bool VersionCheck
        {
            get { return EditorPrefs.GetBool("QFRAMEWORK_VERSION_CHECK", true); }
            set { EditorPrefs.SetBool("QFRAMEWORK_VERSION_CHECK", value); }
        }
    }

    public class PackageKitModel : DvaModelEditor<PackageKitModel, State>
    {
        public override State Reduce(State state, DvaAction action)
        {
            switch (action.Type)
            {
                case "setPackages":
                    state.PackageDatas = action.Payload as List<PackageData>;
                    break;
            }

            return state;
        }

        protected override string Namespace
        {
            get { return "package"; }
        }

        protected override State InitialState
        {
            get
            {
                return new State()
                {
                    PackageDatas = PackageInfosRequestCache.Get().PackageDatas
                };
            }
        }
    }
}