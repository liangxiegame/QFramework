local StashMgr = class("StashMgr")

function StashMgr:ctor()
    self.mWndList = require("Logic/Common/LinkedList").new()
end

function StashMgr:GetWndCnt()
    return self.m_WndCnt;
end

function StashMgr:PushWnd(wndScript,param)
    local _script = self.mWndList:Tail();
    if nil ~= _script and nil ~= _script.value then
        if(nil == param or nil == param[UIConst.HidePreWindow] or true == param[UIConst.HidePreWindow]) then
            WindowMgr:HideWindow(_script.value)
        end
    end
    self.mWndList:Push(wndScript);
    AudioUtil:PlayShowPageAudio() 
end

function StashMgr:PopWnd()    
    local _script = self.mWndList:Pop();
    if nil ~= _script then
        WindowMgr:CloseWindow(_script.value,true)
    end
    _script = self.mWndList:Tail();
    if nil ~= _script then
        WindowMgr:ShowWindow(_script.value:GetFileName())
    end
end

function StashMgr:RePush(wndScript,param)
    local index = self.mWndList:FindIndex(wndScript)
    if index and index ~= self.mWndList:Length() then
        self.mWndList:Remove(index)
        self:PushWnd(wndScript,param)
        return true
    end
    return false
end

function StashMgr:Restore()
    local kScript = self.mWndList:Tail();
    if nil ~= kScript then
        WindowMgr:ShowWindow(kScript.value:GetFileName())
    end
end

function StashMgr:Destroy()
    local _node = self.mWndList:Tail()
    while(nil ~= _node) do 
        WindowMgr:CloseWindow(_node.value,true,false)
        _node = self.mWndList:ReverseIter(_node)
    end
end

return StashMgr