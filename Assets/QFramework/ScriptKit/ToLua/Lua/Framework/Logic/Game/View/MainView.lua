--================================
-- Copyright (c) 2019.2  vin129
--================================
local MainView = class("MainView",LuaBehaviour)

function MainView:BindUI()
    self.StartBtn = self:Find(self.gameObject,"StartBtn")
end

function MainView:RegisterUIEvent()
    QUIHelper.SetButtonClickEvent(self.StartBtn,function() 
        log("Hello World")
    end)
end

return MainView.new();