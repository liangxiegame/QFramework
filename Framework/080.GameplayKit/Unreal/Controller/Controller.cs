
namespace QFramework
{
    /// <summary>
    /// 控制器 是负责定向Pawn的Actor。它们通常有两种风格：AI控制器和玩家控制器。一个控制器可以“拥有”一个Pawn来控制它。
    /// </summary>
    public class Controller : GameplayKitObject
    {
        protected IPuppet mPuppet = null;

        public IPuppet Puppet
        {
            get { return mPuppet; }
        }

        public void BindPuppet(IPuppet value)
        {
            if (mPuppet != value)
            {
                if (mPuppet != null)
                {
                    OnPuppetUnbind(mPuppet);
                    mPuppet.Controller = null;
                }

                mPuppet = value;
                mPuppet.Controller = this;
                OnPuppetBind(value);
            }
        }

        protected virtual void OnPuppetBind(IPuppet puppet)
        {
            
        }

        protected virtual void OnPuppetUnbind(IPuppet puppet)
        {
            
        }
    }
}