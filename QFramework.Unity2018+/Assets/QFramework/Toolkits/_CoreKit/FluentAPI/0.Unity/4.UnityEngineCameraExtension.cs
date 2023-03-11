/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.Camera", 4)]
    [APIDescriptionCN("UnityEngine.Camera 静态扩展")]
    [APIDescriptionEN("UnityEngine.Camera extension")]
#endif
    public static class UnityEngineCameraExtension
    {
#if UNITY_EDITOR
        // v1 No.151
        [MethodAPI]
        [APIDescriptionCN("截图")]
        [APIDescriptionEN("captureScreen")]
        [APIExampleCode(@"
Camera.main.CaptureCamera(new Rect(0, 0, Screen.width, Screen.height));
")]
#endif
        public static Texture2D CaptureCamera(this Camera camera, Rect rect)
        {
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;

            var screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            UnityEngine.Object.Destroy(renderTexture);

            return screenShot;
        }
    }
}