local TimerMgr = class("TimeMgr")
require("Common/VTimer")
function TimerMgr:ctor()
    self.mTimerList = {};
end

function TimerMgr:AddFrameTimer(callback)
    local frameTimer = require("Common/FrameTimer").new(callback)
    table.insert(self.mTimerList,frameTimer);
    return frameTimer;
end

function TimerMgr:AddDelayTimer(delayTime,timeCount,callback,firstNotDelay)
    local delayTimer = require("Common/DelayTimer").new(delayTime,timeCount,callback,firstNotDelay)
    table.insert( self.mTimerList,delayTimer )
    return delayTimer;
end

function TimerMgr:RemoveTimer(timer)
    if nil ~= timer then
        timer:Remove()
    end
end

function TimerMgr:StopTimer(timer)
    if nil ~= timer then
        timer:TimeToStop()
    end
end

function TimerMgr:PlayTimer(timer)
    if nil ~= timer then
        timer:TimeToPlay()
    end
end

function TimerMgr:Update(deltaTime)
	table.foreach(self.mTimerList,function (_,v)
		if not v:ReadyForRemove() and not v:HasStopped() then
			v:Update(deltaTime);
		end
	end)

	table.eraseBy(self.mTimerList,function (v)
		return v:ReadyForRemove()
	end)
end

--服务器给的是毫秒，os.time() 是秒
function TimerMgr:SetServerTime(svrLoginTime)
	self.mSvrLoginTime = Mathf.floor(svrLoginTime/1000)
	self.mClientLoginTime = os.time()
	log(string.format("Set server time [%d], local time [%d]",self.mSvrLoginTime,self.mClientLoginTime))
end

function TimerMgr:GetServerTime()
	if self.mSvrLoginTime then
		return os.time() + self.mSvrLoginTime - self.mClientLoginTime
	else
		logError("Cannot get server time until login game server!")
		return os.time()
	end
end

return TimerMgr.new();