### Editor 模块简介:

这里是对Unity编辑器的扩展，也包含了对Unity组件Inspector的拓展

有任何问题请在 QQ 群里 @拖鞋

# **工具**

## 1. **Transform检视面板**

![](<https://ws3.sinaimg.cn/large/006tNc79gy1fzhuhaxbiej30b803qgln.jpg>)

**按键功能**

1. P/R/S：设置默认，注：P/R 为000，S为111
2. Round./.0/.00：按小数点位数四舍五入
3. Copy/ Paste：复制/粘贴Transform的数据（非引用Transform）

注：区分Local和Global数据

1. PPos/PRot/PSca：单独粘贴Pos, 单独粘贴Rot, 单独粘贴Sca

**Transform和RectTransform共有的：**

![](<https://ws2.sinaimg.cn/large/006tNc79gy1fzhuhrhc3qj30b803qgln.jpg>)

1. Name：根据第一个非Transform的组件自动命名
2. Auto Ref：按照变量名自动引用序列化变量
3. CalledByEditor()：此物体的所有组件调用这个函数（实现编辑器下做批量操作）
4. C：复制CalledByEditor() { }的代码到剪贴板

1. 1. **RectTransform检视面板**

![](<https://ws2.sinaimg.cn/large/006tNc79gy1fzhuhxybczj30b807v3yp.jpg>)

**快捷键：（鼠标指向Help弹出文字提示）**

1. CTRL+箭头：偏移1
2. CTRL Shift+箭头：偏移10

**编辑器界面按钮：**

1. P/D/R/S：设置默认，注：P/R 为000，S为111
2. C/P：复制粘贴数据
3. Round：位数四舍五入成整数
4. F：快速设为下图

![](<https://ws2.sinaimg.cn/large/006tNc79gy1fzhui7ob9pj30b8036746.jpg>)

1. N：快速设为下图

![](<https://ws1.sinaimg.cn/large/006tNc79gy1fzhuiqmonqj30b8036q2u.jpg>)

1. Pivot：快速设锚点

![](<https://ws4.sinaimg.cn/large/006tNc79gy1fzhuj61rerj30b807v3yp.jpg>)

**Transform和RectTransform共有的：**

![](<https://ws1.sinaimg.cn/large/006tNc79gy1fzhujhdb46j30b803qgln.jpg>)

1. Name：根据第一个非Transform的组件自动命名
2. Auto Ref：按照变量名自动引用序列化变量
3. CalledByEditor()：此物体的所有组件调用这个函数（实现编辑器下做批量操作）
4. C：复制CalledByEditor() { }的代码到剪贴板

### 1. 1. **Hierarchy面板*****（注：这个邮件菜单可能有其他成员的功能未列入）***

快捷键：

1. CTRL+d：复制一个物体，保持原命名
2. CTRL+s：排序子物体
3. CTRL+g：选中的物体打组

右键菜单：

![](<https://ws3.sinaimg.cn/large/006tNc79gy1fzhuk0kf4ej309i0bfwej.jpg>)

1. Create Empty – Top：添加子物体，移动到第一位
2. Get Var Code Auto：将选中物体按类型生成代码复制到粘贴板
3. UI->img：新建一个Image，命名为img，不勾选Raycast
4. UI->txt：新建一个Text，命名为txt，不勾选Raycast
