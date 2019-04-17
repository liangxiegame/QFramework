
local LinkedList = class("LinkedList")

function LinkedList:ctor()
    self.mLength = 0;
    self.mLast = nil;
    self.mFirst = nil;
end

function LinkedList:Head()
    if nil ~= self.mFirst then
        return self.mFirst;
    end
    return nil;
end

function LinkedList:Tail()
    if nil ~= self.mLast then
        return self.mLast;
    end
    return nil;
end

function LinkedList:Length()
    return self.mLength;
end

--向链表尾部添加节点
function LinkedList:Push(data)   
    local node = {value = data,next = nil,prev = nil}
    if nil == self.mLast then   
        self.mLast = node;
        self.mFirst = node;
    else
        self.mLast.next = node;
        node.prev = self.mLast;
        self.mLast = node;
    end
    self.mLength = self.mLength + 1;
    return node;
end

-- 弹出尾部节点
function LinkedList:Pop()
    if nil == self.mLast then 
        colorlog("linkedlist is empty","#FF000000")
        return;
    end
    local node = self.mLast;
    if nil ~= node.prev then
        node.prev.next = nil;
        self.mLast = node.prev;
        node.prev = nil;
    else
        --说明是第一个节点了，
        self.mFirst = nil;
        self.mLast = nil;
    end
    self.mLength = self.mLength - 1;
    if self.mLength == 0 then
        self.mFirst = nil
        self.mLast = nil
    end
    return node;
end

function LinkedList:Insert(i,data)
    local findNode = self:Find(i);
    if nil == findNode then
        return;
    end

    local node = {value = data,next = nil,prev = nil}
    node.next = findNode.next;
    findNode.next = node;
    findNode.next.prev = node;
    node.prev = findNode;
    self.mLength = self.mLength + 1;
end

function LinkedList:FindIndex(value)
    local curNode = self.mFirst;
    for i = 1,self.mLength do
        if curNode and curNode.value == value then
            return i
        end
        curNode = curNode.next
    end
    return nil
end

function LinkedList:Remove(i)
    local findNode = self:Find(i);
    if nil == findNode then
        return;
    end
    if findNode.next then
        findNode.next.prev = findNode.prev;
    end
    if findNode.prev then
        findNode.prev.next = findNode.next; 
    end
    if i == 1 then
        self.mFirst = findNode.next
    end
    if i == self.mLength then
        self.mLast = findNode.prev
    end
    self.mLength = self.mLength - 1;
end

function LinkedList:Iter(node)
    if nil == node then
        return nil
    end
    return node.next;
end

function LinkedList:ReverseIter(node)
    if nil == node then
        return nil;
    end
    return node.prev;
end

function LinkedList:Find(i)
     if i < 1 or i > self.mLength then
        colorlog("linked list insert pos is invalid","#FF000000");
        return nil;
    end

    local j = 0;
    local curNode = self.mFirst;
    while j < i -1 do
        j = j+1;
        curNode = curNode.next;
        if nil == curNode.next then
            break;
        end
    end

    if j ~= i - 1  then
        colorlog("linked list in pos "..tostring(i).."is invalid","#FF000000")
        return nil;
    end

    return curNode;
end


function LinkedList:PrintElement()
    local h = self.mFirst
    local r = self.mLast
    local str = nil
    while h ~= r do
        str = "{"..h.value:GetName();
        h = h.next;   
    end
    if nil ~= r then
        if nil == str then
            str = "{"..r.value:GetName().."}"
        else
            str = str..","..r.value:GetName().."}"
        end
    elseif nil ~= str then
        str = str .. "}"
    end
    if nil ~= str then
        log(str)
    end
end

return LinkedList;