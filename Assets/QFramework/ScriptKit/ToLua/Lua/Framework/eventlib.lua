-- EventLib - An event library in pure lua (uses standard coroutine library)
-- License: WTFPL
-- Author: Elijah Frederickson
-- Version: 1.0
-- Copyright (C) 2012 LoDC
-- 
-- Description:
-- ROBLOX has an event library in its RbxUtility library, but it isn't pure Lua. 
-- It originally used a BoolValue but now uses BindableEvent. I wanted to write
-- it in pure Lua, so here it is. It also contains some new features.
-- 
-- 
-- API:
-- 
-- EventLib
--   new([name])
--     aliases: CreateEvent
--     returns: the event, with a metatable __index for the EventLib table
--   Connect(event, func)
--     aliases: connect
--     returns: a Connection
--   Disconnect(event, [func])
--     aliases: disconnect
--     returns: the index of [func]
--     notes: if [func] is nil, it removes all connections
--   DisconnectAll(event)
--     notes: calls Disconnect()
--   Fire(event, ... <args>)
--     aliases: Simulate, fire
--     notes: resumes all :wait() first
--   Wait(event)
--     aliases: wait
--     returns: the Fire() arguments
--     notes: blocks the thread until Fire() is called
--   ConnectionCount(event)
--     returns: the number of current connections
--   Spawn(func)
--     aliases: spawn
--     returns: the result of func
--     notes: runs func in a separate coroutine/thread
--   Destroy(event)
--     aliases: destroy, Remove, remove
--     notes: renders the event completely useless
--   WaitForCompletion(event)
--     notes: blocks current thread until the current event Fire() is done
--       If a connected function calls WaitForCompletion, it will hang forever
--   IsWaiting(event)
--     returns: if the event has any waiters
--   WaitForWaiters(event)
--     notes: waits for all waiters to finish. Called in Fire() to make sure that all
--       the waiting threads are done before settings self.args to nil
--   
-- Event
--   [All EventLib functions]
--   EventName
--     Property, defaults to "<Unknown Event>"
--   <Private fields>
--     handlers, waiter, args, waiters, executing, 
-- 
-- Connection
--   Disconnect
--     aliases: disconnect
--     returns: the result of [Event].Disconnect
-- 
-- Basic usage (there are some tests on the bottom):
--  local EventLib = require'EventLib' 
-- For ROBLOX use: repeat wait() until _G.EventLib local EventLib = _G.EventLib
--  
--  local event = EventLib:new()
--  local con = event:Connect(function(...) print(...) end)
--  event:Fire("test") --> 'test' is print'ed
--  con:disconnect()
--  event:Fire("test") --> nothing happens: no connections
-- 
-- Supported versions/implementations of Lua:
-- Lua 5.1, 5.2
-- SharpLua 2
-- MetaLua
-- RbxLua (automatically registers if it detects ROBLOX)

--[[
Issues:
- None, but see [Todo 1]

Todo:
- fix Wait() for non-roblox clients without a wait function...

Changelog:

v1.0
- Initial version

]]

local _M = { }
_M._VERSION = "1.0"
_M._M = _M
_M._AUTHOR = "Elijah Frederickson"
_M._COPYRIGHT = "Copyright (C) 2012 LoDC"

local function spawn(f)
    return coroutine.resume(coroutine.create(function()
        f()
    end))
end
_M.Spawn = spawn
_M.spawn = spawn

function _M:new(name)
    assert(self ~= nil and type(self) == "table" and self == _M, "Invalid EventLib table (make sure you're using ':' not '.')")
    local s = { }
    s.handlers = { }
    s.waiter = false
    s.args = nil
    s.waiters = 0
    s.EventName = name or "<Unknown Event>"
    s.executing = false
    return setmetatable(s, { __index = self })
end
_M.CreateEvent = _M.new

function _M:Connect(handler)
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    assert(type(handler) == "function", "Invalid handler. Expected function got " .. type(handler))
    assert(self.handlers, "Invalid Event")
    table.insert(self.handlers, handler)
    local t = { }
    t.Disconnect = function()
        return self:Disconnect(handler)
    end
    t.disconnect = t.Disconnect
    return t
end
_M.connect = _M.Connect

function _M:Disconnect(handler)
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    assert(type(handler) == "function" or type(handler) == "nil", "Invalid handler. Expected function or nil, got " .. type(handler))
    if not handler then
        self.handlers = { }
    else
        for k, v in pairs(self.handlers) do
            if v == handler then
                self.handlers[k] = nil
                return k
            end
        end
    end
end
_M.disconnect = _M.Disconnect

function _M:DisconnectAll()
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    self:Disconnect()
end

function _M:Fire(...)
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    self.args = { ... }
    self.executing = true
    --[[
    if self.waiter then
        self.waiter = false
        for k, v in pairs(self.waiters) do
            coroutine.resume(v)
        end
    end]]
    self.waiter = false
    local i = 0
assert(self.handlers, "no handler table")
    for k, v in pairs(self.handlers) do
        i = i + 1
        spawn(function() 
            v(unpack(self.args)) 
            i = i - 1
            if i == 0 then self.executing = false end
        end)
    end
    self:WaitForWaiters()
    self.args = nil
    --self.executing = false
end
_M.Simulate = _M.Fire
_M.fire = _M.Fire

function _M:Wait()
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    self.waiter = true
    self.waiters = self.waiters + 1
    --[[
    local c = coroutine.create(function()
        coroutine.yield()
        return unpack(self.args)
    end)
    
    table.insert(self.waiters, c)
    coroutine.resume(c)
    ]]
    
    while self.waiter or not self.args do if wait then wait() end end
    self.waiters = self.waiters - 1
    return unpack(self.args)
end
_M.wait = _M.Wait

function _M:ConnectionCount()
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    return #self.handlers
end

function _M:Destroy()
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    self:DisconnectAll()
    for k, v in pairs(self) do
        self[k] = nil
    end
    setmetatable(self, { })
end
_M.destroy = _M.Destroy
_M.Remove = _M.Destroy
_M.remove = _M.Destroy

function _M:WaitForCompletion()
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    while self.executing do if wait then wait() end end
    while self.waiters > 0 do if wait then wait() end end
end

function _M:IsWaiting()
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    return self.waiter or self.waiters > 0
end

function _M:WaitForWaiters()
    assert(self ~= nil and type(self) == "table", "Invalid Event (make sure you're using ':' not '.')")
    while self.waiters > 0 do if wait then wait() end end
end

-- Tests
if false then
    local e = _M:new("test")
    local f = function(...) print("| Fired!", ...) end
    local e2 = e:connect(f)
    e:fire("arg1", 5, { })
    -- Would work in a ROBLOX Script, but not on Lua 5.1...
    if script ~= nil and failhorribly then
        spawn(function() print("Wait() results", e:wait()) print"|- done waiting!" end)
    end
    e:fire(nil, "x")
    print("Disconnected events index:", e:disconnect(f))
    print("Couldn't disconnect an already disconnected handler?", e2:disconnect()==nil)
    print("Connections:", e:ConnectionCount())
    assert(e:ConnectionCount() == 0 and e:ConnectionCount() == #e.handlers)
    e:connect(f)
    e:connect(function() print"Throwing error... " error("...") end)
    e:fire("Testing throwing an error...")
    e:disconnect()
    e:Simulate()
    f("plain function call")
    assert(e:ConnectionCount() == 0)
    
    if wait then
        e:connect(function() wait(2, true) print'fired after waiting' end)
        e:Fire()
        e:WaitForCompletion()
        print'Done!'
    end
    
    local failhorribly = false
    if failhorribly then -- causes an eternal loop in the WaitForCompletion call
        e:connect(function() e:WaitForCompletion() print'done with connected function' end)
        e:Fire()
        print'done'
    end
    
    e:Destroy()
    assert(not e.EventName and not e.Fire and not e.Connect)
end

if shared and Instance then -- ROBLOX support
    shared.EventLib = _M 
    _G.EventLib = _M
end

return _M
