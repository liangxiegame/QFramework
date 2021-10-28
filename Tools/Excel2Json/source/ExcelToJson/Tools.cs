using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Excel;

namespace ExcelToJson
{
    public static class Tools
    {
        public const string EnumTableName = "enum";
        public const int HeaderRows = 3;
        public const string ExcelEndFlag = "//END";
        //忽略行或列的前缀
        public const string IgnorePrefix = "#";

        /// <summary>
        /// 根据表名获取生成的类或者json的文件名
        /// </summary>
        public static string GetTableGenName(string excelPath, string tableName)
        {
            string name = tableName;
            if (Regex.Match(tableName, @"Sheet\d+", RegexOptions.IgnoreCase).Success)
            {
                name = Path.GetFileNameWithoutExtension(excelPath);
            }
            return name;
        }

        public static bool CheckTableIsEnum(string tableName)
        {
            return tableName.ToLower() == EnumTableName;
        }
        
        public static bool CheckTableIsIgnore(string tableName)
        {
            return tableName.StartsWith("#");
        }

        public static IExcelDataReader CreateExcelReader(string path)
        {
            using FileStream excelFile = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var extension = Path.GetExtension(path);
            IExcelDataReader result;
            if (extension == ".xls")
            {
                result = ExcelReaderFactory.CreateBinaryReader(excelFile);
            }else if (extension == ".xlsx")
            {
                result = ExcelReaderFactory.CreateOpenXmlReader(excelFile);
            }
            else
            {
                throw new NotSupportedException($"不支持后缀为{extension}的文件");
            }

            return result;
        }
    }
}