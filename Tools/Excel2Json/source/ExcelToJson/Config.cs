using System;
using System.IO;
using System.Text;
using CommandLine;

namespace ExcelToJson
{

    /// <summary>
    /// 命令行参数定义
    /// </summary>
    public sealed class Options
    {

        public static Options Default = new Options();
        
        [Option("encoding", Required = false, DefaultValue = "UTF8", HelpText = "export file encoding.")]
        public string EncodingStr { get; set; }

        public Encoding Encoding => new UTF8Encoding(false);

        [Option( 'e',"excel_path")]
        public string ExcelPath { get; set; }
        [Option( 'j',"json_path")]
        public string JsonPath { get; set; }
        [Option( 'c',"script_path")]
        public string ScriptPath { get; set; }
        [Option( 't',"script_template_path")]
        public string ScriptTemplate { get; set; }

        public void Check()
        {
            ExcelPath = Path.GetFullPath(ExcelPath);
            ScriptTemplate = Path.GetFullPath(ScriptTemplate);
            
            if(!Directory.Exists(ExcelPath))
                Console.WriteLine($"{ExcelPath} not exist");
            
            if (string.IsNullOrEmpty(ScriptPath))
            {
                Console.WriteLine("ScriptPath is null");
            }
            else
            {
                ScriptPath = Path.GetFullPath(ScriptPath);
            }
            if (string.IsNullOrEmpty(JsonPath))
            {
                Console.WriteLine("JsonPath is null");
            }
            else
            {
                JsonPath = Path.GetFullPath(JsonPath);
            }
        }
    }
}