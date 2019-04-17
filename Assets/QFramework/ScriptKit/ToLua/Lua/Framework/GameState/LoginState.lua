module("GameState",package.seeall)

local LoginState = class("LoginState",IState)

function LoginState:OnEnter()
    local _prepareFunc = function()
        self:OnPrepare()
    end

    local _loadFinishFunc = function()
        self:OnLoadFinish()
    end
    
    local _loadResFunc = function() 
        return self:OnLoadRes();
    end
    LuaHelper.SceneTransition("Login","Common/Loading/Loading",_prepareFunc,_loadFinishFunc,_loadResFunc,false)
end

function LoginState:OnPrepare()
    WindowMgr:DestroyStashMgr();
    WindowMgr:SetStashMgr(require('Logic/Common/StashMgr').new())
    self.mLoginView = WindowMgr:ShowWindow(WindowNameConst.LoginView, {[UIConst.StashFlag] = true});
end

function LoginState:OnLoadFinish()
end

function LoginState:OnLoadRes()
    return nil;
end

function LoginState:OnExit()
    -- WindowMgr:CloseWindow(self.mLoginView,true);
end

return LoginState.new();
