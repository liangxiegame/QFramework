local DelayTimer = class("DelayTimer",VTimer)

--[[
    mDelayTime 延迟触发 
    mLoopCount 循环次数(0:无限循环)
]]
function DelayTimer:ctor(delayTime,loopCount,callback,firstNotDelay)
    self:Init(callback);
    self.mDelayTime = delayTime;
    self.mLoopCount = loopCount;
    self.firstNotDelay = firstNotDelay;
    self.mElapseTime = 0;
    self.mCurCount = 0;
end

function DelayTimer:Update(deltaTime)
    if self.mCurCount >= self.mLoopCount and self.mLoopCount > 0 then
        self.mWaitForRemove = true;
        return;
    end
    if self.firstNotDelay then
        self.mElapseTime = self.mDelayTime;
        self.firstNotDelay = false
    else
        self.mElapseTime = self.mElapseTime + deltaTime;
    end  
    if(self.mElapseTime >= self.mDelayTime) then
        self.mElapseTime = 0    --self.mElapseTime - self.mDelayTime;
        self.mCurCount = self.mCurCount + 1;
        if(nil ~= self.mCallback) then
            self.mCallback(deltaTime);
        end
    end
end

return DelayTimer