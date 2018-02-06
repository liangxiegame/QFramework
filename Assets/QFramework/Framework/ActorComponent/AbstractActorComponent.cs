/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
    public class AbstractActorComponent : IActorComponent
    {
        private AbstractActor mActor;

        public AbstractActor Actor
        {
            get { return mActor; }
        }

        public virtual int ComponentOrder
        {
            get { return ComponentOrderDefine.DEFAULT; }
        }

        public void AwakeComponent(AbstractActor actor)
        {
            mActor = actor;

            OnActorBind(actor);

            OnComponentAwake();
        }

        public void OnComponentDisable()
        {

        }

        public void OnComponentEnable()
        {

        }

        public virtual void OnComponentLateUpdate(float dt)
        {

        }

        public virtual void OnComponentStart()
        {

        }

        public virtual void OnComponentUpdate(float dt)
        {

        }

        public void DestroyComponent()
        {
            OnComponentDestroy();
            mActor = null;
        }

#region 子类继承
        protected virtual void OnActorBind(AbstractActor actor)
        {

        }

        protected virtual void OnComponentAwake()
        {

        }
        protected virtual void OnComponentDestroy()
        {

        }
#endregion
    }
}
