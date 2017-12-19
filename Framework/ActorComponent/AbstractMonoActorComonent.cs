/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using UnityEngine;

    public class AbstractMonoActorComponent : MonoBehaviour, IActorComponent
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

            OnAwakeCom();
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
            OnDestroyCom();
            mActor = null;
            Destroy(this);
        }

#region 子类继承
        protected virtual void OnActorBind(AbstractActor actor)
        {

        }

        protected virtual void OnAwakeCom()
        {

        }
        protected virtual void OnDestroyCom()
        {

        }
#endregion
    }
}
