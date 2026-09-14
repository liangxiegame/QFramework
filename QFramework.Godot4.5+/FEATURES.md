# QFramework.GDScript 产品文档

## 产品概述

QFramework.GDScript 是一个为 Godot 4.5 设计的轻量级游戏开发框架，提供 MVVM/CQRS 风格的架构模式，帮助开发者构建结构清晰、易于维护的游戏项目。

**版本**: v0.3.107  
**作者**: liangxiegame  
**许可证**: MIT

---

## 核心特性

### 1. 分层架构 (MVVM/CQRS)

QFramework 采用清晰的分层架构，将游戏逻辑分离为独立的职责单元：

| 层级 | 职责 | 对应类 |
|------|------|--------|
| **Model** | 数据状态管理 | `AbstractModel` |
| **System** | 业务逻辑处理 | `AbstractSystem` |
| **Utility** | 跨层工具服务 | `AbstractUtility` |
| **Command** | 写操作（修改数据） | `AbstractCommand` |
| **Query** | 读操作（查询数据） | `AbstractQuery` |
| **Architecture** | 架构协调器 | `Architecture` |

**优势**:
- 关注点分离，代码易于理解和维护
- 支持单元测试，各层可独立验证
- 避免紧耦合，便于团队协作

### 2. CoreKit 工具集

#### EasyEvent - 轻量级事件系统

```gdscript
var event = EasyEvent.new()

# 注册监听
event.register(func(msg):
    print("收到消息: " + msg)
)

# 触发事件
event.trigger("Hello World")
```

**特性**:
- 自动生命周期管理（节点退出时自动注销）
- 支持带初始值的注册
- 类型安全

#### BindableProperty - 响应式属性

```gdscript
var hp = BindableProperty.new(100)

# 监听变化
hp.register(func(new_hp):
    print("血量变化: " + str(new_hp))
)

# 修改值（自动触发事件）
hp.value = 80
```

**应用场景**: UI数据绑定、状态同步、配置热更新

#### EasyFSM - 有限状态机

```gdscript
var fsm = EasyFSM.new()

# 定义状态
fsm.state(IDLE)
    .on_enter(func(): play_idle_anim())
    .on_process(func(dt): check_input())
    .on_exit(func(): stop_idle_anim())

fsm.state(RUN)
    .on_enter(func(): play_run_anim())
    .on_physics(func(dt): move_character(dt))

# 启动
fsm.start_state(IDLE)

# 切换状态
fsm.change_state(RUN)
```

**特性**:
- 流畅的链式 API
- 支持 Enter/Exit/Process/PhysicsProcess
- 自动计时器

#### FluentUI - 流式 UI 构建器

```gdscript
(FluentUI.vbox()
    .child(FluentUI.label().text("标题").font_size(24))
    .child(FluentUI.button()
        .text("点击我")
        .pressed(func(): print("clicked"))
    )
    .child(FluentUI.hbox()
        .child(FluentUI.line_edit().placeholder("输入..."))
        .child(FluentUI.button().text("提交"))
    )
    .build()
)
```

**支持组件**: Panel、VBox、HBox、Grid、Label、Button、LineEdit、TextEdit、CheckBox、Separator 等

### 3. 编辑器集成

QFramework 提供 Godot 编辑器插件，支持：

- **一键更新**: 自动检测并更新框架版本
- **代码生成**: 快速创建 Command、Query、System、Model、Utility、Architecture
- **扩展插件**: 可选安装背包、对话、任务等官方插件

**快捷键**: `Ctrl/Cmd + E` 快速打开 QFramework 面板

---

## 快速开始

### 安装

1. 下载 QFramework 插件
2. 复制到 `addons/qframework/` 目录
3. 在 Godot 编辑器中启用插件：`项目 → 项目设置 → 插件`

### 创建第一个 Architecture

```gdscript
# my_app.gd
class_name MyApp extends Architecture

func init():
    register_model(GameModel.new())
    register_system(GameSystem.new())
    register_utility(Storage.new())
```

```gdscript
# game_model.gd
class_name GameModel extends AbstractModel

var score = BindableProperty.new(0)
var player_name = BindableProperty.new("Player")

func get_model_name() -> String:
    return "GameModel"
```

```gdscript
# game_system.gd
class_name GameSystem extends AbstractSystem

func add_score(points: int):
    var model = get_architecture().get_model("GameModel") as GameModel
    model.score.value += points

func get_system_name() -> String:
    return "GameSystem"
```

```gdscript
# 使用
await MyApp.send_command(AddScoreCommand.new(10))
var score = await MyApp.send_query(GetScoreQuery.new())
```

---

## 架构指南

### Command 模式（写操作）

用于修改数据的操作，支持异步：

```gdscript
class_name AddScoreCommand extends AbstractCommand

var points: int

func _init(_points: int):
    points = _points

func execute():
    var system = get_system("GameSystem") as GameSystem
    system.add_score(points)
    return true
```

### Query 模式（读操作）

用于查询数据，不修改状态：

```gdscript
class_name GetScoreQuery extends AbstractQuery

func do() -> int:
    var model = get_model("GameModel") as GameModel
    return model.score.value
```

### Model 设计

- 使用 `BindableProperty` 包装需要监听的属性
- 保持 Model 纯粹，不包含业务逻辑
- 通过 `get_model_name()` 提供唯一标识

### System 设计

- 封装业务逻辑
- 可以访问 Model 和 Utility
- 通过 `get_system_name()` 提供唯一标识

### Utility 设计

- 提供跨层服务（存储、网络、配置等）
- 不依赖特定的 Model 或 System
- 通过 `get_utility_name()` 提供唯一标识

---

## 示例项目

### 1. CounterApp（计数器）

路径: `user_guide_code/05.architecture/01.counter_app/`

展示基础的 Architecture + Command + Model 用法。

### 2. QueryExample（查询示例）

路径: `user_guide_code/05.architecture/02.query_example_app/`

展示 Query 模式的使用。

### 3. EasyEvent 示例

路径: `user_guide_code/01.easy_event/`

包含 10 个事件系统使用示例。

### 4. BindableProperty 示例

路径: `user_guide_code/02.bindable_property/`

包含 6 个响应式属性使用示例。

### 5. EasyFSM 示例

路径: `user_guide_code/04.easy_fsm/`

包含 6 个状态机使用示例。

---

## 最佳实践

### 1. 命名规范

| 类型 | 命名规则 | 示例 |
|------|----------|------|
| 类名 | PascalCase | `PlayerModel`, `GameSystem` |
| 文件名 | snake_case | `player_model.gd`, `game_system.gd` |
| 函数/变量 | snake_case | `get_model()`, `player_name` |
| 常量 | UPPER_SNAKE_CASE | `MAX_HEALTH`, `STATE_IDLE` |

### 2. 项目结构

```
project/
├── addons/
│   └── qframework/          # 框架代码
├── src/
│   ├── architecture/        # Architecture 定义
│   ├── models/              # Model 层
│   ├── systems/             # System 层
│   ├── commands/            # Command 定义
│   ├── queries/             # Query 定义
│   ├── utilities/           # Utility 层
│   └── views/               # UI/场景脚本
└── project.godot
```

### 3. 生命周期管理

```gdscript
# 节点退出时自动注销事件
func _ready():
    model.score.register(on_score_changed) \
        .un_register_when_node_exiting_tree(self)
```

### 4. 错误处理

```gdscript
func execute():
    var model = get_model("GameModel") as GameModel
    if model == null:
        push_error("GameModel not found")
        return false
    # ...
```

---

## API 参考

### Architecture

| 方法 | 说明 |
|------|------|
| `register_model(model)` | 注册 Model |
| `register_system(system)` | 注册 System |
| `register_utility(utility)` | 注册 Utility |
| `get_model(name)` | 获取 Model |
| `get_system(name)` | 获取 System |
| `get_utility(name)` | 获取 Utility |
| `send_command(cmd)` | 执行 Command |
| `send_query(query)` | 执行 Query |

### AbstractModel

| 方法 | 说明 |
|------|------|
| `get_model_name()` | 返回 Model 唯一标识 |
| `init()` | 初始化回调 |

### AbstractSystem

| 方法 | 说明 |
|------|------|
| `get_system_name()` | 返回 System 唯一标识 |
| `init()` | 初始化回调 |

### AbstractCommand

| 方法 | 说明 |
|------|------|
| `execute()` | 执行命令 |
| `get_model(name)` | 访问 Model |
| `get_system(name)` | 访问 System |
| `get_utility(name)` | 访问 Utility |

### AbstractQuery

| 方法 | 说明 |
|------|------|
| `do()` | 执行查询并返回结果 |
| `get_model(name)` | 访问 Model |
| `get_system(name)` | 访问 System |
| `get_utility(name)` | 访问 Utility |

---

## 常见问题

### Q: 如何跨场景保持数据？

A: 将 Architecture 设为 Autoload（自动加载单例）。

### Q: Command 和 Query 的区别？

A: Command 用于修改数据（写），Query 用于查询数据（读），分离后更清晰。

### Q: 如何处理异步操作？

A: Command 和 Query 都支持 `await`，在 `execute()` 或 `do()` 中使用 `await` 即可。

### Q: 如何调试？

A: 使用 Godot 的打印输出，或添加断点到 Command/Query 的执行方法。

---

## 更新日志

### v0.3.107
- 优化编辑器插件稳定性
- 改进 FluentUI 性能

### v0.3.100
- 增加扩展插件系统
- 支持背包、对话、任务插件

### v0.3.0
- 初始版本发布
- 核心架构完成
- CoreKit 工具集

---

## 相关资源

- **GitHub**: https://github.com/liangxiegame/QFramework.GDScript
- **文档**: 见 `user_guide_code/` 示例代码
- **社区**: QQ群 xxxxxx

---

## 许可

MIT License - 可自由用于商业和非商业项目。
