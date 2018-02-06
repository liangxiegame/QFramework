/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
    public interface IActorComponent : IComponent
    {
        AbstractActor Actor { get; }

        int ComponentOrder { get; }

        //自身的初始化工作
        void AwakeComponent(AbstractActor actor);

        //和其它组件有关的初始化工作
        void OnComponentStart();

        void OnComponentEnable();
        void OnComponentUpdate(float dt);
        void OnComponentLateUpdate(float dt);
        void OnComponentDisable();
        void DestroyComponent();
    }
}
