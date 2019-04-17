local WindowMgr = class("WindowMgr")

function WindowMgr:ctor()
    self.mMultiViewMap = require("Logic/Common/HashMap").new(); -- 多层同型View存放
    self.mWndList = {}  --全屏窗口列表
    self.mWndArry = require("Logic/Common/LinkedList").new()
    self.waitingNum = 0
    self.mWaitingView = nil
end

function WindowMgr:GetPrefab(prefabName,callback,param)
    local kWndDataTbl = {Body = CreateWndData.New()};
    if nil == param[UIConst.Prefabs ] then
        kWndDataTbl['Name'] = NotiConst.CREATE_WINDOW;
    else
        kWndDataTbl['Name'] = NotiConst.GET_WINDOW_PREFAB;
        kWndDataTbl['Body'].Prefabs = param[UIConst.Prefabs]
    end
    kWndDataTbl["Body"].PrefabName = prefabName;
	kWndDataTbl["Body"].WndName = prefabName;
    kWndDataTbl['Body'].Parent = param[UIConst.Parent];
	kWndDataTbl["Body"].Type = WindowType.Prefab;
    kWndDataTbl["Body"].Async = false;
	kWndDataTbl["Body"].LuaCallback =
	function(gameObject)
        if nil ~= callback then
            callback(gameObject);
        else
            logError("no callback");
        end
	end;
	LuaHelper.SendMessageCommand(kWndDataTbl);
end

function WindowMgr:getView(wndName)
    return self.mWndList[wndName]
end

-- isMultiView 该参数管理了会出现多层叠加的View，设置为false则可以忽略Mgr对该View的管理
function WindowMgr:ShowWindow(wndName,param,isMultiView)  
    if nil == self.mWndList then
        self.mWndList = {}
    end
    if nil == self.mWndArry then
        self.mWndArry = {}
    end
    local viewObj = self.mWndList[wndName];
    if nil ~= viewObj and isMultiView == nil then
        log("ShowWindow "..wndName)
        viewObj:CheckRePush();
        -- viewObj:InitRequest(function()end)
        viewObj:Show();
    else
        viewObj = nil
        log("CreatWindow "..wndName)
        viewObj = LuaHelper.DoFile(wndName);
        if(nil ~= viewObj) then 
            viewObj:Create(param);
            viewObj:SetFileName(wndName);
        end

        if isMultiView ~= nil and isMultiView == false then
            return viewObj
        end

        if self.mWndList[wndName] then
            self:putInMultiViewMap(wndName,self.mWndList[wndName],viewObj)
        end
        self.mWndList[wndName] = viewObj;
    end
    return viewObj
end

function WindowMgr:GetWindow(wndName)
	return self.mWndList[wndName]
end

function WindowMgr:HideWindow(viewObj)
    viewObj:Hide();
end

--[[destory 表示要不要删除gameobject,release 表示要不要释放数据]]
function WindowMgr:CloseWindow(wndObj,destory,release)
    if nil == wndObj then
        return;
    end

    wndObj:Destroy(destory);
    local wndName = wndObj:GetFileName();
	log("CloseWindow "..wndName)
    if nil == wndName or nil == self.mWndList[wndName] then
        return;
    end
    if nil == release or true == release then
        if self:removeMultiViewMap(wndName) then
            self.mWndList[wndName] = nil;
        else
            local viewObj = self:getTailMultiView(wndName)
            if viewObj then
                self.mWndList[wndName] = viewObj
            end
        end
    end
end

function WindowMgr:putInMultiViewMap(wndName,orgViewObj,newViewObj)
    log("putInMultiViewMap")
    local list = self.mMultiViewMap:Get(wndName)
    if not list then
        list = {orgViewObj}
    end
    -- list[#list]:Hide()
    table.insert(list,newViewObj)
    self.mMultiViewMap:Put(wndName,list)
end

function WindowMgr:removeMultiViewMap(wndName)
    if self.mMultiViewMap == nil then
        return true
    end

    local list = self.mMultiViewMap:Get(wndName)
    if list == nil then
        return true
    end
    table.remove(list)
    if list[1] == nil then
        self.mMultiViewMap:Remove(wndName)
        return true
    end
    self.mMultiViewMap:Put(wndName,list)
    -- if  not list[#list]:getStash() then
    --     list[#list]:Show()
    -- end
    return false
end

function WindowMgr:getTailMultiView(wndName)
    local list = self.mMultiViewMap:Get(wndName)
    if list == nil or list[1] == nil then
        return nil
    end
    return list[#list]
end

function WindowMgr:SetStashMgr(mgr)
    if  self.mMultiViewMap == nil then
        self.mMultiViewMap = require("Logic/Common/HashMap").new();
    end
    self.mStashMgr = mgr;
    self.mWaitingView = nil
    self.waitingNum = 0
end

function WindowMgr:GetStashMgr()
    return self.mStashMgr;
end

function WindowMgr:DestroyStashMgr()
    if self.mStashMgr then
        self.mStashMgr:Destroy();
        self.mStashMgr = nil;
    end

    for k, v in pairs(self.mWndList) do
        if nil ~= v then
            while self.mWndList[k] do
                self:CloseWindow(self.mWndList[k], true)
            end
        end 
    end
    self:ctor()
end

function WindowMgr:ShowTips(msg)
    UIUtil:showTips(nil,nil,msg)
end

function WindowMgr:ShowWaiting(cb,ingore)
	if not self.mWaitingView then
        self.mWaitingView = self:ShowWindow(WindowNameConst.WaitingView,{cb = cb});
        if ingore then else
            self.waitingNum = 1
        end
    else
        if ingore then else
            self.waitingNum = self.waitingNum + 1
        end
        self.mWaitingView:OnShow()
    end
end

function WindowMgr:HideWaiting(ingore)
    if self.mWaitingView then
        if ingore then else
            self.waitingNum = self.waitingNum - 1
        end
        if self.waitingNum <= 0 then
            if self.mWaitingView.mUIRoot then
                self:CloseWindow(self.mWaitingView,true)
                self.mWaitingView = nil
                self.waitingNum = 0
            else
                self.mWaitingView = nil
                self.waitingNum = 0
            end
        end
    end
end

function WindowMgr:ShowDialog(x,callback)
	--local param = {showBtn=true,callback=callback}
	--self:ShowTips(x,param)
end

-- 执行跳转
-- ToJumpType = {
--     Recruit = 1, -- 招募
--     Store = 2, -- 商城
--     Fight = 3 -- 竞技
-- }
function WindowMgr:DoJump(Type)
    if Type == ToJumpType.Recruit then
		self:ShowWindow(WindowNameConst.RecruitView, {stash = true});	
	elseif Type == ToJumpType.Store then
		self:ShowWindow(WindowNameConst.StoreMainView, {stash = true});	
	elseif Type == ToJumpType.Fight then 
		self:ShowWindow(WindowNameConst.FightMainView, {stash = true});
	end
end

function WindowMgr:CloseTillHome()
    self:DestroyStashMgr()
	local msg = 
	{
		Name = NotiConst.REMOVE_UIATTACH
	}
    LuaHelper.SendMessageCommand(msg);
    self:SetStashMgr(require('Logic/Common/StashMgr').new())
    self:ShowWindow(WindowNameConst.HomeView,{stash=true});
end

return WindowMgr.new()