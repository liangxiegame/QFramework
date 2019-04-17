local Stack = class("Stack")

function Stack:ctor()
    self.mTable = {}
end

function Stack:Push(element)
    local iSize = self:Size();
    self.mTable[iSize+1] = element;    
end

function Stack:Pop()
    local iSize = self:Size()
    if self:IsEmpty() then  
        colorlog("Error: Stack is empty","#FF000000");
        return ;
    end
    return table.remove( self.mTable,iSize );
end

function Stack:Top()
    local iSize = self:Size();
    if self:IsEmpty() then  
        colorlog("Error: Stack is empty","#FF000000");
        return ;
    end
    return self.mTable[iSize];
end

function Stack:IsEmpty()
    local iSize = self:Size()
    if 0 == iSize then
        return true;
    end
    return false;
end

function Stack:Size()
    return #self.mTable;
end

function Stack:Clear()
    self.mTable = nil ;
    self.mTable = {};
end

function Stack:PrintElement()
    local iSize = self:Size();
    if self:IsEmpty() then  
        colorlog("Error: Stack is empty","#FF000000");
    end

    local str = "{" ..self.mTable[iSize];
    iSize = iSize - 1;
    while(iSize > 0) do 
        str = str..", "..self.stack_table[iSize]
        iSize = iSize - 1
    end
    str = str.."}"
    log(str);
end

return Stack;
