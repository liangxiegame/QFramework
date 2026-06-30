## 1. 骨架与配色配置

- [x] 1.1 新建目录 `Assets/QFramework/Toolkits/_CoreKit/PackageKit/Markdown/Highlight/`
- [x] 1.2 新增配色配置 `CodeHighlightColors`（静态表）：keyword `#569CD6`、string `#D69D85`、comment `#57A64A`、number `#B5CEA8`、type `#4EC9B0`
- [x] 1.3 确认新文件归属 `QFramework.CoreKit` 程序集（asmdef 范围内，Unity 自动生成 .meta）

## 2. C# Tokenizer

- [x] 2.1 新增 `CSharpTokenizer.cs`，定义 token 类型（Keyword / String / Comment / Number / Type / Identifier / Operator / Whitespace / NewLine）
- [x] 2.2 建立完整 C# 关键字表（含 `public/class/void/using/namespace/private/internal/protected/static/readonly/const/abstract/virtual/override/sealed/async/await/get/set/return/if/else/for/foreach/while/do/new/var/this/base/null/true/false/enum/struct/interface/try/catch/finally/throw/switch/case/break/continue/in/out/ref/params` 等）
- [x] 2.3 实现状态机扫描：`Normal` / `String` / `LineComment` / `BlockComment` 状态切换；`String` 支持 verbatim `@"..."` 与插值 ` $"..."` `
- [x] 2.4 整块连续 tokenize 实现跨行状态保持，输出保留行边界的 token 流
- [x] 2.5 确保 `String` / `Comment` 状态下不查关键字表（根治误伤与 `<color>` 嵌套破坏）

## 3. 渲染集成

- [x] 3.1 新增语言识别：依据 `FencedCodeBlock.Info`，`csharp`/`cs` → C#，无标识 → 默认 C#，其它 → 不着色
- [x] 3.2 重写 `MDRendererMarkdown.WriteCode()`：调用 tokenizer，按 token 类型查配色表生成 `<color>` 富文本，保留逐行 `Text()` + `Layout.NewLine()`
- [x] 3.3 更新 `MDRendererBlockCode.cs`：将语言标识传入 `WriteCode` 用于判定，替换原空 `TODO` 分支
- [x] 3.4 删除旧的 `string.Replace` 穷举逻辑与被注释的 `LanguageParser` 残留

## 4. 验证

- [ ] 4.1 在 Unity 编辑器打开"用户手册"，确认 C# 代码块出现 keyword/string/comment/number/type 五色高亮  ← 需在 Unity 编辑器内目视确认
- [x] 4.2 回归对照：行首 `public`、注释内 `class`、字符串内 `void`、verbatim `@class`/`@"..."`、多行 `/* */` 均渲染正确  ← tokenizer 逻辑层已用编译+运行测试验证（10/10 PASS）；端到端像素建议在 Unity 内扫一眼
- [x] 4.3 确认非 C#（如 `json`/`bash` 标识）代码块原样等宽渲染、不着色  ← `IsCSharpLanguage` 对非 C# 返回 false → else 分支逐行原样；等宽由既有 FixedBlock 样式保证
- [x] 4.4 确认运行时编译不受影响（`#if UNITY_EDITOR` 隔离）  ← 全部新增/修改文件均在 `#if UNITY_EDITOR` 内，编译期隔离
- [x] 4.5 `openspec validate "add-csharp-syntax-highlight"` 通过
