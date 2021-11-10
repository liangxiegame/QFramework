- 1.Hotfix中不要写大量复杂的计算,特别是在Update之类的方法中.
- 2.Hotfix中调用泛型需要写重定向，参考ILRuntimeRedirectHelper中的各个注册
- 3.Hotfix对多线程Thread不兼容,使用会导致Unity崩溃闪退
- 4.Hotfix中重写Unity中的虚函数时,不能再调base.xxx(),否则会爆栈,也就是StackOverflow
- 5.Hotfix中不支持unsafe,intptr,interup
- 6.Unity调用Hotfix的方法,开销相对较大,尽量避免.
- 7.大部分情况下，Struct会比class慢，还会有频繁装箱的问题
- 8.不支持复杂的可空值类型
- 10.使用for代替foreach，使用ForDic代替Dic
- 11.主项目中 创建实例化和GetType使用 ReflectionHelper 中的方法
- 12.尽量使用Action以及Func这两个系统内置万用委托类型
- 13.获取属性使用type.GetCustomAttributes(Typeof(Attr), false);
- 14.不要使用Get(Add)Component(Type)来操作组件
- 15.使用[][]代替 [,]
- 16.跨域继承不能多继承  如之前的Army跨域继承了Mono，如果再继承IDraggable，List<IDraggable>是不能识别的，所以需要热更里有一个类
- 继承Mono的同时实现IDraggable接口，Army继承DraggableMono，使用List<DraggableMono> 来接Army
- 17.不能使用ref/out  例如TryGetComponent
- 18.使用Release编译的dll运行，需要生成clr绑定并初始化
- 19.热更中适当使用枚举，不要做过多操作
- 20.Attribute的构造函数只能使用基本类型，不能使用params 数组
- 21.热更中字典如果使用继承自主项目的类做值，不能使用tryGetValue
- 22.继承mono的，不能在构造参数设置值或者给默认值，只能在awake中
- 23.跨域继承的子类无法调用当前重载以外的虚函数，跨域继承中不能在基类的构造函数中调用该类的虚函数


- IL2CPP打包注意：
    - 会自动开启代码剔除
    - 使用CLR自动分析绑定可最大化避免裁剪
    - 适当使用link.xml来预留接口和类型
    - 尤其注意泛型方法和类型


- 影响执行效率的因素：
    - 使用Debug模式编译会比Release模式编译dll慢数倍
    - 编辑器内执行会比打包慢数倍
    - 使用DevelopmentBuild打包出来会慢数倍
    - IL2CPP的C++编译选项Release之外的其他选项也会慢
    - 没有使用CLR绑定
    - 没有注册值类型绑定


- 如何定位热更性能问题
  - Profiler
  - 由于方法是懒加载，在第一次执行的时候会读取方法有什么指令，会有额外开销，如果是性能关键的方法，可通过预热解决(Appdomain.Prewarm)
  - 编辑器中每次调用热更方法会有20字节的格外gc alloc，用于断点和行号，添加DISABLE_ILRUNTIME_DEBUG宏可关闭，打包出来不影响


- 如何排查热更中的gc alloc
  - Profiler
  - 对比非热更情况下的开销，正常情况下热更和非热更的gc应该是一致
  - clr绑定
  - 值类型绑定gc开销问题
  - 当你把方法中的vector3赋值给成员变量时，一定会产生gc，可以使用
  ```c#
  void aa()
  {
  //field为成员变量
  vector3 temp = field;
  for(i = 0, i < 100, i++)
  {
    temp *= 2;
  }
  field = temp;
  //只会在最后一步赋值的时候产生gc   
  
  //如果是下面这样，每一次赋值都会产生gc
  for(i = 0, i < 100, i++)
  {
    field *= 2;
  }
  }
  ```
  - 编辑器禁用调试 DISABLE_ILRUNTIME_DEBUG

  
- 如何使用寄存器模式
  - 2.0才有
  - 大幅提升数值计算性能，优化频繁调用小方法的开销
  - 通过AppDomain构造函数指定运行模式
    - 1.None
    - 2.JITOnDemand（按需编译，如果一个方法频繁调用了多少次以上，就会认为需要走寄存器，在后台多线程会编译成寄存器模式调用）
    - 3.JITImmediately（立即编译，调用所有方法都会同步编译成寄存器模式，对于有些不必要的消耗可能会增加）
    - 所以一般推荐第二种
  - 可通过`[ILRuntimeJIT](ILRuntimeJITFlag.JITImmediately)`对方法或一个类型下所有方法进行重载
  - 如方法内无数值计算，也没有频繁调用热更内的小方法，而以调用主工程方法为主，寄存器模式无法对其进行优化，甚至有可能比常规模式略慢

- 内存占用方面优化
  - 编辑器不准！在真机看
  - ILR对所有类型进行了包装，基础类型长度均为12字节，无论使用byte还是short，所以配置表全部读入会占用大量内存
  - 可以使用Sqlite存储配置随时取用
    