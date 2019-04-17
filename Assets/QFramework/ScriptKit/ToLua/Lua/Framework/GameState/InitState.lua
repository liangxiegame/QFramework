module("GameState",package.seeall)

local InitState = class("InitState",IState)

function InitState:OnEnter ()
    if nil == self.timer then
        self.timer = 0.1;
    end

    AudioUtil:InitVolume()
    -- self.mSplashView = WindowMgr:ShowWindow(WindowNameConst.SplashView);
end

function InitState:OnUpdate(fDeltaTime)
	self.timer = self.timer - fDeltaTime	
	if self.timer < 0 then 
        CommandManager:PostCommand(CommandConst.EnterState,'Login')
	end
end

function InitState:OnExit()
    -- WindowMgr:CloseWindow(self.mSplashView,true);
end
return InitState.new();