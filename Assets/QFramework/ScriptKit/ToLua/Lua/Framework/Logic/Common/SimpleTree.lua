

local SimpleTree = class("SimpleTree")

function SimpleTree:ctor()
    self.mParent = nil
    self.mChildren = nil
    self.mData = nil
    self.mNext = nil
end

function SimpleTree:Init(data)
    self.mData = data;
end

function SimpleTree:WayToRoot(func)
    if nil ~= self.mParent then
        self.mParent:WayToRoot(func);
    end

    if nil ~= func then
        func(self);
    end
end
function SimpleTree:AddChild(child)
    if (nil == self.mChildren) then
        self.mChildren = {};
    end

    child.mParent = self;
    table.insert(self.mChildren, child);
end


function SimpleTree:RmvChild(child)
    if (self.mChildren ~= nil) then
        for k, v in pairs(self.mChildren) do
            if (v == child) then
                self.mChildren[k] = nil;
                break;
            end
        end
    end
end

return SimpleTree