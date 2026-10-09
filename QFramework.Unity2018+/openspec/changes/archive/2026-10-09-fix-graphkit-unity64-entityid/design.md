## Context

GraphKit IMGUI 的编辑器代码源自 xNode（MIT，已魔改），其中三处用到了 Unity 6000.4 起弃用的 InstanceID 系 API。上游 xNode master 未修复且仓库休眠（2024-08 后无推送）；社区存在两个未合并修复 PR：#393（int 签名 + 内部转换）与 #394（EntityId 签名直接迁移）。全仓库扫描确认受影响代码仅两个文件三处（见 proposal.md Impact）。

本仓库使用 Unity 2018.4.36f1，无法本地编译 `UNITY_6000_4_OR_NEWER` 分支。2026-10-09 维护者在 Unity 6000.6 项目实测，发现 v1 方案（OnOpen 保留 int 签名）在该版本编译失败，据此修订为 v2（见 D3）；实测项目另装有 UPM 版 QFramework（未修复），控制台同时报出 PackageCache 路径的 CS0619，属测试环境重复安装问题，非本修复缺陷。

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

### D3（v2，修订）：`OnOpen` 整方法双版本，6.4+ 分支改用 EntityId 签名

**v1 决策（已被实测证伪，2026-10-09）**：保留 `OnOpen(int instanceID, int line)` 签名、内部 `(EntityId)instanceID` 显式转换（xNode PR #393 方案）。

**证伪证据**：维护者在 Unity 6000.6 实测报 `error CS0619: 'EntityId.implicit operator EntityId(int)' is obsolete: 'EntityId will not be representable by an int in the future...'`。逐版本核对 UnityCsReference（6000.4 / 6000.5 / 6000.6 / 6000.7 / master=7000.0）确认：

| Unity | int↔EntityId 转换运算符 | InstanceIDToObject | [OnOpenAsset] 签名 |
|---|---|---|---|
| 6000.4 / 6000.5 | 警告级（可运行） | 警告级 | int 与 EntityId 并存 |
| 6000.6 / 6000.7 | **error 级**（实现 throw） | **error 级** | **仅 EntityId**（int 回调不再被调用） |
| 7000.0 (master) | error 级 | error 级 | 仅 EntityId |

**v2 决策**：`#if UNITY_6000_4_OR_NEWER` 下整方法双版本，新分支签名 `OnOpen(EntityId id, int line)`、直接 `EditorUtility.EntityIdToObject(id)`，全程不做 int→EntityId 转换。EntityId 签名在 6000.4–7000.0 全线受支持。xNode PR #394 的签名方案被采纳；其无条件迁移仍不采用（破坏 <6.4 用户），双版本宏保留。

### D4：`CreateFromTemplate` 传 `EntityId.None` 替代 `0`

新分支调用重载 `StartNameEditingIfProjectWindowExists(EntityId, AssetCreationEndAction, ...)`；`EntityId.None` 是 UnityCsReference 自身用法（对应原来传 `0` 的语义：尚不存在实例 ID）。`ShowCreatedAsset` 未弃用，不动。`EntityId` 位于 `UnityEngine` 命名空间，两个文件已有 `using UnityEngine;`，无需新增 using。

## Risks / Trade-offs

- [6.4 分支无法本地编译验证] → 已逐版本核对 UnityCsReference 6000.4–7000.0 分支源码（D3 表格）而非依赖上游 PR 结论；由维护者在 Unity 6.6/6.7 项目实测收尾（tasks 2.2）。
- [6000.4 补丁版本间警告级/错误级可能不一致] → v2 方案不使用任何 int↔EntityId 转换、不依赖 int 签名回调，各版本级别差异不再影响本修复。
- [宏内代码与上游魔改版本漂移] → 不回合上游（上游休眠）；文件头已保留 xNode 出处注释。
- [测试项目同时存在 UPM 包与 Assets 拷贝两份 QFramework] → 维护者实测时需确认测试项目只保留一份 QFramework，否则 PackageCache 里未修复的包会独立报 CS0619。

## Migration Plan

改动随下一个 bug-fix 版本发布。回滚即 revert 这两个文件的三个 `#if` 块（旧分支代码未动，revert 无连带影响）。
