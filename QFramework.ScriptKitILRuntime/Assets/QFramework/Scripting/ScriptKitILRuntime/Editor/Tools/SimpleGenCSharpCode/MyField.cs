using System;
using System.Collections.Generic;

namespace Code.Core.BDFramework.SimpleGenCSharpCode
{
    public class MyField
    {
        
        private string CodeContent = 
@"      //[Attribute]
        public [type] [field name];";

        private List<string> atrributeList = new List<string>();
        public void AddAttribute(string Attribute)
        {
            atrributeList.Add(Attribute);
        }
        public void SetType(Type type)
        {
            this.CodeContent=    this.CodeContent.Replace("[type]", type.FullName);
        }

        public void SetFieldName(string name)
        {
            this.CodeContent=   this.CodeContent.Replace("[field name]", name);
        }

        public void SetContent(string Content)
        {
            
          
            if (Content.Contains("//[Attribute]") == false)
            {
                this.CodeContent ="      //[Attribute] \n" + Content;
            }
            else
            {
                this.CodeContent =  Content;
            }
        }

        override public string ToString()
        {

            if (this.atrributeList.Count > 0)
            {
                string attr = "";
                foreach (var f in this.atrributeList)
                {
                    attr += ("[" + f.ToString() + "]\n");
                }

                var index = this.CodeContent.LastIndexOf("//[Attribute]");
                this.CodeContent = this.CodeContent.Insert(index, attr);
            }
            return CodeContent;
        }
    }
}