using System;
using UnityEngine;
using QFramework;

namespace Entitas.Unity
{
    public class EntityLink : MonoBehaviour
    {
        public IEntity Entity
        {
            get { return mEntity; }
        }

        public IContext Context
        {
            get { return mContext; }
        }

        IEntity mEntity;
        IContext mContext;

        public void Link(IEntity entity, IContext context)
        {
            if (mEntity != null)
            {
                throw new Exception("EntityLink is already linked to " + mEntity + "!");
            }

            mEntity = entity;
            mContext = context;
            mEntity.Retain(this);
        }

        public void Unlink()
        {
            if (mEntity == null)
            {
                throw new Exception("EntityLink is already unlinked!");
            }

            mEntity.Release(this);
            mEntity = null;
            mContext = null;
        }

        public override string ToString()
        {
            return "EntityLink(" + gameObject.name + ")";
        }
    }

    public static class EntityLinkExtension
    {

        public static EntityLink GetEntityLink(this GameObject gameObject)
        {
            return gameObject.GetComponent<EntityLink>();
        }

        public static EntityLink Link(this GameObject gameObject, IEntity entity, IContext context)
        {
            var link = gameObject.GetEntityLink();
            if (link == null)
            {
                link = gameObject.AddComponent<EntityLink>();
            }

            link.Link(entity, context);
            return link;
        }

        public static void Unlink(this GameObject gameObject)
        {
            gameObject.GetEntityLink().Unlink();
        }
    }
}