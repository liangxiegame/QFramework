using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class CreateILBehaviourCode
    {
        public static void DoCreateCodeFromScene(GameObject gameObject, bool genUIKit = false)
        {
            if (!gameObject)
            {
                Debug.LogWarning("需要选择 GameObject");
                return;
            }

            if (gameObject.GetComponent<Bind>() && !gameObject.GetComponent<ILKitBehaviour>())
            {
                var parentController = gameObject.GetComponentInParent<ILKitBehaviour>();

                if (parentController)
                {
                    gameObject = parentController.gameObject;
                }
            }

            Debug.Log("Create Code");

            var generateInfo = gameObject.GetComponent<ILKitBehaviour>();

            var scriptsFolder = Application.dataPath + "/Scripts";

            if (generateInfo)
            {
                scriptsFolder = generateInfo.ScriptsFolder;
            }

            scriptsFolder.CreateDirIfNotExists();


            var panelCodeInfo = new PanelCodeInfo {GameObjectName = generateInfo.name};

            Debug.Log(gameObject.transform);
            Debug.Log(panelCodeInfo.GameObjectName);

            // 搜索所有绑定
            BindCollector.SearchBinds(gameObject.transform, "", panelCodeInfo,null,typeof(ILKitBehaviour));

            if (genUIKit == false)
            {
                ILBehaviourCodeTemplate.Write(generateInfo.ScriptName, scriptsFolder, generateInfo.Namespace);
                ILBehaviourCodeDesignerTemplate.Write(generateInfo.ScriptName, scriptsFolder, panelCodeInfo,
                    generateInfo.Namespace);
            }
            else
            {
                ILUIPanelCodeTemplate.Write(generateInfo.ScriptName, scriptsFolder, generateInfo.Namespace);
                ILUIPanelCodeDesignerTemplate.Write(generateInfo.ScriptName, scriptsFolder, panelCodeInfo,
                    generateInfo.Namespace);
            }

            AssetDatabase.Refresh();
        }
    }
}