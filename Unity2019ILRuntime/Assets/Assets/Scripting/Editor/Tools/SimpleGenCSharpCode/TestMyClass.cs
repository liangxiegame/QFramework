using System.IO;
using UnityEngine;

namespace Code.Core.BDFramework.SimpleGenCSharpCode
{
    public class TestMyClass : MonoBehaviour
    {
        private void Start()
        {
            DO();
        }

        public void DO()
        {
           MyClass c = new MyClass("TestForGenCode");
            
           c.SetSelfNameSpace("xxx.xxx.xxx");
           c.AddNameSpace("System");
            
            //添加一个field
            var f = new MyField();
            f.SetType(typeof(int));
            f.SetFieldName("testField");
            f.SetContent("public int xxx = 0;");
            f.AddAttribute("test()");
            c.AddField(f);
            
            //添加propty
            
            var p = new MyPropties();
            p.SetProptiesName("testPropties");
            p.SetType(typeof(string));
            c.AddProties(p);
            
            var m = new MyMethod();
            m.SetMethSign(null , "TestMethod" , null);
            m.SetMethodContent(
@"int i = 0;
var xx =i++;");
            
            c.AddMethod(m);

           var code =  c.ToString();
            
            File.WriteAllText("D:/test.cs" , code);
        }
    }
}