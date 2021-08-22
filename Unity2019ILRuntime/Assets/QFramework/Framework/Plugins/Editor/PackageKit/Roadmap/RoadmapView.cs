/****************************************************************************
 * Copyright (c) 2021.3 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.ComponentModel;

namespace QFramework
{
    [DisplayName("Roadmap 路线图")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(int.MaxValue - 1)]
    public class RoadmapView : VerticalLayout, IPackageKitView
    {
        public IQFrameworkContainer Container { get; set; }

        public void Init(IQFrameworkContainer container)
        {
            EasyIMGUI.Label().Text("Roadmap 路线图").FontSize(12).Parent(this);

            EasyIMGUI.Vertical().Box()
                .AddChild(EasyIMGUI.Label().Text("将来也许").FontBold())
                .AddChild(EasyIMGUI.Label().Text("* sLua、toLua、xLua、ILRuntime 支持"))
                .AddChild(EasyIMGUI.Label().Text("* UnityPackageManager 支持"))
                .AddChild(EasyIMGUI.Label().Text("* ResKit 支持自定义目录方案"))
                .AddChild(EasyIMGUI.Label().Text("* UIKit 支持多个 Canvas Root 和 摄像机方案"))
                .AddChild(EasyIMGUI.Label().Text("v0.12.x").FontBold())
                .AddChild(EasyIMGUI.Label().Text("* 插件平台的插件整理"))
                .AddChild(EasyIMGUI.Label().Text("* Example 示例整理"))
                .AddChild(EasyIMGUI.Label().Text("* Asset Store 兼容 & 减少第三方依赖"))
                .AddChild(EasyIMGUI.Label().Text("* 文档整理 & 在编辑器内部内置"))
                .AddChild(EasyIMGUI.Label().Text("v0.11.x（开发中）").FontBold())
                .AddChild(EasyIMGUI.Label().Text("* 打 dll 优化旧设备的编译速度"))
                .AddChild(EasyIMGUI.Label().Text("v0.10.x（已完成）").FontBold())
                .AddChild(EasyIMGUI.Label().Text("* ILRuntime 支持（只完成一部分，后续再支持）"))
                .AddChild(EasyIMGUI.Label().Text("v0.9.x（已完成）").FontBold())
                .AddChild(EasyIMGUI.Label().Text("* 单元测试覆盖"))
                .AddChild(EasyIMGUI.Label().Text("* PackageKit、Framework、Extensions 的示例全部覆盖"))
                .AddChild(EasyIMGUI.Label().Text("* 3 ~ 5 个 Demo 发布"))
                .AddChild(EasyIMGUI.Label().Text("v0.2.x ~ v0.8.x（已完成）").FontBold())
                .AddChild(EasyIMGUI.Label().Text("* PackageManager 独立成 PackageKit"))
                .AddChild(EasyIMGUI.Label().Text("* 剥离掉第三方插件，最为扩展插件支持"))
                .AddChild(EasyIMGUI.Label().Text("* 插件平台发布：https://liangxiegame.com/qf/package"))
                .AddChild(EasyIMGUI.Label().Text("* 命名空间从 QF 改回 QFramework"))
                .AddChild(EasyIMGUI.Label().Text("* 大量 Bug 修复、大量示例编写"))
                .AddChild(EasyIMGUI.Label().Text("* 五子棋 Demo 发布：Demo：五子棋"))
                .AddChild(EasyIMGUI.Label().Text("* QFramework 使用指南 2020 完结：QFramework 使用指南 2020"))
                .AddChild(EasyIMGUI.Label().Text("v0.1.x（已完成）").FontBold())
                .AddChild(EasyIMGUI.Label().Text("* UniRx、Zenject、uFrame、JsonDotnet、CatLib 集成和增强"))
                .AddChild(EasyIMGUI.Label().Text("* IOC 增加 IOC 部分"))
                .AddChild(EasyIMGUI.Label().Text("* 框架自动更新机制 => PackageManager"))
                .AddChild(EasyIMGUI.Label().Text("* 命名空间从 QFramework 改成 QF"))
                .AddChild(EasyIMGUI.Label().Text("v0.0.x（已完成）").FontBold())
                .AddChild(EasyIMGUI.Label().Text("* 框架搭建 2017 的工具集收录"))
                .AddChild(EasyIMGUI.Label().Text("* 框架搭建 2018 的 ResKit 和 UI Kit 模块实现"))
                .AddChild(EasyIMGUI.Label().Text("* ActionKit 模块实现"))
                .AddChild(EasyIMGUI.Label().Text("* Manager Of Managers 支持"))
                .AddChild(EasyIMGUI.Label().Text("* 框架自动更新机制"))
                .Parent(this);
        }

        public void OnUpdate()
        {
        }

        void IPackageKitView.OnGUI()
        {
            DrawGUI();
        }

        public void OnDispose()
        {
        }

        void IPackageKitView.OnShow()
        {
        }

        void IPackageKitView.OnHide()
        {
        }
    }
}