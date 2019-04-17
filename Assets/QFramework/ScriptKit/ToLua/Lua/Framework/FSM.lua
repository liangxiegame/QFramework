--
-- Author: Your Name
-- Date: 2016-10-29 00:24:29
--
--状态基类
FSMState = class("FSMState");

function FSMState:initialize(name)
    self.mName = name;
end

function FSMState:OnEnter(...)
end

--从当前状态再次进入此状态时的回调
function FSMState:OnReEnter(...)
end

function FSMState:OnUpdate()
end

function FSMState:OnExit(...)
end

--当前状态下处理逻辑
function FSMState:HandleMsg(msg, ...)
end



--状态机类
FSM = class("FSM");

function FSM:initialize()
    self.mStates = {};
    self.mCurState = nil;
    self.mTranslationDict = {};
end

--添加状态
function FSM:AddState(state)
    table.insert(self.mStates, state);
    return state;
end

--添加跳转
function FSM:AddTranslation(state, event, nextState)
    local translationDict = self.mTranslationDict[state];
    if translationDict == nil then
        self.mTranslationDict[state] = {};
        translationDict = self.mTranslationDict[state];
    end
    if translationDict[event] then
        error("Translation already exists : " .. state.mName .. " + " .. event .. " -> " .. nextState.mName);
        return;
    end
    translationDict[event] = nextState;
end

--处理事件
function FSM:HandleEvent(event, ...)
    local translationDict = self.mTranslationDict[self.mCurState];
    if not translationDict then
        return;
    end
    local nextState = translationDict[event];
    if not nextState then
        return;
    end
    if nextState == self.mCurState then
        self.mCurState:OnReEnter(...);
    else
        self.mCurState:OnExit();
        self.mCurState = nextState;
        self.mCurState:OnEnter(...);
    end
end

--启动状态机
function FSM:Start(state, ...)
    self.mCurState = state;
    self.mCurState:OnEnter(...);
end

function FSM:Update()
    self.mCurState:OnUpdate();
end
