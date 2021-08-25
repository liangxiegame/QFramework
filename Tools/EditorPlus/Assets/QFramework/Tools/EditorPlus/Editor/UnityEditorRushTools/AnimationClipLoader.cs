using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ModelImporterClipAnimation = UnityEditor.ModelImporterClipAnimation;

internal static class AnimationClipLoader
{
    [MenuItem("Assets/Animation Clip Loader/加载")]
    private static void 加载() { 加载(strFilePath, modelImporter); }

    private static ModelImporter modelImporter;
    private static string strFilePath;
    private static ModelImporterClipAnimation animationy原;
    //如果想直接在Unity中看到文本则改为.txt格式，
    //ProjectSettings->Editor->C# Project排除txt，
    //避免每次修改都更新一下C#工程
    private const string strExtension = ".txt";

    [MenuItem("Assets/Animation Clip Loader/加载", true)]
    private static bool 加载验证()
    {
        if (!Selection.activeObject)
        {
            return false;
        }

        strFilePath = AssetDatabase.GetAssetPath(Selection.activeObject);
        //如果可以建立模型导入器，就是模型文件
        modelImporter = AssetImporter.GetAtPath(strFilePath) as ModelImporter;
        if (!modelImporter)
        {
            return false;
        }

        //看看模型有默认动画？
        return modelImporter.defaultClipAnimations.Length != 0;
    }

    [MenuItem("Assets/Animation Clip Loader/导出")]
    private static void 导出() { 导出(strFilePath, modelImporter); }

    [MenuItem("Assets/Animation Clip Loader/导出", true)]
    private static bool 导出验证() { return 加载验证(); }

    private static void 加载(string strFilePath, ModelImporter modelImporter)
    {
        List<string> listConfigData = new List<string>(16);
        try
        {
            string strModeFileFullPath =
                Application.dataPath.Remove(Application.dataPath.LastIndexOf('/') + 1) + strFilePath;
            string strConfigFileFullPath =
                strModeFileFullPath.Remove(strModeFileFullPath.LastIndexOf('.')) +
                string.Format(".Animation{0}", strExtension);
            using (Stream file = File.Open(strConfigFileFullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    listConfigData.Clear();
                    while (!reader.EndOfStream)
                    {
                        listConfigData.Add(reader.ReadLine());
                    }
                }
            }
        }
        catch
        {
            Debug.LogError(string.Format("{0}.Animation{1}文档加载失败", Selection.activeObject.name, strExtension));
            return;
        }

        Dictionary<string, ModelImporterClipAnimation> dictAnimations = new Dictionary<string, ModelImporterClipAnimation>(32);
        for (int i = 0; i < listConfigData.Count; i++)
        {
            string data = listConfigData[i];
            string[] strData = data.Split('-');
            if (strData.Length < 3)
            {
                Debug.LogError(string.Format("{0}数据分割错误", data));
                continue;
            }

            int firstFrame;
            if (!int.TryParse(strData[0], out firstFrame))
            {
                Debug.LogError(string.Format("{0}第1个数据有误", data));
                continue;
            }

            int lastFrame;
            if (!int.TryParse(strData[1], out lastFrame))
            {
                Debug.LogError(string.Format("{0}第2个数据有误", data));
                continue;
            }
            
            if (strData[2].Equals(string.Empty))
            {
                Debug.LogError(string.Format("{0}第3个数据有误，请检查文件编码", data));
                continue;
            }

            string name = strData[2].Replace("\r", string.Empty);
            if (dictAnimations.ContainsKey(name))
            {
                Debug.LogError(string.Format("{0}名称重复", data));
                continue;
            }

            WrapMode wrapMode = WrapMode.Default;
            //循环模式（可选）
            if (strData.Length > 3)
            {
                switch (strData[3])
                {
                    case "Loop":
                        wrapMode = WrapMode.Loop;
                        break;
                    case "PingPong":
                        wrapMode = WrapMode.PingPong;
                        break;
                    case "ClampForever":
                        wrapMode = WrapMode.ClampForever;
                        break;
                    case "Once":
                        wrapMode = WrapMode.Once;
                        break;
                    default:
                        break;
                }
            }

            //填入数据【开始帧-结束帧-命名】
            ModelImporterClipAnimation animation = new ModelImporterClipAnimation();
            animation.firstFrame = firstFrame;
            animation.lastFrame = lastFrame;
            animation.wrapMode = wrapMode;
            animation.loopTime = wrapMode == WrapMode.Loop;
            animation.name = name;
            dictAnimations.Add(name, animation);
        }

        List<ModelImporterClipAnimation> listAnimations原 = new List<ModelImporterClipAnimation>(modelImporter.clipAnimations);
        //原来的跟新的比较，重复的就修改，不重复的就删除
        for (int i = 0; i < listAnimations原.Count; i++)
        {
            ModelImporterClipAnimation animation原 = listAnimations原[i];
            bool is重复 = false;
            foreach (KeyValuePair<string, ModelImporterClipAnimation> kvpClip in dictAnimations)
            {
                ModelImporterClipAnimation animation新 = kvpClip.Value;
                //修改原来的
                if (animation原.name == animation新.name)
                {
                    animation原.firstFrame = animation新.firstFrame;
                    animation原.lastFrame = animation新.lastFrame;
                    animation原.wrapMode = animation新.wrapMode;
                    animation原.loopTime = animation新.loopTime;
                    is重复 = true;
                    break;
                }
            }

            //循环出来之后都不匹配的就是多余的，把他删了
            if (!is重复)
            {
                listAnimations原.RemoveAt(i);
                i--;
            }
        }

        //listAnimations新用来对应文本的顺序
        List<ModelImporterClipAnimation> listAnimations新 = new List<ModelImporterClipAnimation>(dictAnimations.Count);
        listAnimations新 = new List<ModelImporterClipAnimation>(dictAnimations.Count);
        foreach (KeyValuePair<string, ModelImporterClipAnimation> kvpClip in dictAnimations)
        {
            bool is重复 = false;
            for (int i = 0; i < listAnimations原.Count; i++)
            {
                animationy原 = listAnimations原[i];
                if (animationy原.name == kvpClip.Key)
                {
                    listAnimations新.Add(animationy原);
                    is重复 = true;
                    break;
                }
            }

            if (!is重复)
            {
                listAnimations新.Add(kvpClip.Value);
            }
        }

        //刷新界面：更改Selection.activeObject为null再换回去
        UnityEngine.Object modelObject = Selection.activeObject;
        Selection.activeObject = null;
        modelImporter.clipAnimations = listAnimations新.ToArray();
        modelImporter.SaveAndReimport();
        Selection.activeObject = modelObject;

        //注：当clipAnimations减少时使用下面代码刷新后，检视面板报错
        //EditorUtility.SetDirty(modelObject);
        //2019版本刷新检视面板还需要下面代码
//#if UNITY_2019_1_OR_NEWER
//        InternalEditorUtility.RepaintAllViews();
//#endif
    }

    private static void 导出(string strFilePath, ModelImporter modelImporter)
    {
        ModelImporterClipAnimation[] animations = modelImporter.clipAnimations;
        try
        {
            string strModeFileFullPath =
                Application.dataPath.Remove(Application.dataPath.LastIndexOf('/') + 1) + strFilePath;
            string strConfigFileFullPath = strModeFileFullPath.Remove(strModeFileFullPath.LastIndexOf('.')) +
                                           string.Format(".Animation.bac{0}", strExtension);
            using (Stream file = File.Open(strConfigFileFullPath, FileMode.Create))
            {
                using (StreamWriter reader = new StreamWriter(file))
                {
                    if (animations.Length > 0)
                    {
                        //GUILayout.Label("当前动画：", EditorStyles.largeLabel);
                        for (int i = 0; i < animations.Length; i++)
                        {
                            ModelImporterClipAnimation animation = animations[i];
                            reader.WriteLine("{0:000}-{1:000}-{2}-{3}", animation.firstFrame, animation.lastFrame, animation.name, animation.wrapMode);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        AssetDatabase.Refresh();
    }
}