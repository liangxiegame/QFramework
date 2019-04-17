function class(classname,super)
	local superType = type(super)
	local cls
	if super then
		cls = {}
		setmetatable(cls, {__index = super})
		cls.super = super
	else
		cls = {ctor = function() end}
	end

	cls.__cname = classname
	cls.__index = cls

	function cls.new(...)
		local instance = setmetatable({}, cls)
		instance.super = cls.super
		instance:ctor(...)
		return instance
	end
	return cls	
end

function clone(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local newObject = {}
        lookup_table[object] = newObject
        for key, value in pairs(object) do
            newObject[_copy(key)] = _copy(value)
        end
        local metatable = getmetatable(object)
        if metatable then
            metatable.__newindex = nil
        end
        return setmetatable(newObject,metatable)
    end
    return _copy(object)
end

function IsTableEmpty( tableToTest )
    if(tableToTest==nil)then
        return true;
    end
    return (next(tableToTest) == nil);
end

--输出日志--
function log(str)
    if not ServerDef.DEBUG_MODEL then return end
	if type(str) == "string" or type(str) == "number" then
		LuaHelper.Log(str .."\n"..debug.traceback( ));
	else
		dump(str)
	end
end 

function colorlog(str,color)
    if not ServerDef.DEBUG_MODEL then return end 
	str = str .. "\n" .. debug.traceback( );
	str = string.gsub(str,"\n","</color>\n<color="..color..">")
	LuaHelper.ColorLog(str,color);
end

--错误日志--
function logError(str)
    if not ServerDef.DEBUG_MODEL then return end 
	LuaHelper.LogError(str.."\n"..debug.traceback( ));
end

--警告日志--
function logWarn(str)
    if not ServerDef.DEBUG_MODEL then return end 
	LuaHelper.LogWarning(str.."\n"..debug.traceback( ));
end

local function dump_value_(v)
    if type(v) == "string" then
        v = "\"" .. v .. "\""
    end
    return tostring(v)
end
-- 暂时屏蔽掉所以dump
function vdump(value, desciption, nesting)
    if not ServerDef.DEBUG_MODEL then return end
end

--打印lua table--
function dump(value, desciption, nesting)
    if not ServerDef.DEBUG_MODEL then return end
    if type(nesting) ~= "number" then nesting = 3 end

    local lookupTable = {}
    local result = {}

    local traceback = string.split(debug.traceback("", 2), "\n")
    log("dump from: " .. string.trim(traceback[3]))

    local function dump_(value, desciption, indent, nest, keylen)
        desciption = desciption or "<var>"
        local spc = ""
        if type(keylen) == "number" then
            spc = string.rep(" ", keylen - string.len(dump_value_(desciption)))
        end
        if type(value) ~= "table" then
            result[#result +1 ] = string.format("%s%s%s = %s", indent, dump_value_(desciption), spc, dump_value_(value))
        elseif lookupTable[tostring(value)] then
            result[#result +1 ] = string.format("%s%s%s = *REF*", indent, dump_value_(desciption), spc)
        else
            lookupTable[tostring(value)] = true
            if nest > nesting then
                result[#result +1 ] = string.format("%s%s = *MAX NESTING*", indent, dump_value_(desciption))
            else
                result[#result +1 ] = string.format("%s%s = {", indent, dump_value_(desciption))
                local indent2 = indent.."    "
                local keys = {}
                local keylen = 0
                local values = {}
                for k, v in pairs(value) do
                    keys[#keys + 1] = k
                    local vk = dump_value_(k)
                    local vkl = string.len(vk)
                    if vkl > keylen then keylen = vkl end
                    values[k] = v
                end
                table.sort(keys, function(a, b)
                    if type(a) == "number" and type(b) == "number" then
                        return a < b
                    else
                        return tostring(a) < tostring(b)
                    end
                end)
                for i, k in ipairs(keys) do
                    dump_(values[k], k, indent2, nest + 1, keylen)
                end
                result[#result +1] = string.format("%s}", indent)
            end
        end
    end
    dump_(value, desciption, "- ", 1)

    local temp = "";
    for i, line in ipairs(result) do
        temp = temp.."\n"..line
    end
    log(temp)
end

function handler(obj, method)
    return function(...)
        return method(obj, ...)
    end
end

function string.split(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter=='') then return false end
    local pos,arr = 0, {}
    -- for each divider found
    for st,sp in function() return string.find(input, delimiter, pos, true) end do
        table.insert(arr, string.sub(input, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(input, pos))
    return arr
end

function string.ltrim(input)
    return string.gsub(input, "^[ \t\n\r]+", "")
end

function string.rtrim(input)
    return string.gsub(input, "[ \t\n\r]+$", "")
end

function string.trim(input)
    input = string.gsub(input, "^[ \t\n\r]+", "")
    return string.gsub(input, "[ \t\n\r]+$", "")
end

function table.nums(t)
    local count = 0
    for k, v in pairs(t) do
        count = count + 1
    end
    return count
end

function table.indexof(array, value, begin)
    for i = begin or 1, #array do
        if array[i] == value then return i end
    end
    return false
end

--四舍五入
function math.round(value)
    value = tonumber(value) or 0
    value = value + 0.5
    return Mathf.floor(value)
end

-- table改造
function table.refit(tab)
    local mtab = {}
    mtab.__add = function (a,b)
        local c = {}
        for k,v in pairs(a) do
            c[k] = a[k] + b[k]
        end
        setmetatable(c,mtab)
        return c
    end
    setmetatable(tab,mtab)
    return tab
end

function table.readonlyTab(tab)
    if type(tab) ~= "table" then
        return tab
    end
    for k,v in pairs(tab) do
        if type(tab[k]) == "table" then
            tab[k] = table.readonlyTab(tab[k])
        end
    end
    local mt = getmetatable(tab)
    if mt then
        mt.__newindex = function (t,k,v)
                        logError("readonly table!")
                        UIUtil:showTips(nil,{type = TipPopType.HaveBtn},"只读table不可修改("..tostring(k)..")，请改代码")
                    end
    end
    return tab
end

function table.erase(t,val)
	table.eraseBy(t,function(v) return v == val end)
end
-- 根据条件删除kvTab中数据
function table.eraseBy(t,cb)
	for k,v in pairs(t) do
		if cb(v) then
			t[k] = nil
		end
	end
end

-- KV变数组
function table.toArray(tab)
    local array = {}
    for k,v in pairs(tab) do
        table.insert(array,v)
    end 
    return array
end

-- 将 2000-01-01 00:00:00 这种形式的字符串转换成os.time()
-- 之后一个个弃用掉此功能，转换有上限不适合被使用
-- 默认为北京时间+8
function totime(timeStr,isSecond)
    if isSecond and type(timeStr) ~= "string" then
        return timeStr
    end
    if type(timeStr) ~= "string" then
        return timeStr/1000
    end

    local timeTab = {
        y = string.sub(timeStr,1,4),
        m = string.sub(timeStr,6,7),
        d = string.sub(timeStr,9,10),
        h = string.sub(timeStr,12,13),
        mm = string.sub(timeStr,15,16),
        ss = string.sub(timeStr,18,19),
    }
    local dt = os.time {
        year = timeTab.y,
        month = timeTab.m,
        day = timeTab.d,
        hour = timeTab.h,
        min = timeTab.mm,
        sec = timeTab.ss
    }
    dt = dt - 16 * 3600 + os.time{year = 1970, month = 1, day = 2, hour = 0}
    return dt
end

-- 三目表达式
function math.ab(condition,a,b)
    return (condition and {a} or {b})[1]
end

--转二进制tab
function math.toBinaryTab(num)
    local binaryTab = {}
    local index = 1
    local d,p = math.modf(num/2)
    while d>=1  do
        if p ~= 0 then
            binaryTab[index] = 1
        else
            binaryTab[index] = 0
        end
        index = index + 1
        d,p = math.modf(d/2)
    end
    if p ~= 0 then
        binaryTab[index] = 1
    else
        binaryTab[index] = 0
    end
    return binaryTab
end

-- 数字转中文 0~99
function string.numtoChiness(num)
    local chinessNum = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "百", "千", "万", "亿"}
    num = tonumber(num)
    local numStr = tostring(num)
    local length = #numStr
    if length == 1 then
        return chinessNum[num]
    elseif length == 2 then
        if num == 10 then
           return chinessNum[num] 
        elseif num > 10 and num < 20 then
            return "十"..chinessNum[tonumber(string.sub(numStr,2,2))]
        else
            if num%10 == 0 then
                return chinessNum[tonumber(string.sub(numStr,1,1))].."十"
            else
                return chinessNum[tonumber(string.sub(numStr,1,1))].."十"..chinessNum[tonumber(string.sub(numStr,2,2))]
            end
        end
    end
end

--是否包含
function table.contains(tab,val)
    for k,v in pairs(tab) do
        if v == val then
            return true
        end
    end
    return false
end

function second2DHMS(second)
    local d = Mathf.floor(second / (24 * 3600))
    local h = Mathf.floor(second % (24 * 3600) / 3600)
    local m = Mathf.floor(second % 3600 / 60)
    local s = Mathf.floor(second % 60)

    local res = ""
    if d > 0 then
        res = res..d.."天"
    end
    if h > 0 then
        res = res..h.."时"
    else
        if d > 0 then
            res = res..h.."时"
        end
    end
    if m > 0 then
        res = res..m.."分"
    else
        if d > 0 or h > 0 then
            res = res..m.."分"
        end
    end

    if d <= 0 then
        res = res..s.."秒"
    end
    
    return res
end

--秒转换成时间戳数据
function second2Date(second)
    local date = {}
    date.day = Mathf.floor(second / (24 * 3600))
    date.hour = Mathf.floor(second % (24 * 3600) / 3600)
    date.min = Mathf.floor(second % 3600 / 60)
    date.sec = Mathf.floor(second % 60)
    return date
end

--秒转换成时间戳数据
function second2HMS(second)
    local h = Mathf.floor(second / 3600)
    local m = Mathf.floor((second - 3600*h) / 60)
    local s = second - 3600*h - m*60
    
    return string.format("%02d:%02d:%02d",h,m,s)
end