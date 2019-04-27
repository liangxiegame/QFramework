module("GameState",package.seeall)

require("Common/LuaExtension")
require("GameState/IState")

local StateMgr =  class("StateMgr",ICommand)

function StateMgr:Init()
	self.mStateList = {}
	self.mCurState = nil;
	self.mStateList['Init'] = LuaHelper.DoFile("GameState/InitState")
	self.mStateList['Login'] = LuaHelper.DoFile("GameState/LoginState")
	self.mStateList['MainTown'] = LuaHelper.DoFile("GameState/MainTownState")
	self:RegisterEventHandler();
end

function StateMgr:EnterState(kState)
	if nil ~= self.mCurState then
		log("State "..self.mCurState.__cname.." Exit")
		self.mCurState:OnExit()
    else
        log("cur state is nill")
	end
	
	self.mCurState = self.mStateList[kState]
		
	if self.mCurState then
		log("State "..self.mCurState.__cname.." Enter")
		self.mCurState:OnEnter()	
    else
        log("cur state is nil 2")	
	end
end

function StateMgr:Update(fDeltaTime)
	if nil ~= self.mCurState then
		self.mCurState:OnUpdate(fDeltaTime)		
	end
end

function StateMgr:OnMessage(kMsg)
	if nil == kMsg then
		return;
	end
	if kMsg.Name == CommandConst.EnterState then 
        self:EnterState(kMsg.Body)
    end
end

-- 事件消息注册
function StateMgr:RegisterEventHandler()
    CommandManager:RegisterCommand(CommandConst.EnterState,self);
end
-- 事件消息注销
function StateMgr:RemoveEventHandler()
    CommandManager:RemoveCommand(CommandConst.EnterState,self);
end

function StateMgr:Uninit()
	self:RemoveEventHandler()
	for k,v in pairs(self.mStateList) do
		v = nil
	end
end

return StateMgr.new()