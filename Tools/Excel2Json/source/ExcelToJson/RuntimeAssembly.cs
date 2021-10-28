using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace ExcelToJson
{
    public static class RuntimeAssembly
    {
        public static Assembly RuntimeAsm;

        private static string BaseProgram = @"
using System.Collections.Generic;
using System;
            class Program{
                static void Main(string[] args){
                }
            }";

        public static void Compile(List<string> classes)
        {
            var csc = new CSharpCodeProvider(new Dictionary<string, string>() {{"CompilerVersion", "v3.5"}});
            var parameters = new CompilerParameters(new[] {"mscorlib.dll", "System.Core.dll"}, "", false);
            parameters.GenerateExecutable = true;
            StringBuilder sb = new StringBuilder();
            sb.Append(BaseProgram);
            foreach (string @class in classes)
            {
                sb.Append(@class);
            }
            CompilerResults results = csc.CompileAssemblyFromSource(parameters, sb.ToString());
            sb.Clear();
            if (results.Errors.HasErrors)
            {
                foreach (CompilerError compilerError in results.Errors)
                {
                    sb.AppendLine(compilerError.ErrorText);
                }
                Console.WriteLine("compile error！！！");
                Console.WriteLine(sb.ToString());
            }
            RuntimeAsm = results.CompiledAssembly;
        }
    }
}