using System;

namespace Code.Core.BDFramework.SimpleGenCSharpCode
{
    public class MyMethod
    {
        private string CodeContent =@"
//[Note]
public static [return type] [method name] ([params])
{
   [method content]
}
";

        /// <summary>
        /// 覆盖方法
        /// </summary>
        /// <param name="content"></param>
        public void OverwriteContent(string content)
        {
            this.CodeContent = content;
        }

        public void SetMethSign(Type  returnType, string MethodName ,string Params)
        {
            if (returnType == null)
                this.CodeContent=    CodeContent.Replace("[return type]", "void");
            else 
                this.CodeContent=  CodeContent.Replace("[return type]", returnType.FullName);

            if (Params == null)
                this.CodeContent=    CodeContent.Replace("[params]", "");
            else 
                this.CodeContent=   CodeContent.Replace("[params]", Params);
            //
            this.CodeContent=   CodeContent.Replace("[method name]", MethodName);
        }

        public void SetMethodContent(string methodContent)
        {
            this.CodeContent=   CodeContent.Replace("[method content]", methodContent);
        }

        /// <summary>
        /// 设置备注
        /// </summary>
        /// <param name="note"></param>
        public void SetNote(string note)
        {
            this.CodeContent=   CodeContent.Replace("[Note]", note);
        }


        public string ToString()
        {
            return CodeContent;
        }
    }
}