using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;

namespace QF.GraphDesigner
{
    public class CodeFileGenerator : FileGeneratorBase
    {
        public CodeNamespace Namespace { get; set; }
        public CodeCompileUnit Unit { get; set; }

        public bool RemoveComments { get; set; }
        public string NamespaceName { get; set; }
        public CodeFileGenerator(string ns = null)
        {
            NamespaceName = ns;
        }

        public OutputGenerator[] Generators
        {
            get;
            set;
        }

        public override string CreateOutput()
        {
            RemoveComments = Generators.Any(p => !p.AlwaysRegenerate);

            Namespace = new CodeNamespace(NamespaceName);
            Unit = new CodeCompileUnit();
            Unit.Namespaces.Add(Namespace);
      
            foreach (var codeGenerator in Generators.Where(p=>p.IsValid()))
            {
               // UnityEngine.Debug.Log(codeGenerator.GetType().Name + " is generating");
                codeGenerator.Initialize(this);
            }
            var provider = new CSharpCodeProvider();

            var sb = new StringBuilder();
            var tw1 = new IndentedTextWriter(new StringWriter(sb), "    ");
            
            provider.GenerateCodeFromCompileUnit(Unit, tw1, new CodeGeneratorOptions()
            {
                BlankLinesBetweenMembers = true,
                BracingStyle = "C"
            });
            
            tw1.Close();
            if (RemoveComments)
            {
                var removedLines = sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(10).ToArray();
                return string.Join(Environment.NewLine, removedLines);
            }
            return sb.ToString();
        }
        
        public override bool CanGenerate(FileInfo fileInfo)
        {
         
            if (Generators.All(p => !p.IsValid())) return false;
            if (Generators.All(p => p.AlwaysRegenerate)) return true;

            //var doesAnyTypeExist = Generators.Any(p => p.DoesTypeExist(fileInfo));
            if (fileInfo.Exists)
            {
                return false;
            }
            
            return true;
        }
    }
}