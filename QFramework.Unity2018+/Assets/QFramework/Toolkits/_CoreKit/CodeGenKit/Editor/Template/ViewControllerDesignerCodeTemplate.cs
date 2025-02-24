#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;

namespace QFramework
{
    public class ViewControllerDesignerCodeTemplate
    {
        public static void Generate(CodeGenTask task)
        {
            var viewController = task.GameObject.GetComponent<ViewController>();

            var root = new RootCode()
                .Custom($"// Generate Id:{Guid.NewGuid().ToString()}")
                .Using("UnityEngine")
                .EmptyLine();
            
            if (CodeGenKit.Setting.IsDefaultNamespace)
            {
                root.Custom("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间")
                    .Custom("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
            }

            var namespaceName = string.IsNullOrWhiteSpace(task.Namespace)
                ? CodeGenKit.Setting.Namespace
                : task.Namespace;
            
            root.Namespace(namespaceName,
                (ns) =>
                {
                    var hasArchitecture = viewController.ArchitectureFullTypeName.IsNotNullAndEmpty();
                    var parentClassName = hasArchitecture ? "QFramework.IController" : "";
                    ns.Class(task.ClassName,parentClassName , true, false, cs =>
                    {
                        // binds
                        foreach (var bindData in task.BindInfos)
                        {
                            var hasComment = bindData.BindScript.Comment.IsNotNullAndEmpty();

                            if (hasComment)
                            {
                                cs.Custom("/// <summary>");
                                foreach (var comment in bindData.BindScript.Comment.Split('\n'))
                                {
                                    cs.Custom($"/// {comment}");
                                }
                                cs.Custom("/// </summary>");
                            }
                            
                            cs.Custom($"public {bindData.TypeName} {bindData.MemberName};");
                            cs.EmptyLine();
                        }
                        
                        // other binds
                        var otherBinds = task.GameObject.GetComponent<OtherBinds>();
                        if (otherBinds)
                        {
                            foreach (var referenceBind in otherBinds.Binds)
                            {
                                cs.Custom(
                                    $"public {referenceBind.Object.GetType().FullName} {referenceBind.MemberName};");
                                cs.EmptyLine();
                            }
                        }


                        if (hasArchitecture)
                        {
                            cs.Custom(
                                $"QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>{viewController.ArchitectureFullTypeName}.Interface;");
                        }
                    });
                });
            
            using (var fileWriter = File.CreateText(task.ScriptsFolder + $"/{task.ClassName}.Designer.cs"))
            {
                root.Gen(new FileCodeWriter(fileWriter));
            }
        }
    }
}
#endif