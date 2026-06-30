## ADDED Requirements

### Requirement: C# 代码块语法 token 化

渲染器 SHALL 使用 tokenizer 将 C# 代码块内容切分为带类型的 token，至少包含 `keyword`、`string`、`comment`、`number`、`type`、`identifier`、`operator`、`whitespace`。tokenizer SHALL 在字符串与注释状态下不识别关键字。

#### Scenario: 行首关键字被识别

- **WHEN** 代码块包含位于行首、前后无空格的关键字（如 `public class X` 中的 `public`）
- **THEN** 该关键字被识别为 `keyword` token 并着色，不因缺少前导空格而漏匹配

#### Scenario: 注释内的关键字不被误伤

- **WHEN** 行注释（`//`、`///`）或块注释（`/* */`）内出现关键字字样（如 `// 一个 class`）
- **THEN** 整段注释按 `comment` 着色，其中的 `class` 不被识别为 `keyword`

#### Scenario: 字符串内的关键字不被误伤

- **WHEN** 字符串字面量内出现关键字字样（如 `"return void"`）
- **THEN** 字符串按 `string` 着色，其中的关键字不被识别为 `keyword`

#### Scenario: 多行块注释跨行保持状态

- **WHEN** 代码包含跨多行的 `/* ... */` 块注释
- **THEN** 跨行内容连续按 `comment` 着色，状态不因行边界而丢失

#### Scenario: 含 verbatim @ 的代码不被破坏

- **WHEN** 代码包含 C# verbatim 标识符（如 `@class`）或 verbatim 字符串（如 `@"..."`）
- **THEN** 代码被正确渲染，不被按 `@` 字符切割或破坏

### Requirement: 按 token 类型着色并集中配置

渲染器 SHALL 将每个 token 类型映射到对应颜色，生成 Unity IMGUI richText `<color>` 标签进行着色，覆盖至少 `keyword`、`string`、`comment`、`number`、`type` 五类。所有配色 SHALL 集中在单一静态配置表中。

#### Scenario: 五类 token 分别着色

- **WHEN** 渲染包含关键字、字符串、注释、数字、类型名的 C# 代码
- **THEN** 五类 token 分别按配置表中的对应颜色着色

#### Scenario: 配色集中可改

- **WHEN** 修改配置表中某一 token 类型的颜色
- **THEN** 该类型颜色在所有代码块中随之更新，无需改动 tokenizer 逻辑

### Requirement: 语言识别与默认行为

渲染器 SHALL 根据 fenced 代码块的语言标识（`FencedCodeBlock.Info`）决定高亮策略：`csharp` 与 `cs` 走 C# 高亮；无语言标识的代码块默认按 C# 高亮；其他语言原样渲染、不着色。

#### Scenario: csharp 与 cs 走 C# 高亮

- **WHEN** 代码块语言标识为 `csharp` 或 `cs`
- **THEN** 代码块经 C# tokenizer 着色

#### Scenario: 无语言标识默认按 C# 高亮

- **WHEN** 代码块没有语言标识
- **THEN** 代码块默认按 C# 高亮

#### Scenario: 其他语言原样渲染

- **WHEN** 代码块语言标识为 C# 以外的语言（如 `json`、`bash`）
- **THEN** 代码块内容以等宽字体原样渲染，不进行语法着色

### Requirement: 复用现有布局与样式通道

渲染器 SHALL 保留现有"逐行 `Text()` + `Layout.NewLine()`"的布局方式，并依赖既有 `style.richText = true` 通道使 `<color>` 标签生效。实现 SHALL NOT 改动 `MDStyleConverter` 的样式契约，也 SHALL NOT 改动 `MDLayout` 的布局契约。

#### Scenario: 逐行布局保持不变

- **WHEN** 渲染多行代码块
- **THEN** 渲染结果逐行换行，行数与源代码行数一致

#### Scenario: 复用 richText 通道

- **WHEN** 生成带 `<color>` 标签的代码行
- **THEN** 经 `MDStyleConverter` 渲染为多色文本，且不新增 GUIStyle 或样式条目

### Requirement: 不引入运行时依赖

高亮实现 SHALL 完全位于 `#if UNITY_EDITOR` 之内，且 SHALL NOT 新增任何外部程序集或 dll 依赖。

#### Scenario: 仅参与编辑器编译

- **WHEN** 进行运行时（非编辑器）编译
- **THEN** 语法高亮相关代码不参与编译

#### Scenario: 不新增依赖

- **WHEN** 进行编译
- **THEN** 除既有 `Markdig.dll` 外，不引入新的 dll 或包依赖
