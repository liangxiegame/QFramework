VTimer = class("VTimer")


function VTimer:ctor()
    self:Init(nil);
end

function VTimer:Init(callback)
    self.mCallback = callback
    self.mWaitForRemove = false;
    self.mTimeToStop = false;
end

function VTimer:Remove()
    self.mWaitForRemvoe = true;
end

function VTimer:ReadyForRemove()
    return self.mWaitForRemvoe;
end

function VTimer:TimeToStop()
    self.mTimeToStop = true
end

function VTimer:TimeToPlay()
    self.mTimeToStop = false
end

function VTimer:HasStopped()
    return self.mTimeToStop
end