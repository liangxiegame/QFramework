using System;

namespace Code.Core.BDFramework.SimpleGenCSharpCode
{
    public class MyPropties
    {
        private string CodeContent = "public [type] [field name]{get;set;}";

        public void SetType(Type type)
        {
            this.CodeContent=    this.CodeContent.Replace("[type]", type.FullName);
        }

        public void SetProptiesName(string name)
        {
            this.CodeContent=   this.CodeContent.Replace("[field name]", name);
        }

        public void OverwriteContent(string Content)
        {
            this.CodeContent= Content;
        }

        override public string ToString()
        {
            return CodeContent;
        }
    }
}