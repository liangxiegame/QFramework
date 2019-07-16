namespace Zenject
{
    //
    // I'd recommmend using Installer<> instead, and then always use the approach
    // of calling `MyInstaller.Install(Container)`
    // This way, if you want to add strongly typed parameters later you can do this
    // by deriving from a different Installer<> base class
    //
    public abstract class Installer : InstallerBase
    {
    }

    //
    // Derive from this class then install like this:
    //     FooInstaller.Install(Container);
    //
    public abstract class Installer<TDerived> : InstallerBase
        where TDerived : Installer<TDerived>
    {
        public static void Install(DiContainer container)
        {
            container.Instantiate<TDerived>().InstallBindings();
        }
    }

    // Use these versions to pass parameters to your installer

    public abstract class Installer<TParam1, TDerived> : InstallerBase
        where TDerived : Installer<TParam1, TDerived>
    {
        public static void Install(DiContainer container, TParam1 p1)
        {
            container.InstantiateExplicit<TDerived>(
                InjectUtil.CreateArgListExplicit(p1)).InstallBindings();
        }
    }

    public abstract class Installer<TParam1, TParam2, TDerived> : InstallerBase
        where TDerived : Installer<TParam1, TParam2, TDerived>
    {
        public static void Install(DiContainer container, TParam1 p1, TParam2 p2)
        {
            container.InstantiateExplicit<TDerived>(
                InjectUtil.CreateArgListExplicit(p1, p2)).InstallBindings();
        }
    }

    public abstract class Installer<TParam1, TParam2, TParam3, TDerived> : InstallerBase
        where TDerived : Installer<TParam1, TParam2, TParam3, TDerived>
    {
        public static void Install(DiContainer container, TParam1 p1, TParam2 p2, TParam3 p3)
        {
            container.InstantiateExplicit<TDerived>(
                InjectUtil.CreateArgListExplicit(p1, p2, p3)).InstallBindings();
        }
    }

    public abstract class Installer<TParam1, TParam2, TParam3, TParam4, TDerived> : InstallerBase
        where TDerived : Installer<TParam1, TParam2, TParam3, TParam4, TDerived>
    {
        public static void Install(DiContainer container, TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4)
        {
            container.InstantiateExplicit<TDerived>(
                InjectUtil.CreateArgListExplicit(p1, p2, p3, p4)).InstallBindings();
        }
    }

    public abstract class Installer<TParam1, TParam2, TParam3, TParam4, TParam5, TDerived> : InstallerBase
        where TDerived : Installer<TParam1, TParam2, TParam3, TParam4, TParam5, TDerived>
    {
        public static void Install(DiContainer container, TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5)
        {
            container.InstantiateExplicit<TDerived>(
                InjectUtil.CreateArgListExplicit(p1, p2, p3, p4, p5)).InstallBindings();
        }
    }
}
