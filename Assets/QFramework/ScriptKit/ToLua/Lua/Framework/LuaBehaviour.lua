--================================
-- Copyright (c) 2019.2  vin129
--游戏lua组件模版 Editor中通过Luacomponent挂载
--================================
-- LuaBehaviour = class("LuaBehaviour",ICommand)
LuaBehaviour = class("LuaBehaviour")
function LuaBehaviour:Awake()
    if self.gameObject == nil then
        logError("gameObject is nil !")
        return
    end
    self:BindUI()
    self:RegisterUIEvent()
end

function LuaBehaviour:OnEnable()
end

function LuaBehaviour:Start()
end

function LuaBehaviour:Update()
end

function LuaBehaviour:OnDisable()
end

function LuaBehaviour:OnDestroy()
end

-- 这里重写并执行绑定逻辑
-- 之后考虑自动生成绑定
function LuaBehaviour:BindUI()
end
function LuaBehaviour:RegisterUIEvent()
end


--Shortcut
function LuaBehaviour:Find(obj,path)
    if obj and path then
       return QUIHelper.FindChildGameObj(obj,path)
    end
    return nil
end
