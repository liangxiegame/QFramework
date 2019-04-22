作者：SilenceT     邮箱：499683507@qq.com

# UIPanel

# 继承关系

实现接口：MonoBehaviour、ISingleton

# 描述

UI管理器

# 枚举

**Enum UILevel**

| 名称               | 等级 | 说明                                     |
| ------------------ | ---- | ---------------------------------------- |
| AlwayBottom        | -3   | 如果不想区分太复杂那最底层的UI请使用这个 |
| Bg                 | -2   | 背景层UI                                 |
| AnimationUnderPage | -1   | 动画层                                   |
| Common             | 0    | 普通层UI                                 |
| AnimationOnPage    | 1    | 动画层                                   |
| PopUI              | 2    | 弹出层UI                                 |
| Guide              | 3    | 新手引导层                               |
| Const              | 4    | 持续存在层                               |
| Toast              | 5    | 对话框层UI                               |
| Forward            | 6    | 最高UI层用来放置UI特效和模型             |
| AlwayTop           | 7    | 如果不想区分太复杂那最上层的UI请使用这个 |

 

 # **属性**

|                   |               |
| ----------------- | ------------- |
| Canvas RootCanvas | UI Root节点   |
| Camera UICamera   | UI 摄像机     |
| IManager Manager  | UIManager对象 |
|                   |               |

# **方法**

|                             |                                  |
| --------------------------- | -------------------------------- |
| **SetResolution**           | 设置**Canvas**宽高               |
| **SetMatchOnWidthOrHeight** | **设置Canvas高度百分比**         |
| **OpenUI**                  | **打开界面**                     |
| **ShowUI**                  | **显示界面**                     |
| **HideUI**                  | 隐藏界面                         |
| **CloseAllUI**              | **关闭所有界面**                 |
| **CloseUI**                 | **关闭并卸载界面**               |
| **Push**                    | **临时存储界面信息**             |
| **Back**                    | **返回上一个界面并关闭当前界面** |
| **GetUI**                   | **获取界面**                     |
|                             |                                  |

