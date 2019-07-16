using System;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class FactoryToChoiceBinder<TContract> : FactoryFromBinder<TContract>
    {
        public FactoryToChoiceBinder(
            DiContainer container, BindInfo bindInfo, FactoryBindInfo factoryBindInfo)
            : base(container, bindInfo, factoryBindInfo)
        {
        }

        // Note that this is the default, so not necessary to call
        public FactoryFromBinder<TContract> ToSelf()
        {
            Assert.IsEqual(BindInfo.ToChoice, ToChoices.Self);
            return this;
        }

        public FactoryFromBinderUntyped To(Type concreteType)
        {
            BindInfo.ToChoice = ToChoices.Concrete;
            BindInfo.ToTypes.Clear();
            BindInfo.ToTypes.Add(concreteType);

            return new FactoryFromBinderUntyped(
                BindContainer, concreteType, BindInfo, FactoryBindInfo);
        }

        public FactoryFromBinder<TConcrete> To<TConcrete>()
            where TConcrete : TContract
        {
            BindInfo.ToChoice = ToChoices.Concrete;
            BindInfo.ToTypes.Clear();
            BindInfo.ToTypes.Add(typeof(TConcrete));

            return new FactoryFromBinder<TConcrete>(BindContainer, BindInfo, FactoryBindInfo);
        }
    }
}
