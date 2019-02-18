using System.Collections.Generic;
using System.Linq;

namespace Zenject
{
    [NoReflectionBaking]
    public class FactoryArgumentsToChoiceBinder<TParam1, TContract> : FactoryToChoiceBinder<TParam1, TContract>
    {
        public FactoryArgumentsToChoiceBinder(
            DiContainer bindContainer, BindInfo bindInfo, FactoryBindInfo factoryBindInfo)
            : base(bindContainer, bindInfo, factoryBindInfo)
        {
        }

        // We use generics instead of params object[] so that we preserve type info
        // So that you can for example pass in a variable that is null and the type info will
        // still be used to map null on to the correct field
        public FactoryToChoiceBinder<TParam1, TContract> WithFactoryArguments<T>(T param)
        {
            FactoryBindInfo.Arguments = InjectUtil.CreateArgListExplicit(param);
            return this;
        }

        public FactoryToChoiceBinder<TParam1, TContract> WithFactoryArguments<TFactoryParam1, TFactoryParam2>(TFactoryParam1 param1, TFactoryParam2 param2)
        {
            FactoryBindInfo.Arguments = InjectUtil.CreateArgListExplicit(param1, param2);
            return this;
        }

        public FactoryToChoiceBinder<TParam1, TContract> WithFactoryArguments<TFactoryParam1, TFactoryParam2, TFactoryParam3>(
            TFactoryParam1 param1, TFactoryParam2 param2, TFactoryParam3 param3)
        {
            FactoryBindInfo.Arguments = InjectUtil.CreateArgListExplicit(param1, param2, param3);
            return this;
        }

        public FactoryToChoiceBinder<TParam1, TContract> WithFactoryArguments<TFactoryParam1, TFactoryParam2, TFactoryParam3, TFactoryParam4>(
            TFactoryParam1 param1, TFactoryParam2 param2, TFactoryParam3 param3, TFactoryParam4 param4)
        {
            FactoryBindInfo.Arguments = InjectUtil.CreateArgListExplicit(param1, param2, param3, param4);
            return this;
        }

        public FactoryToChoiceBinder<TParam1, TContract> WithFactoryArguments<TFactoryParam1, TFactoryParam2, TFactoryParam3, TFactoryParam4, TFactoryParam5>(
            TFactoryParam1 param1, TFactoryParam2 param2, TFactoryParam3 param3, TFactoryParam4 param4, TFactoryParam5 param5)
        {
            FactoryBindInfo.Arguments = InjectUtil.CreateArgListExplicit(param1, param2, param3, param4, param5);
            return this;
        }

        public FactoryToChoiceBinder<TParam1, TContract> WithFactoryArguments<TFactoryParam1, TFactoryParam2, TFactoryParam3, TFactoryParam4, TFactoryParam5, TFactoryParam6>(
            TFactoryParam1 param1, TFactoryParam2 param2, TFactoryParam3 param3, TFactoryParam4 param4, TFactoryParam5 param5, TFactoryParam6 param6)
        {
            FactoryBindInfo.Arguments = InjectUtil.CreateArgListExplicit(param1, param2, param3, param4, param5, param6);
            return this;
        }

        public FactoryToChoiceBinder<TParam1, TContract> WithFactoryArguments(object[] args)
        {
            FactoryBindInfo.Arguments = InjectUtil.CreateArgList(args);
            return this;
        }

        public FactoryToChoiceBinder<TParam1, TContract> WithFactoryArgumentsExplicit(IEnumerable<TypeValuePair> extraArgs)
        {
            FactoryBindInfo.Arguments = extraArgs.ToList();
            return this;
        }
    }
}

