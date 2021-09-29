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
            panel.LogoName.AnchorPosY(-160.3f);
            panel.MenuUI.Show();
            (panel.MenuUI.transform as RectTransform)
                .AnchorPosY(66.64f);
            
            SendCommand(new CameraZoomOutCommand(panel.MainCamera));
        }
    }
}