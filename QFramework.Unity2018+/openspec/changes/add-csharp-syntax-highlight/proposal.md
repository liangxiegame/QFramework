## Why

PackageKit 内置 Markdown 渲染器（"用户手册" / Guideline 面板）的代码块当前用一个粗糙的 `string.Replace` 穷举关键字方案做"伪高亮"。它存在多个真实 bug：行首的关键字（`public`/`private`/`internal`/`protected`/`namespace`）因前后无空格而漏匹配；注释和字符串里出现关键字会被误涂，甚至会嵌套破坏已生成的 `<color>` 标签；`Split("@\n@".ToCharArray())` 实为按 `@` 或 `\n` 任一字符切割，遇到 C# 的 verbatim `@`（如 `@class`、`@"..."`）会破坏代码。同时覆盖面只有 9 个关键字、2 种颜色，文档中 270 个代码块（99% 为 C#）的高亮既残缺又不可靠。需要一个正确的 tokenizer 从根上解决。

## What Changes

- 新增轻量 C# tokenizer（手写字符级状态机），将代码切成 `keyword` / `string` / `comment` / `number` / `type` / `identifier` / `operator` / `whitespace` 等 token；在 `comment` / `string` 状态下不识别关键字，从根上杜绝误伤。
- 按 token 类型着色，配色沿用现状的 VS Dark 基调（`#4EC9B0` type、`#57A64A` comment），补齐 `keyword` / `string` / `number` 全套；颜色集中到一个静态配置表，不再散落硬编码。
- 替换 `MDRendererMarkdown.WriteCode()` 中的 `string.Replace` 穷举逻辑，改为调用 tokenizer 产出带 `<color>` 标签的富文本；保留现有的"逐行 `Text()` + `Layout.NewLine()`"布局方式。
- 根据 `FencedCodeBlock.Info` 识别语言：`csharp` / `cs` 走 C# tokenizer，**无语言标识的代码块默认按 C# 处理**（文档 149 个无标识块绝大多数即 C#），其它语言原样渲染（保留扩展点）。
- 复用已开启的 `style.richText` 通道（`MDStyleConverter.Apply()`），不改动样式与布局核心。

## Capabilities

### New Capabilities

- `markdown-code-highlighting`: Markdown 渲染器对 fenced 代码块按语法 token 着色的能力（本期仅实现 C#，预留多语言扩展点）。

### Modified Capabilities

<!-- 无现有 spec,openspec/specs/ 为空 -->

## Impact

- **新增**：`Assets/QFramework/Toolkits/_CoreKit/PackageKit/Markdown/Highlight/` 下新增 C# tokenizer 与配色配置（不单独成 Kit，YAGNI；仅此一处消费者）。
- **修改**：`MDRendererMarkdown.cs` 的 `WriteCode()`、`MDRendererBlockCode.cs` 的语言识别分支。
- **不动**：`MDStyleConverter.cs`（`richText = true` 已就绪）、`MDLayout` / `MDContent*`（逐行渲染不变）、`Markdig.dll`（仅做 Markdown 解析）。
- **范围**：仅影响编辑器（全部代码处于 `#if UNITY_EDITOR`），不影响运行时；无新增外部依赖。
