using System;
using System.CodeDom;

namespace QF.GraphDesigner
{
    //public class EnumCodeGenerator : CodeGenerator
    //{
    //    public EnumData EnumData { get; set; }

    //    public override Type GeneratorFor
    //    {
    //        get { throw new NotImplementedException(); }
    //        set { throw new NotImplementedException(); }
    //    }

    //    public override void Initialize(CodeFileGenerator fileGenerator)
    //    {
    //        base.Initialize(fileGenerator);
    //        AddEnum(EnumData);
    //    }

    //    public void AddEnum(EnumData data)
    //    {
    //        var enumDecleration = new CodeTypeDeclaration(data.Name) {IsEnum = true};
    //        foreach (var item in data.EnumItems)
    //        {
    //            enumDecleration.Members.Add(new CodeMemberField(enumDecleration.Name, item.Name));
    //        }
    //        Namespace.Types.Add(enumDecleration);
    //    }
    //}
}