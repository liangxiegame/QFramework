using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel;
using Excel.Log;

namespace ExcelToJson
{

    sealed class Program
    {
        private static List<string> excelPaths = new List<string>();
        private static List<string> excelFileNames = new List<string>();
        private static readonly object lockObj = new object();
        private static bool finish = false;

        [STAThread]
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            ParseConfig(args);
            CreateDir();

            //第一次CreateOpenXmlReader需要在单一线程跑，否则会出现错误
            foreach (var excelPath in excelPaths)
            {
                Tools.CreateExcelReader(excelPath);
            }
            Gen();
            while (!finish)
            {
                Thread.Sleep(100);
            }
            sw.Stop();
            Console.WriteLine($"use {sw.ElapsedMilliseconds}ms");
            Console.ReadKey();
        }
        
        private static async void Gen()
        {
            List<string> cs = new List<string>();
            int length = excelPaths.Count;
            List<Task> allTask = new List<Task>(excelPaths.Count);
            int index = 0;
            foreach (string excelPath in excelPaths)
            {
                allTask.Add(Task.Run(() =>
                {
                    var str = CSExporter.GenClass(excelPath);
                    lock (lockObj)
                    {
                        cs.Add(str);
                        index++;
                        Console.WriteLine($"gen {Path.GetFileNameWithoutExtension(excelPath)} cs...({index}/{length})");
                    }
                }));
            }
            await Task.WhenAll(allTask);

            RuntimeAssembly.Compile(cs);
            index = 0;
            allTask.Clear();
            foreach (string excelPath in excelPaths)
            {
                allTask.Add(Task.Run(() =>
                {
                    JsonExporter.GenJson(excelPath);
                    lock (lockObj)
                    {
                        index++;
                        Console.WriteLine($"gen {Path.GetFileNameWithoutExtension(excelPath)} json...({index}/{length})");
                    }
                }));
            }
            await Task.WhenAll(allTask);
            finish = true;
            Console.WriteLine(" ************** Export Succeed ! *************** ");
        }

        private static void GenSync()
        {
            List<string> cs = new List<string>();
            int length = excelPaths.Count;
            int index = 0;
            foreach (string excelPath in excelPaths)
            {

                var str = CSExporter.GenClass(excelPath);
                cs.Add(str);
                index++;
                Console.WriteLine($"gen {Path.GetFileNameWithoutExtension(excelPath)} cs...({index}/{length})");
            }

            RuntimeAssembly.Compile(cs);
            index = 0;
            foreach (string excelPath in excelPaths)
            {
                JsonExporter.GenJson(excelPath);
                index++;
                Console.WriteLine($"gen {Path.GetFileNameWithoutExtension(excelPath)} json...({index}/{length})");
            }
            finish = true;
            Console.WriteLine(" ************** Export Succeed ! *************** ");
        }

        private static void ParseConfig(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments(args, Options.Default);
            Options.Default.Check();
            DirectoryInfo root = new DirectoryInfo(Options.Default.ExcelPath);
            var files = root.GetFiles("*.xls").Concat(root.GetFiles("*.xlsx"));
            files = files.Distinct();
            files = files.Where((info => !info.Name.StartsWith("~")));
            foreach (FileInfo file in files)
            {
                if (excelPaths.Contains(file.FullName)) continue;
                excelPaths.Add(file.FullName);
                excelFileNames.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
        }

        private static void CreateDir()
        {
            //如果存在则删除重新创建
            if (!string.IsNullOrEmpty(Options.Default.ScriptPath))
            {
                if(Directory.Exists(Options.Default.ScriptPath))
                    Directory.Delete(Options.Default.ScriptPath, true);
                Directory.CreateDirectory(Options.Default.ScriptPath);
            }
            if (!string.IsNullOrEmpty(Options.Default.JsonPath))
            {
                if (Directory.Exists(Options.Default.JsonPath))
                    Directory.Delete(Options.Default.JsonPath, true);
                Directory.CreateDirectory(Options.Default.JsonPath);
            }
        }
    }
}

