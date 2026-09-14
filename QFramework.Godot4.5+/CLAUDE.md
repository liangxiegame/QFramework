# QFramework.GDScript

Godot 4.5 框架，提供 MVVM/CQRS 架构模式。

## 项目结构

- `addons/qframework/framework/` - 核心框架代码
- `user_guide_code/` - 示例代码（按功能编号组织）
- `playground/` - 实验/测试代码

## 架构组件

### 核心层
- `Architecture` - 架构基类，管理 Model/System/Utility 注册
- `AbstractModel` - 数据层
- `AbstractSystem` - 逻辑层
- `AbstractUtility` - 工具服务层
- `AbstractCommand` - 命令模式（写操作）
- `AbstractQuery` - 查询模式（读操作）

### CoreKit 工具
- `EasyEvent` - 事件系统
- `BindableProperty` - 响应式属性绑定
- `EasyFSM` - 有限状态机
- `FluentUI` - 流式 UI 构建器

## 代码约定

- 类名: PascalCase (`class_name CounterApp`)
- 文件名: snake_case (`counter_app.gd`)
- 函数/变量: snake_case (`get_model`, `send_command`)
- 注释: 中文

## 常用模式

### 创建 Architecture
```gdscript
class_name MyArchitecture extends Architecture

func init():
    register_system(MySystem.new())
    register_model(MyModel.new())
    register_utility(MyUtility.new())
```

### Model/System/Utility 基类继承
```gdscript
class_name MyModel extends AbstractModel
class_name MySystem extends AbstractSystem
class_name MyUtility extends AbstractUtility
```

### Command/Query 使用
```gdscript
# 执行命令
await architecture.send_command(MyCommand.new())

# 执行查询
var result = await architecture.send_query(MyQuery.new())
```

### FluentUI 构建
```gdscript
FluentUI.button().text("Click").pressed(func(): print("clicked"))
```

### 事件注册
```gdscript
event.register(callback_callable)
event.trigger(args)
```

## Godot 项目配置

- 入口配置: `project.godot`
- Autoloads: CounterApp, QueryExampleApp
- 启用插件: qframework, editor_counter_app

## 参考资源

- 示例代码: `user_guide_code/01.easy_event/` 到 `05.architecture/`
- 核心框架: `addons/qframework/framework/architecture/`
