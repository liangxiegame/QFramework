--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2018 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local LuaProfiler = LuaProfiler
local vmdef = jit and require("jit.vmdef")

--通过文件名和行数指定一个函数名字
local ffnames = 
{
	event =
	{
		[20] = "_xpcall.__call",
		[142] = "event.__call",		
	},

    slot = 
    {
        [11] = "slot.__call",
    },

    MainScene = 
    {
        [250] = "MainScene.Update"
    }
}

--不需要Profiler的函数
local blacklist = 
{
    ["ipairs_aux"] = 1,
    ["_xpcall.__call"] = 1,    
    ["unknow"] = 1,
}

local profiler = 
{
    mark = 1,
    cache = 1,    
}

local stacktrace = {}

function profiler:scan(t, name)
    if self.mark[t] then
        return
    end

    self.mark[t] = true    

	for k, v in pairs(t) do
        if type(k) == "string" then
		  if type(v) == "function" then
            local str = k

			if name then 
                str = name ..".".. str 
            end

            if not blacklist[str] and k ~= "__index" and k ~= "__newindex" then            
                self.cache[v] = {name = str, id = -1}
            end
		  elseif type(v) == "table" and not self.mark[v] then            
			self:scan(v, k)		            
		  end
        elseif name and k == tolua.gettag or k == tolua.settag then            
            self:scan(v, name)
        end
	end
end

function profiler:scanlibs()
    local t = package.loaded
    self.mark[t] = true

    for k, v in pairs(t) do
        if type(k) == "string" and type(v) == "table" then
            self:scan(v, k)
            local mt = getmetatable(v)

            if mt then
                self:scan(mt, k)
            end
        end
    end
end

local function findstack(func)
    local pos = #stacktrace + 1

    for i, v in ipairs(stacktrace) do
        if v == func then
            pos = i
        end
    end

    return pos
end

local function jitreturn(count)    
    local top = #stacktrace

    if top > 0 then        
        local ar = debug.getinfo (5, "f")    

        if ar then
            local func = ar.func                        
            local index = findstack(func)

            if index > top then
                ar = debug.getinfo(6, "f")                

                if ar then 
                    func = ar.func 
                    index = findstack(func) or index
                end                                    
            end

            for i = index + 1, top do
                table.remove(stacktrace)
                LuaProfiler.EndSample()
            end            
        end
    end   
end

local function BeginSample(name, func, cache)
    jitreturn()
    table.insert(stacktrace, func)    

    if cache.id == -1 then 
        cache.name = name 
        cache.id = LuaProfiler.GetID(name)
    end

    LuaProfiler.BeginSample(cache.id)  
end

local function BeginSampleNick(name, func, cache)
    jitreturn()
    table.insert(stacktrace, func)        
    local id = -1

    if cache.nick == nil then 
        cache.nick = {}        
    end

    id = cache.nick[name]

    if not id then
        id = LuaProfiler.GetID(name)
        cache.nick[name] = id
    end

    LuaProfiler.BeginSample(id)  
end

function profiler_hook(event, line)    
    if event == 'call' then        
        local name = nil
        local func = debug.getinfo (2, 'f').func
        local cache = profiler.cache[func]        

        if cache then
            name = cache.name         
        end

        if blacklist[name] then                         
            return
        end     
        
        if name == "event.__call" then            
            local ar = debug.getinfo (2, 'n')                  
            BeginSampleNick(ar.name or name, func, cache)
        elseif name then          
            BeginSample(name, func, cache)                                                                      
        else
            local ar = debug.getinfo (2, 'Sn')
            local method = ar.name
            local linedefined = ar.linedefined          

            if not cache then
                cache = {name = "unknow", id = -1}
                profiler.cache[func] = cache
            end    

            if ar.short_src == "[C]" then
                if method == "__index" or method == "__newindex" then
                    return
                end

                local name = tostring(func)                                                             
                local index = name:match("function: builtin#(%d+)")       

                if not index then   
                    if method then
                        name = method    
                        BeginSample(method, func, cache)                                             
                    elseif linedefined ~= -1 then
                        name = ar.short_src .. linedefined                        
                        BeginSample(name, func, cache)                              
                    end 
                else
                    name = vmdef.ffnames[tonumber(index)]

                    if not blacklist[name] then                        
                        BeginSample(name, func, cache)                      
                    end
                end                              
            elseif linedefined ~= -1 or method then
                local short_src = ar.short_src
                method = method or linedefined

                local name = nil                
                name = short_src:match('([^/\\]+)%.%w+$')
                name = name or short_src:match('([^/\\]+)$')

                local ffname = ffnames[name]
                
                if ffname then
                    name = ffname[linedefined]   
                else
                    name = name .. "." .. method                 
                end
                
                if not name then
                    name = short_src .. "." .. method
                end
                                                                            
                BeginSample(name, func, cache)
            else      
                BeginSample(name, func, cache)                                                                                         
            end
        end
    elseif event == "return" then   
        local top = #stacktrace

        if top == 0 then                                
            return 
        end

        local ar = debug.getinfo (2, 'f')

        if ar.func == stacktrace[top] then            
            table.remove(stacktrace)
            LuaProfiler.EndSample()         
        else            
            local index = findstack(ar.func)
            if index > top then return end

            for i = index, top do
                table.remove(stacktrace)
                LuaProfiler.EndSample()
            end
        end
    end    
end

function profiler:start()
    self.mark = {}    
    self.cache = {__mode = "k"}
    
    self:scan(_G, nil)
    self:scanlibs()
    self.mark = nil

    debug.sethook(profiler_hook, 'cr', 0)
end

function profiler:print()
    for k, v in pairs(self.cache) do
        print(v.name)
    end
end

function profiler:stop()
    debug.sethook(nil)    
    self.cache = nil          
end

return profiler