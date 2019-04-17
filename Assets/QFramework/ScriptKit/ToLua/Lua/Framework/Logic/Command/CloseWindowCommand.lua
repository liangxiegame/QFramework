module("Command",package.seeall)

local CloseWindowCommand = class("CloseWindowCommand",ICommand)

function CloseWindowCommand:OnMessage(kMsg)
    if nil == kMsg or nil == kMsg.Body then 
        return 
    end
    WindowMgr:CloseWindow(kMsg.Body.object,kMsg.Body.destory,kMsg.Body.release)
end

return CloseWindowCommand.new()