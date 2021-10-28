using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Excel;

namespace ExcelToJson
{
	/// <summary>
	/// 根据表头，生成C#类定义数据结构
	/// 表头使用三行定义：字段名称、字段类型、注释
	/// </summary>
	static class CSExporter
	{
		struct FieldDef
		{
			public string Name;
			public string Type;
			public string Remark;
		}
		private const string arrayPre = "array_";
		private const string dicPre = "dic_";

		public static string GenClass(string excelPath)
		{
			// 加载Excel文件
			IExcelDataReader excelReader = Tools.CreateExcelReader(excelPath);
			// 数据检测
			if (excelReader.ResultsCount < 1)
			{
				throw new Exception("Excel file is empty: " + excelPath);
			}
			
			StringBuilder result = new StringBuilder();
			try
			{
				do
				{
					var excelName = excelReader.Name;
					string csName = Tools.GetTableGenName(excelPath, excelName);
					if (Tools.CheckTableIsEnum(excelName))
					{
						result.Append(GenEnum(excelReader, csName));
						continue;
					}
					if(Tools.CheckTableIsIgnore(excelName)) continue;
					result.Append(GenOneTable(excelReader, csName));
				
				} while (excelReader.NextResult());
			}
			catch (Exception e)
			{
				Console.WriteLine($"table {Path.GetFileNameWithoutExtension(excelPath)} parse cs error !!!! \n{e}");
				throw;
			}

			return result.ToString();
		}

		private static string GenOneTable(IExcelDataReader excelReader, string typeName)
		{
			StringBuilder outCompileCs = new StringBuilder("public class #class_name{#fields}");
			
			
			var fieldList = ParseField(excelReader);
			if (fieldList.Count <= 0) return "";
			outCompileCs = outCompileCs.Replace("#class_name", typeName);

			StringBuilder sb = new StringBuilder();
			foreach (FieldDef field in fieldList)
			{
				sb.AppendLine($"/// <summary> {field.Remark} </summary>");
				sb.AppendFormat("\tpublic {0} {1} {{ get; set; }}", field.Type, field.Name);
				sb.AppendLine();
			}
			
			outCompileCs = outCompileCs.Replace("#fields", sb.ToString());
			if (!string.IsNullOrEmpty(Options.Default.ScriptPath))
			{
				var template = File.ReadAllText(Options.Default.ScriptTemplate);
				template = template.Replace("#class_name", typeName);
				template = template.Replace("#fields", sb.ToString());
				var scriptPath = Path.Combine(Options.Default.ScriptPath, $"{typeName}.cs");
				using FileStream file = new FileStream(scriptPath, FileMode.Create, FileAccess.Write);
				using TextWriter writer = new StreamWriter(file, Options.Default.Encoding);
				writer.Write(template);
			}
			return outCompileCs.ToString();
		}

		private static List<FieldDef> ParseField(IExcelDataReader excelReader)
		{
			var result = new List<FieldDef>();
			int row = 0;
			List<string> names = new List<string>();
			List<string> types = new List<string>();
			List<string> comments = new List<string>();
			List<int> ignoreLine = new List<int>();
			// 第一行是字段名 第二行是类型 第三行是注释
			while (excelReader.Read())
			{
				List<string> targetList;
				row++;
				if (row == 1)
				{
					targetList = names;
				}
				else if (row == 2)
				{
					targetList = types;
				}
				else if (row == 3)
				{
					targetList = comments;
				}
				else
				{
					break;
				}
				for (int i = 0; i < excelReader.FieldCount; i++)
				{
					if(ignoreLine.Contains(i)) continue;
					var str = excelReader.GetString(i) ?? string.Empty;
					//如果是第一行，而且是#开头 则忽略
					if (row == 1 && str.StartsWith(Tools.IgnorePrefix))
					{
						ignoreLine.Add(i);
						continue;
					}
					targetList.Add(str);
				}
			}
			for (int i = 0; i < names.Count; i++)
			{
				var name = names[i];
				var type = types[i];
				if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)) break;
				var comment = comments[i];
				FieldDef field;
				field.Name = name;
				if (type.Contains(arrayPre))
				{
					var listTypeStr = type.Replace(arrayPre, "");
					type = $"List<{listTypeStr}>";
				}
				else if(type.Contains(dicPre))
				{
					var match = Regex.Match(type, @"\w+_(\w+)_(\w+)");
					type = $"Dictionary<{match.Groups[1].Value}, {match.Groups[2].Value}>";
				}else if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(field.Name))
				{
					continue;
				}
				field.Type = type;
				field.Remark = Regex.Replace(comment, "[\r\n\t]", " ", RegexOptions.Compiled);
				result.Add(field);
			}

			return result;
		}

		private static string GenEnum(IExcelDataReader excelReader, string typeName)
		{
			int row = 0;
			List<StringBuilder> stringBuilders = new List<StringBuilder>();
			while (excelReader.Read())
			{
				if (stringBuilders.Count <= 0)
				{
					stringBuilders = new List<StringBuilder>();
					for (int i = 0; i < excelReader.FieldCount; i++)
					{
						var obj = excelReader.GetString(i);
						if(string.IsNullOrEmpty(obj)) continue;
						stringBuilders.Add(new StringBuilder());
					}
				}
				// i是列 row是行
				row++;
				for (int i = 0; i < excelReader.FieldCount; i++)
				{
					var obj = excelReader.GetString(i);
					if(string.IsNullOrEmpty(obj)) continue;
					if (row == 1)
					{
						stringBuilders[i].AppendLine($"public enum {obj}{{");
					}
					else
					{
						stringBuilders[i].AppendLine($"\t{obj},");
					}
				}
			}
			StringBuilder result = new StringBuilder();
			foreach (var sb in stringBuilders)
			{
				sb.AppendLine("}");
				result.Append(sb);
			}
			if (!string.IsNullOrEmpty(Options.Default.ScriptPath))
			{
				var scriptPath = Path.Combine(Options.Default.ScriptPath,
					$"{Path.GetFileNameWithoutExtension(typeName)}Enum.cs");
				using FileStream file = new FileStream(scriptPath, FileMode.Create, FileAccess.Write);
				using TextWriter writer = new StreamWriter(file, Options.Default.Encoding);
				writer.Write(result.ToString());
			}
			return result.ToString();
		}
	}
}
