local FrameTimer = class("FrameTimer",VTimer)

function FrameTimer:ctor(callback)
    self:Init(callback);
end

function FrameTimer:Update(deltaTime)
    if(nil ~= self.mCallback) then  
        self.mCallback(deltaTime)
    end
end

return FrameTimer