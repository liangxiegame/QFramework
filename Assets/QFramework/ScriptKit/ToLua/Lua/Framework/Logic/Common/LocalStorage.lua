module("Common", package.seeall)

local LocalStorage = class("LocalStorage")

function LocalStorage:setValue(key, value)
	local temp = {value = value}
	UnityEngine.PlayerPrefs.SetString(key, json.encode(temp))
end

function LocalStorage:getValue(key, defaultValue)
	local ret = UnityEngine.PlayerPrefs.HasKey(key);
	if ret then
		local temp = UnityEngine.PlayerPrefs.GetString(key);
		local tempJ = json.decode(temp);
		if not tempJ then
			return temp
		end
		return tempJ["value"];
	else
		return defaultValue;
	end
end

function LocalStorage:delValue(key)
	UnityEngine.PlayerPrefs.DeleteKey(key);
end

function LocalStorage:deleteAll()
	UnityEngine.PlayerPrefs.DeleteAll();
end	

return LocalStorage.new() 