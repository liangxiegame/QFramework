module("Command",package.seeall)

local CommandManager = class("CommandManager")

function CommandManager:ctor()  
    self.mController = require("logic/Command/Controller")
    self.mPostCmdList = {}
    self:RegisterCommand(CommandConst.CloseWindow,require("logic/Command/CloseWindowCommand"))
end

function CommandManager:RegisterCommand(cmdName,view)
    self.mController:RegisterCommand(cmdName,view)
end

function CommandManager:RemoveCommand(cmdName,view)
    self.mController:RemoveCommand(cmdName,view)
end

function CommandManager:SendCommand(type,value)
    local param = {};
    param['Name'] = type
    param['Body'] = value;
    self.mController:ExecuteCmd(param); 
end

function CommandManager:PostCommand(type,value)
    local param = {}
    param['Name'] = type
    param['Body'] = value;
    table.insert( self.mPostCmdList,param )
end

function CommandManager:Update()
    local curCmdList = {}
    for i = 1, #self.mPostCmdList, 1 do 
        local param = self.mPostCmdList[i]; 
        table.insert(curCmdList, param)    
    end
    self.mPostCmdList = {};
    for i = 1, #curCmdList, 1 do       
        local param = curCmdList[i]
        self.mController:ExecuteCmd(param); 
    end
end

return CommandManager.new()