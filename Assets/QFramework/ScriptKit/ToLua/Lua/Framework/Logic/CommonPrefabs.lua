--卡牌信息展示
local CommonPrefabs = class("CommonPrefabs")

function CommonPrefabs:ctor()
	self.mData = CreateWndData.New()
	self:Init()
end

function CommonPrefabs:Init()
	self.mData:LoadAllPrefabs("Prefabs/UI/Common")
	self.Prefabs = self.mData.Prefabs
end

function CommonPrefabs:GetPrefabs()
	return self.Prefabs
end

return CommonPrefabs.new()