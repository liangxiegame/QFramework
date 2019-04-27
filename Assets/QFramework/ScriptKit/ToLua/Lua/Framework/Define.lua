--
-- Author: Your Name
-- Date: 2016-10-29 14:54:12
--
QUtil = QFramework.QUtil;
QAppConst = QFramework.QAppConst;
QLuaHelper = QFramework.QLuaHelper;
QUIHelper = QFramework.UIHelper


QLuaBehavihour = QFramework.QLuaBehavihour
-- QUIEventListener = QFramework.UI.UGUIEventListener

WWW = UnityEngine.WWW;
GameObject = UnityEngine.GameObject;
Resources = UnityEngine.Resources;
Image = UnityEngine.UI.Image;
Input = UnityEngine.Input;
Screen = UnityEngine.Screen
Camera = UnityEngine.Camera
PlayerPrefs = UnityEngine.PlayerPrefs
--UI
ScrollRect = UnityEngine.UI.ScrollRect
Toggle = UnityEngine.UI.Toggle
ToggleEvent = UnityEngine.UI.Toggle.ToggleEvent
RectTransform = UnityEngine.RectTransform
--2D
Sprite = UnityEngine.Sprite

Rect = UnityEngine.Rect

function ccp(x,y)
	QFramework.QUtil.Log(Screen.width..":"..Screen.height)
	return Vector2.New(x - 320,y - 480)
end




TexturePool = {}


UIMgr = QFramework.UIMgr
ResMgr = QFramework.ResMgr
AudioMgr = QFramework.AudioManager
