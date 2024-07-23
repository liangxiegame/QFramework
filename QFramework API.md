- - QFramework API 文档

    

    ## 1. 架构（Architecture）

    

    ### 1.1 IArchitecture 接口

    

    - **描述：** 定义了架构的基本功能，包括注册、获取和发送命令、查询、事件以及反初始化。
    - **方法：**
      - `void RegisterSystem<T>(T system) where T : ISystem`：注册一个系统。
      - `void RegisterModel<T>(T model) where T : IModel`：注册一个模型。
      - `void RegisterUtility<T>(T utility) where T : IUtility`：注册一个工具类。
      - `T GetSystem<T>() where T : class, ISystem`：获取一个系统。
      - `T GetModel<T>() where T : class, IModel`：获取一个模型。
      - `T GetUtility<T>() where T : class, IUtility`：获取一个工具类。
      - `void SendCommand<T>(T command) where T : ICommand`：发送一个无返回值的命令。
      - `TResult SendCommand<TResult>(ICommand<TResult> command)`：发送一个有返回值的命令。
      - `TResult SendQuery<TResult>(IQuery<TResult> query)`：发送一个查询。
      - `void SendEvent<T>() where T : new()`：发送一个无参构造函数的事件。
      - `void SendEvent<T>(T e)`：发送一个事件。
      - `IUnRegister RegisterEvent<T>(Action<T> onEvent)`：注册一个事件监听器。
      - `void UnRegisterEvent<T>(Action<T> onEvent)`：移除一个事件监听器。
      - `void Deinit()`：反初始化架构。

    

    ### 1.2 Architecture<T> 抽象类

    

    - **描述：** 架构的抽象基类，实现了 IArchitecture 接口。
    - **类型参数：**
      - `T`：具体的架构类型，必须继承自 Architecture<T>。
    - **字段：**
      - `public static Action<T> OnRegisterPatch`：注册补丁时的回调函数。
      - `protected static T mArchitecture`：架构的单例实例。
      - `public static IArchitecture Interface`：获取架构的接口。
    - **方法：**
      - `protected abstract void Init()`：初始化架构，子类必须实现。
      - `public void Deinit()`：反初始化架构。
      - `protected virtual void OnDeinit()`：反初始化时的回调函数，子类可重写。
    - **受保护的成员：**
      - `private IOCContainer mContainer`：用于管理依赖注入的容器。
      - `protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command)`：执行有返回值的命令。
      - `protected virtual void ExecuteCommand(ICommand command)`：执行无返回值的命令。
      - `protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)`：执行查询。
      - `private TypeEventSystem mTypeEventSystem`：类型事件系统，用于管理事件的发送和接收。

    

    ### 1.3 IOnEvent<T> 接口

    

    - **描述：** 定义了事件处理方法的接口。
    - **类型参数：**
      - `T`：事件类型。
    - **方法：**
      - `void OnEvent(T e)`：事件处理方法。

    

    ### 1.4 OnGlobalEventExtension 静态类

    

    - **描述：** 提供了 IOnEvent<T> 接口的扩展方法，用于注册和移除全局事件监听器。
    - **方法：**
      - `public static IUnRegister RegisterEvent<T>(this IOnEvent<T> self) where T : struct`：注册一个全局事件监听器。
      - `public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct`：移除一个全局事件监听器。

    

    ## 2. 控制器（Controller）

    

    ### 2.1 IController 接口

    

    - **描述：** 定义了控制器的基本功能，继承自多个接口，包括 IBelongToArchitecture、ICanSendCommand、ICanGetSystem、ICanGetModel、ICanRegisterEvent、ICanSendQuery 和 ICanGetUtility。

    

    ## 3. 系统（System）

    

    ### 3.1 ISystem 接口

    

    - **描述：** 定义了系统的基本功能，继承自多个接口，包括 IBelongToArchitecture、ICanSetArchitecture、ICanGetModel、ICanGetUtility、ICanRegisterEvent、ICanSendEvent、ICanGetSystem 和 ICanInit。

    

    ### 3.2 AbstractSystem 抽象类

    

    - **描述：** 系统的抽象基类，实现了 ISystem 接口。
    - **字段：**
      - `public bool Initialized { get; set; }`：指示系统是否已初始化。
    - **方法：**
      - `protected abstract void OnInit()`：初始化系统，子类必须实现。
      - `public void Deinit()`：反初始化系统。
      - `protected virtual void OnDeinit()`：反初始化时的回调函数，子类可重写。

    

    ## 4. 模型（Model）

    

    ### 4.1 IModel 接口

    

    - **描述：** 定义了模型的基本功能，继承自多个接口，包括 IBelongToArchitecture、ICanSetArchitecture、ICanGetUtility、ICanSendEvent 和 ICanInit。

    

    ### 4.2 AbstractModel 抽象类

    

    - **描述：** 模型的抽象基类，实现了 IModel 接口。
    - **字段：**
      - `public bool Initialized { get; set; }`：指示模型是否已初始化。
    - **方法：**
      - `protected abstract void OnInit()`：初始化模型，子类必须实现。
      - `public void Deinit()`：反初始化模型。
      - `protected virtual void OnDeinit()`：反初始化时的回调函数，子类可重写。

    

    ## 5. 工具类（Utility）

    

    ### 5.1 IUtility 接口

    

    - **描述：** 定义了工具类的基本功能，没有任何方法。

    

    ## 6. 命令（Command）

    

    ### 6.1 ICommand 接口

    

    - **描述：** 定义了无返回值命令的基本功能，继承自多个接口，包括 IBelongToArchitecture、ICanSetArchitecture、ICanGetSystem、ICanGetModel、ICanGetUtility、ICanSendEvent、ICanSendCommand 和 ICanSendQuery。
    - **方法：**
      - `void Execute()`：执行命令。

    

    ### 6.2 ICommand<TResult> 接口

    

    - **描述：** 定义了有返回值命令的基本功能，继承自多个接口，包括 IBelongToArchitecture、ICanSetArchitecture、ICanGetSystem、ICanGetModel、ICanGetUtility、ICanSendEvent、ICanSendCommand 和 ICanSendQuery。
    - **类型参数：**
      - `TResult`：命令执行结果的类型。
    - **方法：**
      - `TResult Execute()`：执行命令并返回结果。

    

    ### 6.3 AbstractCommand 抽象类

    

    - **描述：** 无返回值命令的抽象基类，实现了 ICommand 接口。
    - **方法：**
      - `protected abstract void OnExecute()`：执行命令的具体逻辑，子类必须实现。

    

    ### 6.4 AbstractCommand<TResult> 抽象类

    

    - **描述：** 有返回值命令的抽象基类，实现了 ICommand<TResult> 接口。
    - **类型参数：**
      - `TResult`：命令执行结果的类型。
    - **方法：**
      - `protected abstract TResult OnExecute()`：执行命令的具体逻辑并返回结果，子类必须实现。

    

    ## 7. 查询（Query）

    

    ### 7.1 IQuery<TResult> 接口

    

    - **描述：** 定义了查询的基本功能，继承自多个接口，包括 IBelongToArchitecture、ICanSetArchitecture、ICanGetModel、ICanGetSystem 和 ICanSendQuery。
    - **类型参数：**
      - `TResult`：查询结果的类型。
    - **方法：**
      - `TResult Do()`：执行查询并返回结果。

    

    ### 7.2 AbstractQuery<T> 抽象类

    

    - **描述：** 查询的抽象基类，实现了 IQuery<T> 接口。
    - **类型参数：**
      - `T`：查询结果的类型。
    - **方法：**
      - `protected abstract T OnDo()`：执行查询的具体逻辑并返回结果，子类必须实现。

    

    ## 8. 规则（Rule）

    

    ### 8.1 IBelongToArchitecture 接口

    

    - **描述：** 定义了属于架构的类型的基本功能。
    - **方法：**
      - `IArchitecture GetArchitecture()`：获取所属的架构。

    

    ### 8.2 ICanSetArchitecture 接口

    

    - **描述：** 定义了可以设置架构的类型的基本功能。
    - **方法：**
      - `void SetArchitecture(IArchitecture architecture)`：设置所属的架构。

    

    ### 8.3 ICanGetModel 接口

    

    - **描述：** 定义了可以获取模型的类型的基本功能，继承自 IBelongToArchitecture 接口。

    

    ### 8.4 CanGetModelExtension 静态类

    

    - **描述：** 提供了 ICanGetModel 接口的扩展方法，用于获取模型。
    - **方法：**
      - `public static T GetModel<T>(this ICanGetModel self) where T : class, IModel`：获取指定类型的模型。

    

    ### 8.5 ICanGetSystem 接口

    

    - **描述：** 定义了可以获取系统的类型的基本功能，继承自 IBelongToArchitecture 接口。

    

    ### 8.6 CanGetSystemExtension 静态类

    

    - **描述：** 提供了 ICanGetSystem 接口的扩展方法，用于获取系统。
    - **方法：**
      - `public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem`：获取指定类型的系统。

    

    ### 8.7 ICanGetUtility 接口

    

    - **描述：** 定义了可以获取工具类的类型的基本功能，继承自 IBelongToArchitecture 接口。

    

    ### 8.8 CanGetUtilityExtension 静态类

    

    - **描述：** 提供了 ICanGetUtility 接口的扩展方法，用于获取工具类。
    - **方法：**
      - `public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility`：获取指定类型的工具类。

    

    ### 8.9 ICanRegisterEvent 接口

    

    - **描述：** 定义了可以注册事件监听器的类型的基本功能，继承自 IBelongToArchitecture 接口。

    

    ### 8.10 CanRegisterEventExtension 静态类

    

    - **描述：** 提供了 ICanRegisterEvent 接口的扩展方法，用于注册和移除事件监听器。
    - **方法：**
      - `public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)`：注册一个事件监听器。
      - `public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent)`：移除一个事件监听器。

    

    ### 8.11 ICanSendCommand 接口

    

    - **描述：** 定义了可以发送命令的类型的基本功能，继承自 IBelongToArchitecture 接口。

    

    ### 8.12 CanSendCommandExtension 静态类

    

    - **描述：** 提供了 ICanSendCommand 接口的扩展方法，用于发送命令。
    - **方法：**
      - `public static void SendCommand<T>(this ICanSendCommand self) where T : ICommand, new()`：发送一个无参构造函数的命令。
      - `public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand`：发送一个命令。
      - `public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command)`：发送一个有返回值的命令。

    

    ### 8.13 ICanSendEvent 接口

    

    - **描述：** 定义了可以发送事件的类型的基本功能，继承自 IBelongToArchitecture 接口。

    

    ### 8.14 CanSendEventExtension 静态类

    

    - **描述：** 提供了 ICanSendEvent 接口的扩展方法，用于发送事件。
    - **方法：**
      - `public static void SendEvent<T>(this ICanSendEvent self) where T : new()`：发送一个无参构造函数的事件。
      - `public static void SendEvent<T>(this ICanSendEvent self, T e)`：发送一个事件。

    

    ### 8.15 ICanSendQuery 接口

    

    - **描述：** 定义了可以发送查询的类型的基本功能，继承自 IBelongToArchitecture 接口。

    

    ### 8.16 CanSendQueryExtension 静态类

    

    - **描述：** 提供了 ICanSendQuery 接口的扩展方法，用于发送查询。
    - **方法：**
      - `public static TResult SendQuery<TResult>(this ICanSendQuery self, IQuery<TResult> query)`：发送一个查询。

    

    ### 8.17 ICanInit 接口

    

    - **描述：** 定义了可以初始化的类型的基本功能。
    - **字段：**
      - `bool Initialized { get; set; }`：指示是否已初始化。
    - **方法：**
      - `void Init()`：初始化。
      - `void Deinit()`：反初始化。

    

    ## 9. 类型事件系统（TypeEventSystem）

    

    ### 9.1 IUnRegister 接口

    

    - **描述：** 定义了取消注册操作的基本功能。
    - **方法：**
      - `void UnRegister()`：取消注册。

    

    ### 9.2 IUnRegisterList 接口

    

    - **描述：** 定义了取消注册列表的基本功能。
    - **属性：**
      - `List<IUnRegister> UnregisterList { get; }`：取消注册列表。

    

    ### 9.3 IUnRegisterListExtension 静态类

    

    - **描述：** 提供了 IUnRegisterList 接口的扩展方法，用于管理取消注册列表。
    - **方法：**
      - `public static void AddToUnregisterList(this IUnRegister self, IUnRegisterList unRegisterList)`：将一个取消注册操作添加到列表中。
      - `public static void UnRegisterAll(this IUnRegisterList self)`：取消注册列表中的所有操作。

    

    ### 9.4 CustomUnRegister 结构体

    

    - **描述：** 自定义的取消注册操作，实现了 IUnRegister 接口。
    - **构造函数：**
      - `public CustomUnRegister(Action onUnRegister)`：创建一个新的 CustomUnRegister 实例。

    

    ### 9.5 UnRegisterTrigger 抽象类

    

    - **描述：** 取消注册触发器的基类，继承自 UnityEngine.MonoBehaviour。
    - **方法：**
      - `public IUnRegister AddUnRegister(IUnRegister unRegister)`：添加一个取消注册操作。
      - `public void RemoveUnRegister(IUnRegister unRegister)`：移除一个取消注册操作。
      - `public void UnRegister()`：取消注册所有操作。

    

    ### 9.6 UnRegisterOnDestroyTrigger 类

    

    - **描述：** 在游戏对象销毁时取消注册的触发器，继承自 UnRegisterTrigger。

    

    ### 9.7 UnRegisterOnDisableTrigger 类

    

    - **描述：** 在游戏对象禁用时取消注册的触发器，继承自 UnRegisterTrigger。

    

    ### 9.8 UnRegisterCurrentSceneUnloadedTrigger 类

    

    - **描述：** 在当前场景卸载时取消注册的触发器，继承自 UnRegisterTrigger。
    - **属性：**
      - `public static UnRegisterCurrentSceneUnloadedTrigger Get`：获取 UnRegisterCurrentSceneUnloadedTrigger 的实例。

    

    ### 9.9 UnRegisterExtension 静态类

    

    - **描述：** 提供了 IUnRegister 接口的扩展方法，用于在特定情况下取消注册。

    

    ### 9.10 TypeEventSystem 类

    

    - **描述：** 类型事件系统，用于发送和接收事件。
    - **字段：**
      - `public static readonly TypeEventSystem Global`：全局的 TypeEventSystem 实例。
    - **方法：**
      - `public void Send<T>() where T : new()`：发送一个无参构造函数的事件。
      - `public void Send<T>(T e)`：发送一个事件。
      - `public IUnRegister Register<T>(Action<T> onEvent)`：注册一个事件监听器。
      - `public void UnRegister<T>(Action<T> onEvent)`：移除一个事件监听器。

    

    ## 10. 依赖注入容器（IOC）

    

    ### 10.1 IOCContainer 类

    

    - **描述：** 依赖注入容器，用于管理依赖关系。
    - **方法：**
      - `public void Register<T>(T instance)`：注册一个实例。
      - `public T Get<T>() where T : class`：获取一个实例。
      - `public IEnumerable<T> GetInstancesByType<T>()`：获取指定类型的所有实例。
      - `public void Clear()`：清空容器。

    

    ## 11. 可绑定属性（BindableProperty）

    

    ### 11.1 IBindableProperty<T> 接口

    

    - **描述：** 定义了可绑定属性的基本功能，继承自 IReadonlyBindableProperty<T> 接口。
    - **类型参数：**
      - `T`：属性值的类型。
    - **属性：**
      - `new T Value { get; set; }`：属性值。
    - **方法：**
      - `void SetValueWithoutEvent(T newValue)`：设置属性值，但不触发事件。

    

    ### 11.2 IReadonlyBindableProperty<T> 接口

    

    - **描述：** 定义了只读可绑定属性的基本功能，继承自 IEasyEvent 接口。
    - **类型参数：**
      - `T`：属性值的类型。
    - **属性：**
      - `T Value { get; }`：属性值。
    - **方法：**
      - `IUnRegister RegisterWithInitValue(Action<T> action)`：注册一个事件监听器，并在注册时触发一次事件。
      - `void UnRegister(Action<T> onValueChanged)`：移除一个事件监听器。
      - `IUnRegister Register(Action<T> onValueChanged)`：注册一个事件监听器。

    

    ### 11.3 BindableProperty<T> 类

    

    - **描述：** 可绑定属性的实现类，实现了 IBindableProperty<T> 接口。
    - **类型参数：**
      - `T`：属性值的类型。
    - **字段：**
      - `public static Func<T, T, bool> Comparer { get; set; }`：用于比较两个属性值是否相等的比较器。
    - **构造函数：**
      - `public BindableProperty(T defaultValue = default)`：创建一个新的 BindableProperty 实例。
      - `public BindableProperty<T> WithComparer(Func<T, T, bool> comparer)`：创建一个新的 BindableProperty 实例，并指定比较器。

    

    ### 11.4 ComparerAutoRegister 类

    

    - **描述：** 用于自动注册常用类型的比较器的类。

    

    ## 12. 简单事件（EasyEvent）

    

    ### 12.1 IEasyEvent 接口

    

    - **描述：** 定义了简单事件的基本功能。
    - **方法：**
      - `IUnRegister Register(Action onEvent)`：注册一个事件监听器。

    

    ### 12.2 EasyEvent 类

    

    - **描述：** 无参数简单事件的实现类，实现了 IEasyEvent 接口。
    - **方法：**
      - `public IUnRegister Register(Action onEvent)`：注册一个事件监听器。
      - `public void UnRegister(Action onEvent)`：移除一个事件监听器。
      - `public void Trigger()`：触发事件。

    

    ### 12.3 EasyEvent<T> 类

    

    - **描述：** 单参数简单事件的实现类，实现了 IEasyEvent 接口。
    - **类型参数：**
      - `T`：事件参数的类型。
    - **方法：**
      - `public IUnRegister Register(Action<T> onEvent)`：注册一个事件监听器。
      - `public void UnRegister(Action<T> onEvent)`：移除一个事件监听器。
      - `public void Trigger(T t)`：触发事件。

    

    ### 12.4 EasyEvent<T, K> 类

    

    - **描述：** 双参数简单事件的实现类，实现了 IEasyEvent 接口。
    - **类型参数：**
      - `T`：第一个事件参数的类型。
      - `K`：第二个事件参数的类型。
    - **方法：**
      - `public IUnRegister Register(Action<T, K> onEvent)`：注册一个事件监听器。
      - `public void UnRegister(Action<T, K> onEvent)`：移除一个事件监听器。
      - `public void Trigger(T t, K k)`：触发事件。

    

    ### 12.5 EasyEvent<T, K, S> 类

    

    - **描述：** 三参数简单事件的实现类，实现了 IEasyEvent 接口。
    - **类型参数：**
      - `T`：第一个事件参数的类型。
      - `K`：第二个事件参数的类型。
      - `S`：第三个事件参数的类型。
    - **方法：**
      - `public IUnRegister Register(Action<T, K, S> onEvent)`：注册一个事件监听器。
      - `public void UnRegister(Action<T, K, S> onEvent)`：移除一个事件监听器。
      - `public void Trigger(T t, K k, S s)`：触发事件。

    

    ### 12.6 EasyEvents 类

    

    - **描述：** 用于管理多个简单事件的类。
    - **字段：**
      - `public static readonly EasyEvents mGlobalEvents`：全局的 EasyEvents 实例。
    - **方法：**
      - `public static T Get<T>() where T : IEasyEvent`：获取一个简单事件。
      - `public static void Register<T>() where T : IEasyEvent, new()`：注册一个简单事件。
      - `public void AddEvent<T>() where T : IEasyEvent, new()`：添加一个简单事件。
      - `public T GetEvent<T>() where T : IEasyEvent`：获取一个简单事件。
      - `public T GetOrAddEvent<T>() where T : IEasyEvent, new()`：获取或添加一个简单事件。

    

    ## 13. 事件扩展（Event Extension）

    

    ### 13.1 OrEvent 类

    

    - **描述：** 用于组合多个事件