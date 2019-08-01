#if !NOT_UNITY3D

using System;
using UnityEngine;

namespace Zenject
{
    [NoReflectionBaking]
    public class GameObjectCreationParameters
    {
        public string Name
        {
            get;
            set;
        }

        public string GroupName
        {
            get;
            set;
        }

        public Transform ParentTransform
        {
            get;
            set;
        }

        public Func<InjectContext, Transform> ParentTransformGetter
        {
            get;
            set;
        }

        public Vector3? Position
        {
            get;
            set;
        }

        public Quaternion? Rotation
        {
            get;
            set;
        }

        public static readonly GameObjectCreationParameters Default = new GameObjectCreationParameters();

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + (Name == null ? 0 : Name.GetHashCode());
                hash = hash * 29 + (GroupName == null ? 0 : GroupName.GetHashCode());
                hash = hash * 29 + (ParentTransform == null ? 0 : ParentTransform.GetHashCode());
                hash = hash * 29 + (ParentTransformGetter == null ? 0 : ParentTransformGetter.GetHashCode());
                hash = hash * 29 + (!Position.HasValue ? 0 : Position.Value.GetHashCode());
                hash = hash * 29 + (!Rotation.HasValue ? 0 : Rotation.Value.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is GameObjectCreationParameters)
            {
                GameObjectCreationParameters otherId = (GameObjectCreationParameters)other;
                return otherId == this;
            }

            return false;
        }

        public bool Equals(GameObjectCreationParameters that)
        {
            return this == that;
        }

        public static bool operator ==(GameObjectCreationParameters left, GameObjectCreationParameters right)
        {
            return Equals(left.Name, right.Name)
                && Equals(left.GroupName, right.GroupName);
        }

        public static bool operator !=(GameObjectCreationParameters left, GameObjectCreationParameters right)
        {
            return !left.Equals(right);
        }
    }
}

#endif
