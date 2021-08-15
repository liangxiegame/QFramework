using UnityEngine;
using DG.Tweening;

namespace QFramework.ILKitDemo.Tetris
{
    public class CameraManager
    {

        private Camera mainCamera;

        public CameraManager(Transform parentTransform)
        {
            mainCamera = parentTransform.Find("Main Camera").GetComponent<Camera>();
        }

        //放大
        public void ZoomIn()
        {
            mainCamera.DOOrthoSize(14, 0.5f);
        }

        //缩小
        public void ZoomOut()
        {
            mainCamera.DOOrthoSize(18.48f, 0.5f);
        }
    }
}