## Purpose

架构代码生成器（CodeGenKit Architecture）默认命名空间的解析契约：偏好读取顺序、从既有脚本推断与兜底规则，以及解析过程不得因目录缺失产生编辑器警告。

## ADDED Requirements

### Requirement: 默认命名空间解析顺序

已保存的命名空间偏好 SHALL 优先于任何推断结果；仅当 EditorPrefs 中无存值时才进行推断。推断 SHALL 基于脚本目录下既有 MonoScript 的命名空间（取出现次数最多者）；无可用候选时 SHALL 使用项目名派生的默认命名空间。

#### Scenario: 使用已保存偏好且不触发扫描
- **WHEN** EditorPrefs 中已存在该项目的命名空间偏好
- **THEN** 直接使用保存值，不执行任何资产扫描

#### Scenario: 从既有脚本推断
- **WHEN** 无保存偏好且脚本目录下存在带命名空间的脚本
- **THEN** 默认命名空间取其中出现次数最多的命名空间

#### Scenario: 项目名兜底
- **WHEN** 无保存偏好且无任何可用命名空间候选
- **THEN** 默认命名空间由项目名（productName）派生

### Requirement: 推断过程不产生目录缺失警告

命名空间推断依赖的脚本目录（默认 `Assets/Scripts`）在项目中不存在时，SHALL 跳过扫描并走兜底逻辑，且 SHALL NOT 在控制台产生 `Folder not found` 类警告。

#### Scenario: 脚本目录不存在
- **WHEN** 项目中没有默认脚本目录且代码生成器视图初始化
- **THEN** 控制台无 `Folder not found` 警告，命名空间走项目名兜底
