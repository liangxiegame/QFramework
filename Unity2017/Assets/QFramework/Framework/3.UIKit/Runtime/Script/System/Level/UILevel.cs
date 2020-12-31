using System;

namespace QFramework
{
    
    public enum UILevel
    {
        [Obsolete]
        AlwayBottom = -3, //如果不想区分太复杂那最底层的UI请使用这个
        Bg = -2, //背景层UI
        [Obsolete]
        AnimationUnderPage = -1, //动画层
        Common = 0, //普通层UI
        [Obsolete]
        AnimationOnPage = 1, // 动画层
        PopUI = 2, //弹出层UI
        [Obsolete]
        Guide = 3, //新手引导层
        [Obsolete]
        Const = 4, //持续存在层UI
        [Obsolete]
        Toast = 5, //对话框层UI
        [Obsolete]
        Forward = 6, //最高UI层用来放置UI特效和模型
        [Obsolete]
        AlwayTop = 7, //如果不想区分太复杂那最上层的UI请使用这个

        // 一个 Panel 就是一个 Canvas 的 Panel
        CanvasPanel = 100, // 
    }
}