module("Common", package.seeall)
require("Common/ktJson")
local TableConfig = class("TableConfig")

--local LocalStorage = require("Logic/Common/LocalStorage")

local CommonTableJsonMD5PrefsKey = "common_md5"
local CommonTableJsonFilename = "/common.table"
local LocalCommonTableJsonFilename = "/local_common.table"

local function WriteTableConfigFile(strText)
	local t_file,error = io.open(UnityEngine.Application.persistentDataPath .. CommonTableJsonFilename, "wb")
	if t_file then
		t_file:write(strText)
		t_file:close()
		return true
	else
		logWarn(error)
		return false
	end
end

function TableConfig:ReadTableConfigFile()
	local t_file,err = io.open(UnityEngine.Application.persistentDataPath .. LocalCommonTableJsonFilename, "rb")
	--log(UnityEngine.Application.persistentDataPath .. LocalCommonTableJsonFilename)
	if t_file then
		self.haveLocal=true
	else
		self.haveLocal=false
		t_file,error = io.open(UnityEngine.Application.persistentDataPath .. CommonTableJsonFilename, "rb")
	end

	if t_file then
		local content = t_file:read("*a")
		t_file:close()
		return LuaHelper.ConvertBytesToString(NetManager.LuaDecompressGZip(content))
	else
		logWarn(error)
		return nil
	end
end

function TableConfig:ctor()
	self:InitSetting()
	self.mTblCfgs = {}
	self.md5 = LocalStorage:getValue(CommonTableJsonMD5PrefsKey,"")
	if #self.md5 > 0 then
		local t_tableText = self:ReadTableConfigFile()
		if not t_tableText or not self:ParseCommonTable(t_tableText) then
			self.md5 = ""
		end
	end
end

function TableConfig:ParseCommonTable(strText,md5,fromSvr)
	if fromSvr and self.haveLocal then
		return
	end
	
	local strOrigin = strText
	if fromSvr then
		strOrigin = LuaHelper.ConvertBytesToString(NetManager.LuaDecompressGZip(strText))
	end
	
	local list = json.decode(strOrigin);
	if not list then
		return false
	end

	for i,v in ipairs(list) do
		local name = v["name"];
		if name then
			self.mTblCfgs[name] = table.readonlyTab(self:HandleCommonTable(name,v["data"]))

			if fromSvr then
				log("from server, loaded Common Table "..name);
			else
				if self.haveLocal then
					log("from test local disk, loaded Common Table "..name);
				else
					log("from local disk, loaded Common Table "..name);					
				end
			end
		end
	end
	
	if fromSvr then
		self.md5 = md5
		LocalStorage:setValue(CommonTableJsonMD5PrefsKey,md5)
		WriteTableConfigFile(strText)
	end

	return true
end

function TableConfig:HandleCommonTable(name,tabData)
	local data = {}

	local keyStr = self.settings[tostring(name)]
	if keyStr then
		if type(keyStr) == "string"  then
			local keys = string.split(keyStr, ",")
			local len = #keys
			for _,vv in ipairs(tabData) do
				local key1 = vv[tostring(keys[1])]
				if len == 1 then
					data[tostring(key1)] = vv
				elseif len == 2 then
					if not data[tostring(key1)] then
						data[tostring(key1)] = {}
					end
					local key2 = vv[tostring(keys[2])]
					data[tostring(key1)][tostring(key2)] = vv
				elseif len == 3 then
					if not data[tostring(key1)] then
						data[tostring(key1)] = {}
					end
					local key2 = vv[tostring(keys[2])]
					if not data[tostring(key1)][tostring(key2)] then
						data[tostring(key1)][tostring(key2)] = {}
					end
					local key3 = vv[tostring(keys[3])]
					data[tostring(key1)][tostring(key2)][tostring(key3)] = vv
				end
			end
		else
			data = tabData
		end
	else
		for _,vv in ipairs(tabData) do
			local id = vv["id"]
			data[tostring(id)] = vv
		end
	end
	return data
end	

function TableConfig:InitSetting()
	self.settings = {
		common_player_star = "star_level",
		common_player_relation_detail = "relation_id,relation_level",
		common_player_relation_info = "player_id,id",
		common_player_grow_base = "type,star,gradation",
		common_team_club = "icon",
		common_equipment_uplevel = "level",
		common_story_career_plot = "plot,step",
		common_username = true,
        common_player_formation = true,
		common_player_level = true,
		common_ranked_race_award = true,
		common_tour_pve_difficulty = true,
		common_tour_pve_grid = true,
		common_tour_pve_list = true,
		--common_tour_pve_map = true,
		common_tour_pve_map = "map_no,league,position_no",
		common_debris_playerInfo = "player_id",
		common_player_pve_section = "type,sort_id",
		common_player_pve_award = "type,section_id",
		common_player_pve_custom = "type,section_id,custom_id",
		--common_diamond_cost = "type,order",
		common_vip = "level",
		common_player_skill = "pid",
		common_player_skill_cost = "quality,position,level",
		common_player_skill_maxlevel = "is_player,condition",
		common_player_tacit_understanding_level = true,
		common_player_tacit_understanding_add_value = true,
		common_player_tacit_understanding_extra = "total_tacit_level",
		common_equipment_skill_uplevel = "position,level",
		common_equipment_skill_refresh = "num,locked",
		common_property_add_up_cost = "type,order",
		common_recycling_property = "type,star",
		common_user_level = "user_level",
		common_advert_media = true,
		-- common_advert_media_chat = "chat_type",
		common_first_recharge = "club_id",
		common_vip_package = "club_id,vip_level",
		common_ranked_race_grade = "grade",
		common_get_tili = true,
		common_level_package = "club,level",
		common_limited_level_package = true,
		common_story_career_banner = "club,chapter",
		common_player_gradation_rate = "overall",
		common_player_quality = "quality",
		common_champion_road_checkpoint = "type,checkpoint",
		common_practice_daily_detail = "competition_id,sort_id",
		common_seven_days_mark = "days,id",
		common_seven_days_task = "mark_id,id",
		common_football_facilities = true,
		common_subcourt_info = true,
		common_football_facilities_level = "type,level",
		common_football_field = true,
		common_employee = true,
		common_employee_skill = true,
		common_manager_building = "level",
	}	
end

--加载json表
function TableConfig:OnLoadTable(tblName, strText)
    -- log("load Table "..tostring(tblName));
    -- --log("Content  "..tostring(strText));
	-- local strContent = json.decode(strText);
	-- if self.mTblCfgs[tblName] ~= nil then
	-- 	log("Table "..tostring(tblName).." has been loaded");
	-- end;
	-- self.mTblCfgs[tblName] = strContent;
	-- strContent = nil;
end

-- 一个获取数据表的函数
function TableConfig:GetTable(tblName)
    assert(tblName ~= nil, "name is nil")
    return self.mTblCfgs[tblName]
end

function TableConfig:GetRow(tblName,key)
    local kTbl = self:GetTable(tblName)
    if nil == kTbl then
        logError('Get Table '..tblName..' Failed');
        return nil
    end
    assert(key ~= nil, "key is nil")
    return kTbl[key]
end

function TableConfig:GetValue(tblName,rowKey,colKey)
    local kRowTbl = self:GetRow(tblName,rowKey)
    if nil == kRowTbl then
        logError('Get Row '..rowKey..' Failed at table:'..tblName .."Rowkey: "..rowKey.." and Colkey: "..colKey);
        return nil
    end
    assert(colKey ~= nil, "colKey is nil")
    return kRowTbl[colKey]
end

-- load 测试json数据
function TableConfig:LoadJson(fileName,callback)
	local strText = ""
	local url = UnityEngine.Application.persistentDataPath .."/".. fileName

	local t_file,err = io.open(url, "rb")
	if t_file then
		strText = t_file:read("*a")
		t_file:close()
	end

	local tab = json.decode(strText)
	if callback then
		callback(tab)
		return
	end

	return tab
end

function TableConfig:LoadJsonString(fileName,callback)
	local strText = ""
	local url = UnityEngine.Application.persistentDataPath .."/".. fileName

	local t_file,err = io.open(url, "rb")
	if t_file then
		strText = t_file:read("*a")
		t_file:close()
	end

	if callback then
		callback(strText)
		return
	end

	return strText
end

function TableConfig:SaveJson(fileName,strText)
	local url = UnityEngine.Application.persistentDataPath .."/".. fileName
	if type(strText) == "table" then
		strText = json.encode(strText)
	end
	local t_file,error = io.open(url, "wb")
	if t_file then
		t_file:write(strText)
		t_file:close()
	end
end

return TableConfig.new()
