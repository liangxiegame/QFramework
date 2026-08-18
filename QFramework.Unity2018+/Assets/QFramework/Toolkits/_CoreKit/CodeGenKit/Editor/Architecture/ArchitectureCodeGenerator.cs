/****************************************************************************
 * Copyright (c) 2015 ~ 2026 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QFramework
{
    public enum ArchitectureCodeType
    {
        Architecture,
        System,
        Model,
        Command,
        Utility,
        Query
    }

    public sealed class ArchitectureCodeGenerationPreview
    {
        internal ArchitectureCodeGenerationPreview(bool isValid, string errorMessage, ArchitectureCodeType codeType,
            string className, string codeNamespace, string assetPath, string code)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
            CodeType = codeType;
            ClassName = className;
            Namespace = codeNamespace;
            AssetPath = assetPath;
            Code = code;
        }

        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; }
        public ArchitectureCodeType CodeType { get; private set; }
        public string ClassName { get; private set; }
        public string Namespace { get; private set; }
        public string AssetPath { get; private set; }
        public string Code { get; private set; }
    }

    public sealed class ArchitectureCodeGenerationResult
    {
        internal ArchitectureCodeGenerationResult(bool success, string errorMessage, string assetPath)
        {
            Success = success;
            ErrorMessage = errorMessage;
            AssetPath = assetPath;
        }

        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public string AssetPath { get; private set; }
    }

    public static class ArchitectureCodeGenerator
    {
        private static readonly HashSet<string> CSharpKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach",
            "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long",
            "namespace", "new", "null", "object", "operator", "out", "override", "params", "private",
            "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
            "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof",
            "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
        };

        public static ArchitectureCodeGenerationPreview CreatePreview(ArchitectureCodeType codeType,
            string inputName, string codeNamespace, string outputRoot, bool generateInterface = false)
        {
            inputName = (inputName ?? string.Empty).Trim();
            codeNamespace = (codeNamespace ?? string.Empty).Trim();
            outputRoot = NormalizeAssetPath(outputRoot);

            var error = ValidateInputName(inputName);
            if (!string.IsNullOrEmpty(error)) return Invalid(codeType, error);

            error = ValidateNamespace(codeNamespace);
            if (!string.IsNullOrEmpty(error)) return Invalid(codeType, error);

            error = ValidateOutputRoot(outputRoot);
            if (!string.IsNullOrEmpty(error)) return Invalid(codeType, error);

            var className = BuildClassName(codeType, inputName);
            var assetPath = BuildAssetPath(codeType, className, outputRoot);
            var code = BuildCode(codeType, className, codeNamespace, generateInterface);

            return new ArchitectureCodeGenerationPreview(true, string.Empty, codeType, className, codeNamespace,
                assetPath, code);
        }

        public static ArchitectureCodeGenerationResult Generate(ArchitectureCodeGenerationPreview preview)
        {
            if (preview == null)
                return new ArchitectureCodeGenerationResult(false, "生成预览不能为空", string.Empty);

            if (!preview.IsValid)
                return new ArchitectureCodeGenerationResult(false, preview.ErrorMessage, preview.AssetPath);

            string temporaryPath = null;

            try
            {
                var directory = Path.GetDirectoryName(preview.AssetPath);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

                temporaryPath = preview.AssetPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
                using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write,
                           FileShare.None))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    writer.Write(preview.Code);
                }

                File.Move(temporaryPath, preview.AssetPath);
                temporaryPath = null;

                return new ArchitectureCodeGenerationResult(true, string.Empty, preview.AssetPath);
            }
            catch (IOException)
            {
                if (File.Exists(preview.AssetPath))
                    return new ArchitectureCodeGenerationResult(false,
                        "文件已存在，QFramework 不会覆盖已有代码：" + preview.AssetPath, preview.AssetPath);

                return new ArchitectureCodeGenerationResult(false, "无法创建文件：" + preview.AssetPath,
                    preview.AssetPath);
            }
            catch (Exception exception)
            {
                return new ArchitectureCodeGenerationResult(false,
                    "无法创建文件：" + preview.AssetPath + "\n" + exception.Message, preview.AssetPath);
            }
            finally
            {
                if (!string.IsNullOrEmpty(temporaryPath)) TryDeleteFile(temporaryPath);
            }
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch
            {
                // The destination is never exposed until the atomic move succeeds.
            }
        }

        public static string CreateDefaultNamespace(string productName)
        {
            var source = (productName ?? string.Empty).Trim();
            var builder = new StringBuilder();
            var capitalizeNext = true;

            foreach (var character in source)
            {
                if (!char.IsLetterOrDigit(character))
                {
                    capitalizeNext = true;
                    continue;
                }

                builder.Append(capitalizeNext && char.IsLetter(character)
                    ? char.ToUpperInvariant(character)
                    : character);
                capitalizeNext = false;
            }

            var result = builder.ToString();
            if (string.IsNullOrEmpty(result)) return "Game";
            if (char.IsDigit(result[0]) || CSharpKeywords.Contains(result)) result = "Game" + result;
            return result;
        }

        public static string ResolveDefaultNamespace(string configuredNamespace, bool isDefaultConfiguration,
            IEnumerable<string> existingNamespaces, string productName)
        {
            configuredNamespace = (configuredNamespace ?? string.Empty).Trim();
            if (!isDefaultConfiguration && string.IsNullOrEmpty(ValidateNamespace(configuredNamespace)))
                return configuredNamespace;

            var namespaceCounts = new Dictionary<string, int>(StringComparer.Ordinal);
            if (existingNamespaces != null)
            {
                foreach (var existingNamespace in existingNamespaces)
                {
                    var candidate = (existingNamespace ?? string.Empty).Trim();
                    if (!string.IsNullOrEmpty(ValidateNamespace(candidate))) continue;

                    int count;
                    namespaceCounts.TryGetValue(candidate, out count);
                    namespaceCounts[candidate] = count + 1;
                }
            }

            var selectedNamespace = string.Empty;
            var selectedCount = 0;
            foreach (var pair in namespaceCounts)
            {
                if (pair.Value < selectedCount) continue;
                if (pair.Value == selectedCount && string.CompareOrdinal(pair.Key, selectedNamespace) >= 0) continue;

                selectedNamespace = pair.Key;
                selectedCount = pair.Value;
            }

            return string.IsNullOrEmpty(selectedNamespace)
                ? CreateDefaultNamespace(productName)
                : selectedNamespace;
        }

        private static ArchitectureCodeGenerationPreview Invalid(ArchitectureCodeType codeType, string error)
        {
            return new ArchitectureCodeGenerationPreview(false, error, codeType, string.Empty, string.Empty,
                string.Empty, string.Empty);
        }

        private static string BuildClassName(ArchitectureCodeType codeType, string inputName)
        {
            var suffix = GetSuffix(codeType);
            if (string.IsNullOrEmpty(suffix) || inputName.EndsWith(suffix, StringComparison.Ordinal)) return inputName;
            return inputName + suffix;
        }

        private static string BuildAssetPath(ArchitectureCodeType codeType, string className, string outputRoot)
        {
            var folder = GetFolder(codeType);
            if (!string.IsNullOrEmpty(folder)) outputRoot += "/" + folder;
            return outputRoot + "/" + className + ".cs";
        }

        private static string BuildCode(ArchitectureCodeType codeType, string className, string codeNamespace,
            bool generateInterface)
        {
            var classBody = generateInterface && SupportsInterfaceGeneration(codeType)
                ? BuildInterfaceBody(codeType, className)
                : BuildPlainBody(codeType, className);

            return "using QFramework;\n\n" +
                   "namespace " + codeNamespace + "\n" +
                   "{\n" + classBody + "\n}\n";
        }

        public static bool SupportsInterfaceGeneration(ArchitectureCodeType codeType)
        {
            return codeType == ArchitectureCodeType.System
                || codeType == ArchitectureCodeType.Model
                || codeType == ArchitectureCodeType.Utility;
        }

        public static string GetRegisterMethodName(ArchitectureCodeType codeType)
        {
            switch (codeType)
            {
                case ArchitectureCodeType.System: return "RegisterSystem";
                case ArchitectureCodeType.Model: return "RegisterModel";
                case ArchitectureCodeType.Utility: return "RegisterUtility";
                default: return string.Empty;
            }
        }

        private static string BuildPlainBody(ArchitectureCodeType codeType, string className)
        {
            switch (codeType)
            {
                case ArchitectureCodeType.Architecture:
                    return "    public class " + className + " : Architecture<" + className + ">\n" +
                           "    {\n" +
                           "        protected override void Init()\n" +
                           "        {\n" +
                           "        }\n" +
                           "    }";
                case ArchitectureCodeType.System:
                    return "    public class " + className + " : AbstractSystem\n" +
                           "    {\n" +
                           "        protected override void OnInit()\n" +
                           "        {\n" +
                           "        }\n" +
                           "    }";
                case ArchitectureCodeType.Model:
                    return "    public class " + className + " : AbstractModel\n" +
                           "    {\n" +
                           "        protected override void OnInit()\n" +
                           "        {\n" +
                           "        }\n" +
                           "    }";
                case ArchitectureCodeType.Command:
                    return "    public class " + className + " : AbstractCommand\n" +
                           "    {\n" +
                           "        public " + className + "()\n" +
                           "        {\n" +
                           "        }\n\n" +
                           "        protected override void OnExecute()\n" +
                           "        {\n" +
                           "        }\n" +
                           "    }";
                case ArchitectureCodeType.Query:
                    return "    public class " + className + " : AbstractQuery<object>\n" +
                           "    {\n" +
                           "        public " + className + "()\n" +
                           "        {\n" +
                           "        }\n\n" +
                           "        protected override object OnDo()\n" +
                           "        {\n" +
                           "            return default(object);\n" +
                           "        }\n" +
                           "    }";
                case ArchitectureCodeType.Utility:
                    return "    public class " + className + " : IUtility\n" +
                           "    {\n" +
                           "    }";
                default:
                    throw new ArgumentOutOfRangeException("codeType", codeType, null);
            }
        }

        private static string BuildInterfaceBody(ArchitectureCodeType codeType, string className)
        {
            var interfaceName = "I" + className;
            var registerMethod = GetRegisterMethodName(codeType);
            var interfaceBody = "    {\n" +
                                "        // TODO: 在这里声明模块对外 API（属性/方法）\n" +
                                "    }\n\n";
            var headerComment = "    // 已生成模块接口 " + interfaceName + "，请在 Architecture.Init() 中按接口类型注册：\n" +
                                "    //     this." + registerMethod + "<" + interfaceName + ">(new " + className + "());\n";

            switch (codeType)
            {
                case ArchitectureCodeType.System:
                    return headerComment +
                           "    public interface " + interfaceName + " : ISystem\n" + interfaceBody +
                           "    public class " + className + " : AbstractSystem, " + interfaceName + "\n" +
                           "    {\n" +
                           "        protected override void OnInit()\n" +
                           "        {\n" +
                           "        }\n" +
                           "    }";
                case ArchitectureCodeType.Model:
                    return headerComment +
                           "    public interface " + interfaceName + " : IModel\n" + interfaceBody +
                           "    public class " + className + " : AbstractModel, " + interfaceName + "\n" +
                           "    {\n" +
                           "        protected override void OnInit()\n" +
                           "        {\n" +
                           "        }\n" +
                           "    }";
                case ArchitectureCodeType.Utility:
                    return headerComment +
                           "    public interface " + interfaceName + " : IUtility\n" + interfaceBody +
                           "    public class " + className + " : " + interfaceName + "\n" +
                           "    {\n" +
                           "    }";
                default:
                    return BuildPlainBody(codeType, className);
            }
        }

        private static string GetSuffix(ArchitectureCodeType codeType)
        {
            switch (codeType)
            {
                case ArchitectureCodeType.System: return "System";
                case ArchitectureCodeType.Model: return "Model";
                case ArchitectureCodeType.Command: return "Command";
                case ArchitectureCodeType.Query: return "Query";
                case ArchitectureCodeType.Utility: return "Utility";
                default: return string.Empty;
            }
        }

        private static string GetFolder(ArchitectureCodeType codeType)
        {
            switch (codeType)
            {
                case ArchitectureCodeType.System: return "System";
                case ArchitectureCodeType.Model: return "Model";
                case ArchitectureCodeType.Command: return "Command";
                case ArchitectureCodeType.Query: return "Query";
                case ArchitectureCodeType.Utility: return "Utility";
                default: return string.Empty;
            }
        }

        private static string ValidateInputName(string inputName)
        {
            if (string.IsNullOrEmpty(inputName)) return "请输入名字";
            if (CSharpKeywords.Contains(inputName)) return "名字不能是 C# 关键字";
            if (!IsIdentifier(inputName)) return "名字必须是有效的 C# 标识符";
            return string.Empty;
        }

        private static string ValidateNamespace(string codeNamespace)
        {
            if (string.IsNullOrEmpty(codeNamespace)) return "请输入命名空间";

            var segments = codeNamespace.Split('.');
            foreach (var segment in segments)
            {
                if (CSharpKeywords.Contains(segment)) return "命名空间不能包含 C# 关键字";
                if (!IsIdentifier(segment)) return "请输入有效的 C# 命名空间";
            }

            return string.Empty;
        }

        private static string ValidateOutputRoot(string outputRoot)
        {
            if (string.IsNullOrEmpty(outputRoot)) return "请输入 Assets/ 目录下的生成根目录";
            if (outputRoot != "Assets" && !outputRoot.StartsWith("Assets/", StringComparison.Ordinal))
                return "生成根目录必须位于 Assets/ 目录中";

            var segments = outputRoot.Split('/');
            foreach (var segment in segments)
            {
                if (string.IsNullOrEmpty(segment) || segment == "." || segment == "..")
                    return "生成根目录必须位于 Assets/ 目录中";
            }

            return string.Empty;
        }

        private static bool IsIdentifier(string value)
        {
            if (string.IsNullOrEmpty(value) || !(char.IsLetter(value[0]) || value[0] == '_')) return false;

            for (var index = 1; index < value.Length; index++)
            {
                if (!(char.IsLetterOrDigit(value[index]) || value[index] == '_')) return false;
            }

            return true;
        }

        private static string NormalizeAssetPath(string assetPath)
        {
            return (assetPath ?? string.Empty).Trim().Replace('\\', '/').TrimEnd('/');
        }
    }
}
#endif
