--
-- Author: Your Name
-- Date: 2016-10-29 00:25:32
--
local GameObject = UnityEngine.GameObject;
local Transform = UnityEngine.Transform;
local Debug = UnityEngine.Debug;

--工具类
Utility = {};

--设置父子关系
--childTrans: 子节点，Transform
--parentTrans: 父节点，Transform
function Utility.SetParent(childTrans, parentTrans)
	if not childTrans then
		Debug.LogError("SetParent childTrans is nil");
		return;
	end
	if not parentTrans then
		Debug.LogError("SetParent parentTrans is nil");
		return;
	end
	childTrans.parent = parentTrans;
end

--设置父子关系，并归一，位移归零，旋转归零，缩放归一
--childTrans: 子节点，Transform
--parentTrans: 父节点，Transform
function Utility.SetParentNormally(childTrans, parentTrans)
	Utility.SetParent(childTrans, parentTrans);
	childTrans.localPosition = Vector3.zero;
	childTrans.localRotation = Quaternion.identity;
	childTrans.localScale = Vector3.one;
end

--简化创建LuaBehaviour并关联的接口
--gameObj: 游戏对象
--luaTable: Lua对象
--...: 不定参数，传递到Awake(...)，供初始化使用
function Utility.CreateLuaBehaviour(gameObj, luaObj, ...)
	if not gameObj then
		return nil;
	end
	if not luaObj then
		return nil;
	end
	
	local luaBeh = gameObj:AddComponent("LuaBehaviour");
	luaBeh.table = luaObj;
	--缓存gameObject与transform
	luaObj.gameObj = gameObj;
	luaObj.trans = gameObj.transform;
	--模拟Awake声明周期
	if luaObj.Awake then
		luaObj:Awake(...);
	end
	return luaObj;
end