using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class FactoryToChoiceBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>
        : FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract>
    {
        public FactoryToChoiceBinder(
            DiContainer bindContainer, BindInfo bindInfo, FactoryBindInfo factoryBindInfo)
            : base(bindContainer, bindInfo, factoryBindInfo)
        {
        }

        // Note that this is the default, so not necessary to call
        public FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TContract> ToSelf()
        {
            Assert.IsEqual(BindInfo.ToChoice, ToChoices.Self);
            return this;
        }

        public FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TConcrete> To<TConcrete>()
            where TConcrete : TContract
        {
            BindInfo.ToChoice = ToChoices.Concrete;
            BindInfo.ToTypes.Clear();
            BindInfo.ToTypes.Add(typeof(TConcrete));

            return new FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TParam5, TConcrete>(BindContainer, BindInfo, FactoryBindInfo);
        }
    }
}
