module("GameState",package.seeall)

IState = class("IState")

function IState:OnEnter()
	
end

function IState:OnUpdate(fDeltaTime)
	
end

function IState:OnExit()
	
end

function IState:LoadTable(tblName,tblContent)
    TableConfig:OnLoadTable(tblName,tblContent)
end