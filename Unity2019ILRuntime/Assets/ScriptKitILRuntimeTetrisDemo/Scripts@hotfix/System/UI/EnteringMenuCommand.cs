using DG.Tweening;
using UnityEngine;

namespace QFramework.ILKitDemo.Tetris
{
    public class EnteringMenuCommand : ILCommand<Tetris>
    {
        public override void Execute()
        {
            var panel = ILUIKit.GetPanel<UITetrisPanel>();
            panel.LogoName.Show();
            panel.LogoName.DOAnchorPosY(-160.3f, 0.5f);
            panel.MenuUI.Show();
            (panel.MenuUI.transform as RectTransform)
                .DOAnchorPosY(66.64f, 0.5f);
            
            SendCommand(new CameraZoomOutCommand(panel.MainCamera));
        }
    }
}