namespace QFramework{
	using UnityEngine;
	using UnityEditor;
    using System.Text.RegularExpressions;

    public class LuaCodeGenerator{
		private const int mToLua = 1; 
		private const string mFileSuffix = ".lua";
		private static string mFilePath = LuaConst.luaDir + ScriptBaseSetting.ScriptPath;
		private static IBaseTemplate[] mTemplates = {new LuaUIPanelCodeTemplate() as IBaseTemplate,new LuaPanelComponentsCodeTemplate() as IBaseTemplate,new LuaPanelTemplate() as IBaseTemplate};

		private static ScriptKitCodeBind mScriptCodeBind = (GameObject uiPrefab,string filePath)=>{addScriptComponent(uiPrefab,filePath);};

		[MenuItem("Assets/@Script Kit - Create ToLua Code")]
		public static void CreateHotScriptCode(){
			Debug.Log("<color=#EE6A50> >>>>>>>Create ToLua Code  </color>");
			var info = new ScriptKitInfo();
			info.HotScriptType = mToLua;
			info.HotScriptFilePath = mFilePath;
			info.HotScriptSuffix = mFileSuffix;
			info.Templates = mTemplates;
			info.CodeBind = mScriptCodeBind;
			UICodeGenerator.CreateScriptUICode(info);
		}

		private static void addScriptComponent(GameObject uiPrefab,string filePath){
			var lc = uiPrefab.GetComponent<LuaComponent>();
			var uiLuaPanel =  uiPrefab.GetComponent<UILuaPanel>();
			if(lc.IsNull()){
				lc = uiPrefab.AddComponent<LuaComponent>();
				var newPath = filePath;
				var resultString = Regex.Split(newPath, "/Lua/", RegexOptions.IgnoreCase);
				newPath = resultString[1];
				newPath = newPath.Replace(".lua", "");
				newPath = newPath.Replace("/", ".");
				lc.LuaPath = newPath;
				lc.LuaFilePath = filePath;
			}	
			Debug.Log("<color=#EE6A50> >>>>>>>Success ToLua Code </color>");
		} 
	}
}
