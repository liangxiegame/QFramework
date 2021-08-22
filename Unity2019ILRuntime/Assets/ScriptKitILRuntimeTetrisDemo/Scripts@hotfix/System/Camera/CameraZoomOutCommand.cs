using DG.Tweening;
using UnityEngine;

namespace QFramework.ILKitDemo.Tetris
{
    public class CameraZoomOutCommand : ILCommand<Tetris>
    {
        private readonly Camera mCamera;

        public CameraZoomOutCommand(Camera camera)
        {
            mCamera = camera;
        }

        public override void Execute()
        {
            mCamera.DOOrthoSize(18.48f, 0.5f);
        }
    }
}