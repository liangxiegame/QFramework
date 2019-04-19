>作者：vin129     邮箱：515019721@qq.com

# DefaultPanelLoader

# 继承关系

实现接口：IPanelLoader

# 描述

> 管理**UIPanel** 自身资源加载
>
>  **UIPanel**  创建时被创建并绑定 **UIPanel.mPanelLoader**

# 私有属性

|                          |              |
| ------------------------ | ------------ |
| ResLoader **mResLoader** | 用于资源管理 |

# 方法

|                     |                                         |
| ------------------- | --------------------------------------- |
| **LoadPanelPrefab** | 加载UI资源（Resource/panelName）        |
| **LoadPanelPrefab** | 加载UI资源（assetBundleName,panelName） |
| **Unload**          | 卸载、回收                              |

