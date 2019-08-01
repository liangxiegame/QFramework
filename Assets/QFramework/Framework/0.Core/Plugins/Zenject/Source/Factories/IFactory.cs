namespace Zenject
{
    public interface IFactory
    {
    }

    public interface IFactory<out TValue> : IFactory
    {
        TValue Create();
    }

    public interface IFactory<in TParam1, out TValue> : IFactory
    {
        TValue Create(TParam1 param);
    }

    public interface IFactory<in TParam1, in TParam2, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, in TParam4, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, in TParam8, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, in TParam8, in TParam9, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, in TParam8, in TParam9, in TParam10, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9, TParam10 param10);
    }
}

