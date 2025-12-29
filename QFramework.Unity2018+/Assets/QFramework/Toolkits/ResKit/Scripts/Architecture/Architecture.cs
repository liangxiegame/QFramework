using UnityEngine;

namespace QFramework
{
    internal class Architecture : Architecture<Architecture>
    {
        internal static ZipFileHelper ZipFileHelper { get; } = new ZipFileHelper();
        internal static BinarySerializer BinarySerializer { get; } = new BinarySerializer();
        protected override void Init()
        {
            RegisterSystem(new ConsoleModuleSystem());
            RegisterUtility(ZipFileHelper);
            RegisterUtility(BinarySerializer);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInit()
        {
            InitArchitecture();
            LogKit.I("ResKit.Architecture Inited");
        }
    }
}