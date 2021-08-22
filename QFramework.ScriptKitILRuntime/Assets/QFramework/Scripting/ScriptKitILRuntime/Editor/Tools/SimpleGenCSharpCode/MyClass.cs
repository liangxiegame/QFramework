using System.Collections.Generic;
using System.Web;

namespace Code.Core.BDFramework.SimpleGenCSharpCode
{
    public class MyClass
    {
        private string CodeContent =
@"
namespace [namespace]{
//工具生成代码,请勿删除标签，否则无法进行添加操作
//[using namespace]
//[Note]
//[Attribute]
public class [ClassName]
{
   //------[class end]------
   //------[Field end]------
   //------[Propties end]------
   //------[Method end]------
}
}//[namespace]
";

        public string Name { get; private set; }
        private string SelfNameSpace { get; set; }
        private List<MyClass> ClassesList;
        private List<MyField> FieldsList;
        private List<MyMethod> MethodList;
        private List<MyPropties> ProptiesList;
        private List<string> NamespaceList;
        private List<string> AtrributeList = new List<string>();


        public MyClass(string className)
        {
            this.FieldsList = new List<MyField>();
            this.MethodList = new List<MyMethod>();
            this.ProptiesList = new List<MyPropties>();
            this.NamespaceList = new List<string>();
            this.ClassesList = new List<MyClass>();
            //
            this.CodeContent = this.CodeContent.Replace("[ClassName]", className);
            this.Name = className;
        }

        public void SetSelfNameSpace(string nameSpace)
        {
            this.SelfNameSpace = nameSpace;
        }
        public void AddNameSpace(params string[] names)
        {
            foreach (var n in names)
            {
                this.NamespaceList.Add(n);
            }
        }

        public void AddClass(MyClass c)
        {
            this.ClassesList.Add(c);
        }
        public void AddField(MyField f)
        {
            FieldsList.Add(f);
        }

        public void AddProties(MyPropties p)
        {
            ProptiesList.Add(p);
        }

        public void AddMethod(MyMethod m)
        {
            MethodList.Add(m);
        }

        public void AddAttribute(string Attribute)
        {
            AtrributeList.Add(Attribute);
        }

        //
        override public string ToString()
        {
            string classes = "";
            foreach (var c in this.ClassesList)
            {
                var tempC = c.ToString().Replace(" //------[class end]------", " //------[subClass end]------");
                tempC = tempC.Replace("//------[Field end]------", "//------[subField end]------");
                tempC = tempC.Replace("//------[Propties end]------", "//------[subPropties end]------");
                tempC = tempC.Replace("//------[Method end]------", "//------[subMethod end]------");
                tempC = tempC.Replace("//工具生成代码,请勿删除标签，否则无法进行添加操作", "//SubClass " + c.Name);
                classes += (tempC + " \n");
            }

            string fields = "";
            foreach (var f in FieldsList)
            {
                fields += (f.ToString() + "\n");
            }
            string propties = "";
            foreach (var p in ProptiesList)
            {
                propties += (p.ToString() + "\n");
            }
            string methods = "";
            foreach (var m in MethodList)
            {
                methods += (m.ToString() + "\n");
            }

            string namespaces = "";
            foreach (var n in NamespaceList)
            {
                namespaces += ("using " + n.ToString() + ";\n");
            }

            //self namespace

            if (string.IsNullOrEmpty(SelfNameSpace))
            {
                this.CodeContent = this.CodeContent.Replace("namespace [namespace]{", "").Replace("}//[namespace]", "");
            }
            else
            {
                this.CodeContent = this.CodeContent.Replace("[namespace]", SelfNameSpace);
            }

            string attributes = "";
            if (this.AtrributeList.Count > 0)
            {
                foreach (var f in this.AtrributeList)
                {
                    attributes += ("[" + f.ToString() + "]\n");
                }
            }


            //
            var index = this.CodeContent.LastIndexOf("//[using namespace]");
            this.CodeContent = this.CodeContent.Insert(index, namespaces);

            index = this.CodeContent.LastIndexOf("//------[class end]------");
            this.CodeContent = this.CodeContent.Insert(index, classes);

            index = this.CodeContent.LastIndexOf("//------[Field end]------");
            this.CodeContent = this.CodeContent.Insert(index, fields);

            index = this.CodeContent.LastIndexOf("//------[Propties end]------");
            this.CodeContent = this.CodeContent.Insert(index, propties);

            index = this.CodeContent.LastIndexOf("//------[Method end]------");
            this.CodeContent = this.CodeContent.Insert(index, methods);

            index = this.CodeContent.LastIndexOf("//[Attribute]");
            this.CodeContent = this.CodeContent.Insert(index, attributes);

            return this.CodeContent;
        }
    }
}