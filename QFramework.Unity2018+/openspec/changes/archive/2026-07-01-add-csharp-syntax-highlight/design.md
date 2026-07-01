## Context

PackageKit 的"用户手册"（Guideline 面板）用一套基于 Markdig + 自研 IMGUI 布局的 Markdown 渲染器（`PackageKit/Markdown/`）展示文档。代码块由 `MDRendererBlockCode` → `MDRendererMarkdown.WriteCode()` 负责绘制。

现状的 `WriteCode()` 用一连串 `string.Replace` 穷举 9 个关键字 + `//` / `///`，靠 `MDStyleConverter.Apply()` 中已有的 `style.richText = true` 让 `<color>` 标签生效。该方案半成品化：覆盖残缺、有行首漏匹配 / 注释字符串误伤 / `Split("@\n@")` 的 `@`/`\n` 陷阱等 bug。文档共 270 个代码块，其中 121 个标识为 `csharp`/`cs`，149 个无标识（绝大多数亦为 C#），其它语言为 0。

约束：全部渲染代码处于 `#if UNITY_EDITOR`；唯一外部依赖是 `Markdig.dll`（仅做 Markdown 解析）；QFramework 一贯偏好自研、少依赖。`MDStyleConverter` 已提供 `fixed_block` / `fixed_inline` 等宽样式且 richText 已开启。

## Goals / Non-Goals

**Goals:**

- 用正确的 tokenizer 替换 `string.Replace`，根治"注释/字符串内关键字误伤"与 `@`/`\n` 切割陷阱。
- 覆盖完整 C# 词法（关键字、字符串、注释、数字、类型名等），提供 5 类着色。
- 配色集中到单一静态配置表，沿用 VS Dark 基调。
- 最小侵入：复用逐行布局与既有 richText 通道，不改 `MDStyleConverter` / `MDLayout` 契约。
- 无新外部依赖。

**Non-Goals:**

- 不做通用多语言高亮（本期仅 C#，但保留语言识别与 tokenizer 的扩展点）。
- 不引入第三方语法库（Colorizer / TextMate 类）。
- 不升级为独立 Kit（`CodeHighlightKit` 之类）；目前仅 Markdown 渲染器一处消费者。
- 不做主题切换 UI / 用户可配置项（仅静态配色表）。
- 不影响运行时（保持 `#if UNITY_EDITOR`）。

## Decisions

### D1. 词法策略：手写字符级状态机

逐字符扫描，维护 `Normal` / `String` / `LineComment` / `BlockComment` 等状态。在 `String` / `LineComment` / `BlockComment` 状态下**不查关键字表**，所有字符归入对应 token —— 这从机制上根除"注释/字符串里的 `class` 被涂成关键字"和 `<color>` 标签嵌套破坏。

- 备选 A：正则驱动。被否——多行 `/* */` 跨行、注释内关键字排除等用正则要么脆弱要么回溯；与现有 bug 同源。
- 备选 B：第三方语法库（作者注释里曾尝试的 Colorizer 类）。被否——要加 dll、改 asmdef，与 QF"少依赖、自研"基调相悖，且本场景几乎只有 C#，性价比低。
- 选用状态机的理由：O(n) 单遍无回溯、零依赖、行为可预测、契合 QF 自研风格。

### D2. 跨行状态保持

对整个代码块连续 tokenize（而非逐行独立扫描），使 `/* ... */` 多行注释、多行 verbatim 字符串能正确保持状态；渲染时再按 NewLine token 切回逐行输出。

- 备选：逐行独立 tokenize。被否——多行注释会在行边界处状态丢失，重蹈误伤覆辙。

### D3. 渲染集成：保留逐行布局

tokenizer 输出 token 流；渲染侧把同一逻辑行的 token 拼成一行带 `<color>` 的富文本，调用 `Text()` 后 `Layout.NewLine()`，与现状逐行布局完全一致。

- 备选：整块一次 `Text()`。被否——会改变 `MDLayout` 的逐行布局与选中/换行行为，侵入大。

### D4. 语言识别与默认行为

依据 `FencedCodeBlock.Info`：`csharp` / `cs` → C# tokenizer；无标识 → **默认 C#**；其它 → 原样等宽渲染。

- 备选：无标识不着色。被否——会使 149 个无标识块失去高亮，体验倒退；文档 99% 为 C#，默认 C# 的误判成本极低（仅影响颜色，不破坏内容）。

### D5. 配色集中化

新增静态配置表（如 `CodeHighlightColors`），集中存放 keyword/string/comment/number/type 五色，沿用现状 VS Dark 基调（`#4EC9B0` type、`#57A64A` comment，补 `#569CD6` keyword、`#D69D85` string、`#B5CEA8` number）。

- 备选 A：硬编码在 tokenizer 内（即现状做法）。被否——散落、不可维护。
- 备选 B：接入 GUISkin / 主题系统。被否——过重，超出本期目标。

### D6. 放置位置

放在 `PackageKit/Markdown/Highlight/`（新增 `CSharpTokenizer.cs` 与配色配置），不单独成 Kit。

- 备选：升级为 `CoreKit` 下的独立工具。被否——YAGNI，目前单一消费者。

## Risks / Trade-offs

- **[风险] C# 词法边角情形**（插值字符串 `$"...{var}..."`、verbatim `@"..."` / `@$"..."`、逐字标识符 `@class`）→ 缓解：`String` 状态做扎实（识别 `@""` 不转义、`$""` 插值的大括号），用现有 270 块文档做回归对照；tokenizer 明确覆盖目标文档出现的语法。
- **[风险] richText `<color>` 在超长行 / 嵌套下 Unity 渲染异常** → 缓解：标签不跨行、行级独立包裹；保持逐行渲染。
- **[权衡] 无标识块默认 C# 可能把极少数非 C# 块误判** → 可接受（文档几乎无非 C#；仅影响颜色；识别 `Info` 后可逐步收紧）。
- **[风险] 性能（每次切文档重新 tokenize）** → 可忽略：270 块、字符级 O(n)，远轻于 Markdig 解析本身。
- **[权衡] 手写状态机需自维护关键字表** → 可接受：C# 关键字表稳定，集中一处。

## Migration Plan

- 无数据 / 公共接口迁移：纯编辑器行为替换，直接改 `WriteCode()` 实现。
- 回滚：单文件级 `git revert`；或在 `WriteCode()` 内保留"原样逐行渲染"作为 tokenizer 的兜底分支，异常时降级。

## Open Questions

- 配色是否最终采用 VS Dark 全套？（proposal 已提议，本设计沿用；可在实现期微调）
- 是否需在 PackageKit Preference 暴露高亮开关？（当前 Non-Goal，暂不做）
