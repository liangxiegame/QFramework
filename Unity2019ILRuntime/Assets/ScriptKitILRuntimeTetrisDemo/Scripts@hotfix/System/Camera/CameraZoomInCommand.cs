using DG.Tweening;
using UnityEngine;

namespace QFramework.ILKitDemo.Tetris
{
    public class CameraZoomInCommand : ILCommand<Tetris>
    {
        private readonly Camera mCamera;

        public CameraZoomInCommand(Camera camera)
        {
            mCamera = camera;
        }
        
        public override void Execute()
        {
            mCamera.DOOrthoSize(14, 0.5f);
        }
    }
}