module("GameState",package.seeall)
local MainTownState = class('MainTownState',IState)

function MainTownState:OnEnter()
    local _prepareFunc = function()
        self:OnPrepare();
    end

    local _loadFinishFunc = function()
        self:OnLoadFinish();
    end

    local _loadResFunc = function()
        return self:OnLoadRes();
    end

    LuaHelper.SceneTransition("MainTown","Common/Loading/Loading",_prepareFunc,_loadFinishFunc,_loadResFunc,false);
end

function MainTownState:OnPrepare()
    WindowMgr:DestroyStashMgr();
    WindowMgr:SetStashMgr(require('Logic/Common/StashMgr').new())
end

function MainTownState:OnLoadFinish()
    WindowMgr:ShowWindow(WindowNameConst.HomeView,{stash=true});
end 

function MainTownState:OnLoadRes()
    return nil;
end

function MainTownState:OnExit()
    WindowMgr:DestroyStashMgr();
end

return MainTownState.new()

