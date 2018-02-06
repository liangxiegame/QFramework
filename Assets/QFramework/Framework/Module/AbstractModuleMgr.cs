/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using System.Collections.Generic;

    public class AbstractModuleMgr : AbstractActor
    {
        private Dictionary<string, IModule> mModuleMgrMap = new Dictionary<string,IModule>();

        public IModule GetModule(string name)
        {
            IModule ret = null;
            if (mModuleMgrMap.TryGetValue(name, out ret))
            {
                return ret;
            }
            return null;
        }

        protected override void OnAddComponent(IActorComponent actorComponent)
        {
            if (actorComponent is IModule)
            {
                IModule module = actorComponent as IModule;
                string name = module.GetType().Name;
                if (mModuleMgrMap.ContainsKey(name))
                {
                    Log.E("ModuleMgr Already Add Module:" + name);
                    return;
                }
                mModuleMgrMap.Add(name, module);

                OnModuleRegister(module);
            }
        }

        protected virtual void OnModuleRegister(IModule module)
        {

        }
    }
}
