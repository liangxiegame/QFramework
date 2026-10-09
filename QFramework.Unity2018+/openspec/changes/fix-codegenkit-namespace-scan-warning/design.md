## Context

v1.0.257（commit 4a25d817c）引入 `ArchitectureCodeGeneratorView`，`Init()` 中 `EditorPrefs.GetString(key, GetProjectNamespace())` 的默认值参数在 C# 中立即求值，导致每次 PackageKit 窗口初始化都会执行 `GetProjectNamespace()` 里的 `AssetDatabase.FindAssets("t:MonoScript", new[]{ "Assets/Scripts" })`；目录不存在时 Unity 打 `Folder not found` 警告。偏好仅在用户显式保存（`SavePreferences`）时写入 EditorPrefs。

## Goals / Non-Goals

**Goals:**
- 消除脚本目录缺失时的控制台警告
- 已保存偏好的用户不再执行任何扫描（顺带省掉 FindAssets 开销）
- 命名空间取值行为与现状完全等价

**Non-Goals:**
- 不改动默认输出目录 `Assets/Scripts` 约定本身（`mOutputRoot`、`CodeGenKitSetting.ScriptDir` 等保持不动）
- 不改动 `ResolveDefaultNamespace` 的推断/兜底逻辑
- 不处理其他 Toolkit 的类似约定目录（全库扫描确认无其他对不存在目录 FindAssets 的调用）

## Decisions

### D1：`AssetDatabase.IsValidFolder` 前置判断，而非 try/catch 或静默吞日志

`FindAssets` 对缺失目录只打日志、返回空数组，不抛异常，无法 catch；Unity 也无官方 API 关闭该日志。`IsValidFolder("Assets/Scripts")` 是官方提供的目录存在性检查，目录不存在时跳过 `FindAssets`，`existingNamespaces` 保持空列表，`ResolveDefaultNamespace` 自然走 productName 兜底——与现状（空扫描结果）行为一致，仅少了警告。

### D2：`HasKey` 惰性默认值，而非引入缓存字段

```csharp
// 改前：默认值参数立即求值，HasKey 也会执行扫描
mNamespace = EditorPrefs.GetString(key, GetProjectNamespace());
// 改后：仅无存值时求值
mNamespace = EditorPrefs.HasKey(key) ? EditorPrefs.GetString(key) : GetProjectNamespace();
```
两式在所有输入下返回值相同（含 key 存在且值为空串的情形），仅去除副作用。备选方案是给 `GetProjectNamespace` 加静态缓存，但那会让"目录后来被创建"的场景拿到过期结果，且改动面更大。

## Risks / Trade-offs

- [目录在视图打开后才被创建 → 该次初始化仍走兜底] → 与现状一致（现状每次都会扫描但用户场景里目录通常先于首次生成存在）；下次无存值初始化会重新推断，无陈旧值风险。
- [用户显式保存过空串命名空间 → HasKey 为真使用空串] → 与现状 `GetString(key, default)` 行为一致，非回归。

## Migration Plan

随下一个 bug-fix 版本发布。回滚即 revert 该文件两处小改。
