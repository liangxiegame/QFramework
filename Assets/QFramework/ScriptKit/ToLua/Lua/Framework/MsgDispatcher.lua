--
-- Author: Your Name
-- Date: 2016-10-29 00:25:00
--
MsgDispatcher = {};

--将消息分发模块放置在Lua中的原因是，c#处理LuaTable的判断太麻烦，而且有LuaBehaviour可以接收到所有unity发出的事件
MsgDispatcher.mMsgHandlerDict = {};

--注册逻辑消息
--msg: 消息名，字符串
--self: 回调对象，table
--callback: 回调函数，function
function MsgDispatcher.RegLogicMsg(msg, self, callback)
    if not msg or msg == "" then
		Logger.Error("RegLogicMsg Message is nil or Empty");
		return;
	end
	if not callback then
		Logger.Error("RegLogicMsg callback is nil");
		return;
	end
	if not MsgDispatcher.mMsgHandlerDict[msg] then
		MsgDispatcher.mMsgHandlerDict[msg] = {};
	end
	local handlers = MsgDispatcher.mMsgHandlerDict[msg];
	--防止重复注册
	for i = 1, #handlers do
		local h = handlers[i];
		if h.self == self and h.callback == callback then
			return;
		end
	end
	local handler = { self = self, callback = callback };
	setmetatable(handler, { __mode = "v" });
	table.insert(handlers, handler);
end

--解注册逻辑消息
--msg: 消息名，字符串
--callback: 回调函数，function
function MsgDispatcher.UnRegLogicMsg(msg, self, callback)
    if not msg or msg == "" then
		Logger.Error("UnRegLogicMsg Message is nil or Empty");
		return;
	end
	if not callback then
		Logger.Error("UnRegLogicMsg callback is nil");
		return;
	end	
	local handlers = MsgDispatcher.mMsgHandlerDict[msg];
	--遍历删除需要从后往前
	for i = #handlers, 1, -1 do
		local h = handlers[i];
		if h.self == self and h.callback == callback then
			table.remove(handlers, i);
			break;
		end
	end
end

--发送逻辑消息
--msg: 消息名，字符串
--...: 消息参数
function MsgDispatcher.SendLogicMsg(msg, ...)
    if not msg or msg == "" then
		Logger.Error("UnRegLogicMsg Message is nil or Empty");
		return;
	end
	local handlers = MsgDispatcher.mMsgHandlerDict[msg];
	if handlers == nil then
		return;
	end
	for i = #handlers, 1, -1 do
		if handlers[i].self then
			handlers[i].callback(handlers[i].self, ...);
		else
			--由于注册的表是弱引用，如果注册的表已经被销毁，则从链表中移除
			table.remove(handlers, i);
		end
	end
end