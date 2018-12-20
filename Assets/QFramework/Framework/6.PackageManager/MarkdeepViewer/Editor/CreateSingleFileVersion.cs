using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

public class CreateSingleFileVersion
{
    [MenuItem("MarkdeepViewer/Build single file version")]
    static void Build()
    {
        string markdeepContent,
            darkStyleCSSContent,
            lightStyleCSSContent,
            docViewerContent,
            inspectorOverrideContent;

        if (!FindAssetContent("markdeep.min", out markdeepContent)
            || !FindAssetContent("light_style", out lightStyleCSSContent)
            || !FindAssetContent("dark_style", out darkStyleCSSContent)
            || !FindAssetContent("DocViewerWindow", out docViewerContent)
            || !FindAssetContent("TextInspectorBypass", out inspectorOverrideContent))
            return;

        StringBuilder fileResult = new StringBuilder();

        //This define will tell the system it use the single file version which will trigger copy of the needed file (markdeep, css etc.) to the Library folder
        fileResult.Append("#define SINGLE_FILE\n\n");

        //we append the using manually instead of trying to discover them from the file, as finding the duplicate would be tricky
        //TODO : make it more robust by actually parsing the required using so any change is correctly reflected here
        fileResult.Append(
            "using UnityEngine;\r\nusing UnityEditor;\r\nusing System;\r\nusing System.IO;\r\nusing System.Timers;\r\nusing System.Reflection;\r\n\n");

        int startOfClass = docViewerContent.IndexOf("public class");
        int endOfClass = docViewerContent.LastIndexOf('}');

        fileResult.Append(docViewerContent.Substring(startOfClass, endOfClass - startOfClass - 1));
        fileResult.Append("\n\n");
        fileResult.AppendFormat("\tstatic readonly string MARKDEEP_CONTENT = {0};\n", ToLiteral(markdeepContent));
        fileResult.AppendFormat("\tstatic readonly string LIGHT_STYLE_CONTENT = {0};\n", ToLiteral(lightStyleCSSContent));
        fileResult.AppendFormat("\tstatic readonly string DARK_STYLE_CONTENT = {0};\n", ToLiteral(darkStyleCSSContent));
        fileResult.Append("}\n\n");

        int startOfInspector = inspectorOverrideContent.IndexOf("[CustomEditor");
        fileResult.Append(inspectorOverrideContent.Substring(startOfInspector));

        System.IO.File.WriteAllText(Application.dataPath + "/../MarkdeepViewerSingleFile.cs", fileResult.ToString());
    }

    //return false if couldn't find the asset asked
    static bool FindAssetContent(string assetName, out string content)
    {
        string[] guids = AssetDatabase.FindAssets(assetName);
        if (guids.Length == 0)
        {
            Debug.LogErrorFormat("Couldn't find {0} file in the project", assetName);
            content = "";
            return false;
        }

        content = System.IO.File.ReadAllText(AssetDatabase.GUIDToAssetPath(guids[0]).Replace("Assets", Application.dataPath));
        return true;
    }

    private static string ToLiteral(string input)
    {
        using (var writer = new StringWriter())
        {
            using (var provider = CodeDomProvider.CreateProvider("CSharp"))
            {
                provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                return writer.ToString();
            }
        }
    }
}
