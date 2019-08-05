using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace QFramework
{
    /// <summary>
    /// 图片最优尺寸
    /// </summary>

    public class TextureHalveAtlas : MonoBehaviour
    {
        public static int CompressQuality = 50;
        public static float halveRate = 0.5f;
        private static bool IsFinish = false;

        [MenuItem("QFramework/Tool/UI/最优图片尺寸")]
        public static void HalveAtlas()
        {
            int errorDirPathCount = 0;

            IsFinish = false;
            
            UnityEngine.Object[] objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets); //获取选择文件夹
            for (int i = 0; i < objects.Length; i++)
            {
                string dirPath = AssetDatabase.GetAssetPath(objects[i]).Replace("\\", "/");
                if (!Directory.Exists(dirPath))
                {
                    errorDirPathCount++;

                    if (i == objects.Length - 1 && errorDirPathCount > 0) { EditorUtility.DisplayDialog("错误", "选择正确文件夹！", "好的"); }

                    continue;
                }

                if (i == objects.Length - 1) { IsFinish = true; }

                HalveSprite(dirPath);
            }
        }

        private static void HalveSprite(string dirPath)
        {
            string[] files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                filePath = filePath.Replace("\\", "/");

                if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg"))
                {
                    //筛选出png和jpg图片
                    EditorUtility.DisplayProgressBar("处理中>>>", filePath, (float)i / (float)files.Length);

                    TextureImporter textureImporter = AssetImporter.GetAtPath(filePath) as TextureImporter;
                    if (textureImporter == null) { return; }

                    //判断图片有无alpha通道，有默认格式设置成：RGBA16；无默认格式设置成：RGB16
#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
                    textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
#endif
                    
                    AssetDatabase.ImportAsset(filePath);

                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                    int textureSize = Math.Max(texture.height, texture.width);

                    TextureImporterFormat defaultTextureFormat = TextureImporterFormat.RGB16;
                    if (texture.format == TextureFormat.RGB24)
                    {
                        //no alpha
                        defaultTextureFormat = TextureImporterFormat.RGB16;
                    }
                    else
                    {
                        defaultTextureFormat = TextureImporterFormat.RGBA16;
                    }

#if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
                    #region OLD Settings
                    TextureImporterSettings settings = new TextureImporterSettings();
                    textureImporter.ReadTextureSettings(settings);

                    textureImporter.textureType = TextureImporterType.Default;
                    int defaultMaxTextureSize = textureImporter.maxTextureSize; //settings.maxTextureSize;
                    defaultMaxTextureSize = Math.Min(textureSize, defaultMaxTextureSize);
                    defaultMaxTextureSize = (int)(defaultMaxTextureSize * halveRate);

                    settings.textureFormat = defaultTextureFormat;
                    settings.maxTextureSize = GetValidSize(defaultMaxTextureSize);
                    //textureImporter.maxTextureSize = GetValidSize(defaultMaxTextureSize);

                    #region IOS Android
                                        ////int androidMaxTextureSize = 0;
                                        ////TextureImporterFormat androidTextureFormat = UnityEditor.TextureImporterFormat.ETC_RGB4;
                                        ////bool isAndroidOverWrite = textureImporter.GetPlatformTextureSettings("Android", out androidMaxTextureSize, out androidTextureFormat);
                                        ////if (true == isAndroidOverWrite)
                                        ////{
                                        ////    androidMaxTextureSize = Math.Min(textureSize, androidMaxTextureSize);
                                        ////    androidMaxTextureSize = (int)(androidMaxTextureSize * halveRate);
                                        ////    textureImporter.SetPlatformTextureSettings("Android", GetValidSize(androidMaxTextureSize), androidTextureFormat, CompressQuality);
                                        ////}

                                        ////int iphoneMaxTextureSize = 0;
                                        ////TextureImporterFormat iphoneTextureFormat = UnityEditor.TextureImporterFormat.PVRTC_RGBA4;
                                        ////bool isIphoneOverWrite = textureImporter.GetPlatformTextureSettings("iPhone", out iphoneMaxTextureSize, out iphoneTextureFormat);
                                        ////if (true == isIphoneOverWrite)
                                        ////{
                                        ////    iphoneMaxTextureSize = Math.Min(textureSize, iphoneMaxTextureSize);
                                        ////    iphoneMaxTextureSize = (int)(iphoneMaxTextureSize * halveRate);
                                        ////    textureImporter.SetPlatformTextureSettings("iPhone", GetValidSize(iphoneMaxTextureSize), iphoneTextureFormat, CompressQuality);
                                        ////}
                    #endregion
                    textureImporter.SetTextureSettings(settings);
                    #endregion

#else
                    #region NEW Settings
                    TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
                    setting = textureImporter.GetDefaultPlatformTextureSettings();

                    textureImporter.textureType = TextureImporterType.Default;
                    int defaultMaxTextureSize = textureImporter.maxTextureSize;
                    defaultMaxTextureSize = Math.Min(textureSize, defaultMaxTextureSize);
                    defaultMaxTextureSize = (int)(defaultMaxTextureSize * halveRate);

                    setting.format = defaultTextureFormat;
                    setting.maxTextureSize = GetValidSize(defaultMaxTextureSize);
                    
                    textureImporter.SetPlatformTextureSettings(setting);
                    #endregion
#endif

                    AssetDatabase.SaveAssets();
                    DoAssetReimport(filePath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                }
            }

            if (IsFinish)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("成功", "处理完成！", "好的");
            }
        }

        private static int GetValidSize(int size)
        {
            int result = 0;
            if (size <= 48)
            {
                result = 32;
            }
            else if (size <= 96)
            {
                result = 64;
            }
            else if (size <= 192)
            {
                result = 128;
            }
            else if (size <= 384)
            {
                result = 256;
            }
            else if (size <= 768)
            {
                result = 512;
            }
            else if (size <= 1536)
            {
                result = 1024;
            }
            else if (size <= 3072)
            {
                result = 2048;
            }

            return result;
        }

        public static void DoAssetReimport(string path, ImportAssetOptions options)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportAsset(path, options);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}