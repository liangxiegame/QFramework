FRAMEWORK_INITED = false

if not FRAMEWORK_INITED then
	require ("Framework/Function")
	require ("Framework/Define")
	require ("Framework/Utility")
	require ("Framework/MsgDispatcher")
	require ("Framework/FSM")	
	-- require ("Framework/Logic/Command/ICommand") -- 暂时先不处理消息
	require ("Framework/LuaBehaviour")


	--创建lua文件
	function CreateLuaFile(luaFilePath,gameObject)
		log("CreateLuaFile:"..luaFilePath)
		local luaTable = nil
		luaTable = require(luaFilePath).new()
		return luaTable
	end

	FRAMEWORK_INITED = true

	-- ResMgr.Init();
	--=======================TEST Init===========================
	log("Init Sucess")
	-- Resources
	UIMgr.OpenPanel("Resources/TestView")

	-- AssetBundle
	-- UIMgr.OpenPanel("TestView",QFramework.UILevel.PopUI,"testview_prefab")
	--=======================================================
end 