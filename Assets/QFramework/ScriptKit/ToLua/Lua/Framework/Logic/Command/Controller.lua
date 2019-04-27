module("Command",package.seeall)

local Controller = class("Controller")

function Controller:ctor()
    self.mCmdMap = {}
end

function Controller:ExecuteCmd(kMsg)
    if nil == kMsg or nil == kMsg.Name then
        return;
    end
    local kViewTbl = self.mCmdMap[kMsg.Name]
    if nil == kViewTbl then
        print("no cmd with msg name = "..kMsg.Name)
        return;
    end
    
    for key,value in pairs(kViewTbl) do
        if nil ~= value then    
            value:OnMessage(kMsg)
        end
    end
end
-- register view command
function Controller:RegisterCommand(cmdName,view)
    if nil == self.mCmdMap[cmdName] then
        self.mCmdMap[cmdName] = {}
    end
    self.mCmdMap[cmdName][tostring(view)] = view
end

-- remove view command
function Controller:RemoveCommand(cmdName,view)
    if nil == view then
        colorlog("Error: RemoveCommand view is nil","#FF000000")
    end
    if(nil == self.mCmdMap[cmdName]) then
        return; 
    end
    self.mCmdMap[cmdName][tostring(view)] = nil
end

function Controller:PrintCommand()
    for k,v in pairs(self.mCmdMap) do 
        local kList = v;
        for i,j in pairs(kList) do 
            log(j:GetName())
        end
    end
end

return Controller.new()