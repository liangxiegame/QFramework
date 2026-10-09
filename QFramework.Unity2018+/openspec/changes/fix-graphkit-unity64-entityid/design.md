## Context

GraphKit IMGUI 的编辑器代码源自 xNode（MIT，已魔改），其中三处用到了 Unity 6000.4 起弃用的 InstanceID 系 API。上游 xNode master 未修复且仓库休眠（2024-08 后无推送）；社区 PR [Siccity/xNode#393](https://github.com/Siccity/xNode/pull/393)（未合并）采用 `#if UNITY_6000_4_OR_NEWER` 双版本兼容方案，与本仓库约束（最低支持 Unity 2018.4）吻合，本设计以其为模板。全仓库扫描确认受影响代码仅两个文件三处（见 proposal.md Impact）。

本仓库使用 Unity 2018.4.36f1，无法本地编译 `UNITY_6000_4_OR_NEWER` 分支。

## Goals / Non-Goals

**Goals:**
- Unity 6000.4+ 项目中 GraphKit 编辑器代码编译通过（消除 error 级弃用）
- Unity 2018.4–6000.3 的编译产物与行为零变化（旧分支代码逐字节不变）

**Non-Goals:**
- 不做全库 Unity 6 兼容审计（其他 API 仅为警告级或未受影响）
- 不迁移与本次报错无关的 InstanceID 用法
- 不引入运行时程序集的任何改动
- 不做 Unity 6.4 自动化测试（无 CI/asmdef 基础设施，见 tasks 的手动验证安排）

## Decisions

### D1：`#if UNITY_6000_4_OR_NEWER` 双版本宏，而非直接迁移

替代方案是 xNode PR #394 式的直接切 EntityId（无宏），但那会把最低 Unity 版本抬到 6.4，破坏 2018.4+ 用户。双版本宏是唯一同时满足两端的方案。`UNITY_6000_4_OR_NEWER` 由 Unity 自动生成（Unity 6000.4+ 定义），与 PR #393 用法一致。

### D2：`DoCreateCodeFile` 整类双版本，而非只对方法签名加宏

基类 `EndNameEditAction` 本身是 error 级弃用，类声明处就会报 CS0619，因此 `#if` 必须包住整个类声明。类体仅 4 行，两份重复可接受；新分支继承 `AssetCreationEndAction` 并 override `Action(EntityId, string, string)`，方法体不变（未使用 instanceId 参数）。

### D3：`OnOpen` 保留 `int` 签名，内部显式转换

`[OnOpenAsset]` 回调签名保持 `OnOpen(int instanceID, int line)`，内部用 `EditorUtility.EntityIdToObject((EntityId)instanceID)`。替代方案（PR #394）把签名改成 `OnOpen(EntityId id, ...)`，但那依赖 6.4 的 OnOpenAsset 对 EntityId 重载的分发行为，风险更高且无收益。显式 `(EntityId)` 转换在存在隐式转换的语言规则下必然合法，最稳妥。

### D4：`CreateFromTemplate` 传 `EntityId.None` 替代 `0`

新分支调用重载 `StartNameEditingIfProjectWindowExists(EntityId, AssetCreationEndAction, ...)`；`EntityId.None` 是 UnityCsReference 自身用法（对应原来传 `0` 的语义：尚不存在实例 ID）。`ShowCreatedAsset` 未弃用，不动。`EntityId` 位于 `UnityEngine` 命名空间，两个文件已有 `using UnityEngine;`，无需新增 using。

## Risks / Trade-offs

- [6.4 分支无法本地编译验证] → 以 PR #393（同一 API 面、已在 6000.4+ 验证过报错与签名）为模板逐点对照；由维护者在 Unity 6.4+ 项目手动编译 + 走两遍创建/打开流程（tasks 中列为用户侧验证项）。
- [6000.4 补丁版本间警告级/错误级可能不一致] → 无论级别，API 面相同，双版本宏方案不受影响。
- [宏内代码与上游魔改版本漂移] → 不回合上游（上游休眠）；在本仓库文件头已保留 xNode 出处注释的基础上，于 `DoCreateCodeFile` 双版本处不再额外注明来源，保持与文件现有注释风格一致。

## Migration Plan

改动随下一个 bug-fix 版本发布。回滚即 revert 这两个文件的三个 `#if` 块（旧分支代码未动，revert 无连带影响）。
