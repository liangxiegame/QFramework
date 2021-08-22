using QFramework.ILRuntime;

namespace QFramework.Example
{
    public class MVCWithIOCConfig
    {
        private static MVCWithIOCConfig mPrivateConfig = null;
        
        static MVCWithIOCConfig mConfig
        {
            get
            {
                if (mPrivateConfig == null)
                {
                    mPrivateConfig = new MVCWithIOCConfig();
                    mPrivateConfig.Init();
                }

                return mPrivateConfig;
            }
        }

        private ILRuntimeIOCContainer mModelLayer = new ILRuntimeIOCContainer();
        

        public static T GetModel<T>() where T : class
        {
            return mConfig.mModelLayer.Resolve<T>();
        }
        
        private void Init()
        {
            // 注册模型
            mModelLayer.RegisterInstance(new MVCWithIOCModel());
        }
    }
}