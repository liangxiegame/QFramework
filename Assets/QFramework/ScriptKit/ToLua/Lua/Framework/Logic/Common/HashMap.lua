
local HashMap = class("HashMap")

function HashMap:ctor()
	self.mLength = 0;
	self.mData = {};
	self.mArray = {};
end

function HashMap:IsEmpty()
	return self.mLength == 0;
end

function HashMap:ContainsKey(key)
	local ret = false;
	if self.mData[key] then
		ret = true;
	end
	return ret;
end

function HashMap:Put(key, value)
	if value == nil then
		return;
	end
	
	if self:ContainsKey(key) == false then
		self.mLength = self.mLength + 1;
		table.insert(self.mArray, value);
	else
		local oldValue = self.mData[key];
		local index = table.indexof(self.mArray, oldValue);
		if index then
			self.mArray[index] = value;
		end
	end
	self.mData[key] = value;
end

function HashMap:Get(key)
	if self:ContainsKey(key) == false then
		return nil;
	end
	return self.mData[key];
end

function HashMap:Remove(key)
	if self:ContainsKey(key) == false then
		return;
	end
	local oldValue = self.mData[key];
	local index = table.indexof(self.mArray, oldValue);
	if index then
		table.remove(self.mArray, index);
	end
	
	self.mData[key] = nil;
	self.mLength = self.mLength - 1;
end

function HashMap:Size()
	return self.mLength;
end

--遍历
function HashMap:Each(cb)
	cb = cb or function() end	
	
	for k, v in pairs(self.mData) do
		cb(k, v);
	end
end

--排序
function HashMap:Sort(cb)
	cb = cb or function() end	
	
	table.sort(self.mArray, function(a, b)
		return cb(a, b);
	end)
	
	return self.mArray;
end

function HashMap:Array()
	return self.mArray;
end

function HashMap:Index(index)
	return self.mArray[index];
end

function HashMap:ArrayEach(cb)
	cb = cb or function() end	
	for i, v in ipairs(self.mArray) do
		cb(i, v)
	end
end

function HashMap:Clear()
	self.mData = {};
	self.mArray = {};
	self.mLength = 0;
end

return HashMap; 